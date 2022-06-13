using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

#nullable enable

namespace NBagOfTricks.ScriptCompiler
{
    /// <summary>Parses/compiles script file(s).</summary>
    public class ScriptCompilerCore
    {
        #region Properties
        /// <summary>Client needs to tell us this.</summary>
        public bool IgnoreWarnings { get; set; } = true;

        /// <summary>Default system dlls. Client can add or subtract.</summary>
        public List<string> SystemDlls { get; } = new() { "System", "System.Private.CoreLib", "System.Runtime", "System.Collections", "System.Linq" };

        /// <summary>App dlls.</summary>
        public List<string> LocalDlls { get; set; } = new();

        /// <summary>Additional using statements not supplied by dlls.</summary>
        public List<string> Usings { get; set; } = new() { "System.Collections.Generic", "System.Text" };

        /// <summary>The compiled script.</summary>
        public object? Script { get; set; } = null;

        /// <summary>Accumulated errors/results.</summary>
        public List<CompileResult> Results { get; } = new List<CompileResult>();

        /// <summary>All active source files. Provided so client can monitor for external changes.</summary>
        public IEnumerable<string> SourceFiles { get { return _filesToCompile.Values.Select(f => f.SourceFile).ToList(); } }

        /// <summary>Compile products are here.</summary>
        public string TempDir { get; set; } = "";
        #endregion

        #region Fields
        /// <summary>Script info.</summary>
        string _scriptName = "";

        /// <summary>Accumulated lines to go in the constructor.</summary>
        readonly List<string> _initLines = new();

        /// <summary>Products of file preprocess. Key is generated file name.</summary>
        readonly Dictionary<string, FileContext> _filesToCompile = new();
        #endregion

        #region Overrides for derived classes to hook
        /// <summary>Hook to override.</summary>
        public virtual void PreExecute() { }

        /// <summary>Hook to override.</summary>
        public virtual void PostExecute() { }

        /// <summary>Hook to override.</summary>
        /// <param name="sline">Trimmed line</param>
        /// <param name="pcont">File context</param>
        /// <returns>True if handled</returns>
        public virtual bool PreprocessFile(string sline, FileContext pcont) { return false; }
        #endregion

        #region Public functions
        /// <summary>
        /// Run the Compiler.
        /// </summary>
        /// <param name="scriptfn">Fully qualified path to main file.</param>
        public void Execute(string scriptfn)
        {
            // Reset everything.
            Script = null;
            Results.Clear();
            _filesToCompile.Clear();
            _initLines.Clear();

            if (File.Exists(scriptfn))
            {
                PreExecute();

                Results.Add(new CompileResult()
                {
                    ResultType = CompileResultType.Info,
                    Message = $"Compiling {scriptfn}."
                });

                ///// Get and sanitize the script name.
                _scriptName = Path.GetFileNameWithoutExtension(scriptfn);
                StringBuilder sb = new();
                _scriptName.ForEach(c => sb.Append(char.IsLetterOrDigit(c) ? c : '_'));
                _scriptName = sb.ToString();
                var dir = Path.GetDirectoryName(scriptfn);

                ///// Compile.
                DateTime startTime = DateTime.Now; // for metrics

                ///// Process the source files into something that can be compiled. PreprocessFile is a recursive function.
                FileContext pcont = new()
                {
                    SourceFile = scriptfn,
                    LineNumber = 1
                };
                PreprocessFile(pcont);

                ///// Compile the processed files.
                Script = CompileNative(dir!);

                Results.Add(new CompileResult()
                {
                    ResultType = CompileResultType.Info,
                    Message = $"Compile took {(DateTime.Now - startTime).Milliseconds} msec."
                });

                PostExecute();
            }
            else
            {
                Results.Add(new CompileResult()
                {
                    ResultType = CompileResultType.Error,
                    Message = $"Invalid file {scriptfn}."
                });
            }
        }
        #endregion

        #region Private functions
        /// <summary>
        /// The actual compiler driver.
        /// </summary>
        /// <returns>Compiled script</returns>
        object? CompileNative(string baseDir)
        {
            object? script = null;

            try // many ways to go wrong...
            {
                // Create temp output area and/or clean it.
                TempDir = Path.Combine(baseDir, "temp");
                Directory.CreateDirectory(TempDir);
                Directory.GetFiles(TempDir).ForEach(f => File.Delete(f));

                ///// Assemble constituents.
                List<SyntaxTree> trees = new();

                // Write the generated source files to temp build area.
                foreach (string genFn in _filesToCompile.Keys)
                {
                    FileContext ci = _filesToCompile[genFn];
                    string fullpath = Path.Combine(TempDir, genFn);
                    File.Delete(fullpath);
                    File.WriteAllLines(fullpath, ci.CodeLines);

                    // Build a syntax tree.
                    string code = File.ReadAllText(fullpath);
                    CSharpParseOptions popts = new();
                    SyntaxTree tree = CSharpSyntaxTree.ParseText(code, popts, genFn);
                    trees.Add(tree);
                }

                // We now build up a list of references needed to compile the code.
                var references = new List<MetadataReference>();
                // System stuff location.
                var dotnetStore = Path.GetDirectoryName(typeof(object).Assembly.Location);
                // Project refs like nuget.
                var localStore = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                // System dlls.
                foreach (var dll in SystemDlls)
                {
                    references.Add(MetadataReference.CreateFromFile(Path.Combine(dotnetStore!, dll + ".dll")));
                }

                // Local dlls.
                foreach (var dll in LocalDlls)
                {
                    references.Add(MetadataReference.CreateFromFile(Path.Combine(localStore!, dll + ".dll")));
                }

                ///// Emit to stream.
                var copts = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
                var compilation = CSharpCompilation.Create($"{_scriptName}.dll", trees, references, copts);
                var ms = new MemoryStream();
                EmitResult result = compilation.Emit(ms);

                if (result.Success)
                {
                    // Load into currently running assembly and locate the new script.
                    var assy = Assembly.Load(ms.ToArray());
                    var types = assy.GetTypes();

                    foreach (Type t in types)
                    {
                        if (t is not null && t.Name == _scriptName)
                        {
                            // We have a good script file. Create the executable object.
                            object? o = Activator.CreateInstance(t);
                            if(o is not null)
                            {
                                script = o;
                            }
                        }
                    }
                }

                ///// Compile results.
                foreach (var diag in result.Diagnostics)
                {
                    CompileResult se = new();
                    se.Message = diag.GetMessage();
                    bool keep = true;

                    switch (diag.Severity)
                    {
                        case DiagnosticSeverity.Error:
                            se.ResultType = CompileResultType.Error;
                            break;

                        case DiagnosticSeverity.Warning:
                            if (IgnoreWarnings)
                            {
                                keep = false;
                            }
                            else
                            {
                                se.ResultType = CompileResultType.Warning;
                            }
                            break;

                        case DiagnosticSeverity.Info:
                            se.ResultType = CompileResultType.Info;
                            break;

                        case DiagnosticSeverity.Hidden:
                            if (IgnoreWarnings)
                            {
                                keep = false;
                            }
                            else
                            {
                                //?? se.ResultType = CompileResultType.Warning;
                            }
                            break;
                    }

                    var genFileName = diag.Location.SourceTree!.FilePath;
                    var genLineNum = diag.Location.GetLineSpan().StartLinePosition.Line; // 0-based

                    // Get the original info.
                    if (_filesToCompile.TryGetValue(Path.GetFileName(genFileName), out var context))
                    {
                        se.SourceFile = context.SourceFile;
                        // Dig out the original line number.
                        string origLine = context.CodeLines[genLineNum];
                        int ind = origLine.LastIndexOf("//");
                        if (ind != -1)
                        {
                            se.LineNumber = int.TryParse(origLine[(ind + 2)..], out int origLineNum) ? origLineNum : -1; // 1-based
                        }
                    }
                    else
                    {
                        // Presumably internal generated file - should never have errors.
                        se.SourceFile = "";
                    }

                    if(keep)
                    {
                        Results.Add(se);
                    }
                }
            }
            catch (Exception ex)
            {
                Results.Add(new CompileResult()
                {
                    ResultType = CompileResultType.Error,
                    Message = "Exception: " + ex.Message,
                });
            }

            return script;
        }

        /// <summary>
        /// Parse one file. Recursive to support nested include(fn).
        /// </summary>
        /// <param name="pcont">The parse context.</param>
        /// <returns>True if a valid file.</returns>
        bool PreprocessFile(FileContext pcont)
        {
            bool valid = File.Exists(pcont.SourceFile);

            if (valid)
            {
                string genFn = $"{_scriptName}_src{_filesToCompile.Count}.cs".ToLower();
                _filesToCompile.Add(genFn, pcont);

                ///// Preamble.
                pcont.CodeLines.AddRange(GenTopOfFile(pcont.SourceFile));

                ///// The content.
                List<string> sourceLines = new(File.ReadAllLines(pcont.SourceFile));

                for (pcont.LineNumber = 1; pcont.LineNumber <= sourceLines.Count; pcont.LineNumber++)
                {
                    string s = sourceLines[pcont.LineNumber - 1];

                    // Remove any comments. Single line type only.
                    int pos = s.IndexOf("//");
                    string cline = pos >= 0 ? s.Left(pos) : s;

                    // Test for preprocessor directives.
                    string strim = s.Trim();

                    // like Include("path\name.neb");
                    //Include(path\utils.neb);
                    if (strim.StartsWith("Include"))
                    {
                        // Exclude from output file.
                        List<string> parts = strim.SplitByTokens("()");
                        if (parts.Count >= 2)
                        {
                            string fn = parts[1];

                            // Recursive call to parse this file
                            FileContext subcont = new()
                            {
                                SourceFile = fn,
                                LineNumber = 1
                            };
                            valid = PreprocessFile(subcont);
                        }
                        else
                        {
                            valid = false;
                        }

                        if (!valid)
                        {
                            Results.Add(new CompileResult()
                            {
                                ResultType = CompileResultType.Error,
                                Message = $"Invalid Include: {strim}",
                                SourceFile = pcont.SourceFile,
                                LineNumber = pcont.LineNumber
                            });
                        }
                    }
                    else if (PreprocessFile(strim, pcont))
                    {
                       // NOP
                    }
                    else // plain line
                    {
                        if (cline.Trim() != "")
                        {
                            // Store the whole line with line number tacked on and some indentation.
                            pcont.CodeLines.Add($"        {cline} //{pcont.LineNumber}");
                        }
                    }
                }

                ///// Postamble.
                pcont.CodeLines.AddRange(GenBottomOfFile());
            }

            return valid;
        }

        /// <summary>
        /// Create the boilerplate file top stuff.
        /// </summary>
        /// <param name="fn">Source file name. Empty means it's an internal file.</param>
        /// <returns></returns>
        List<string> GenTopOfFile(string fn)
        {
            string origin = fn == "" ? "internal" : fn;

            // Create the common contents.
            List<string> codeLines = new()
            {
                $"// Created from:{origin}",
            };

            SystemDlls.ForEach(d => codeLines.Add($"using {d};"));
            LocalDlls.ForEach(d => codeLines.Add($"using {d};"));
            Usings.ForEach(d => codeLines.Add($"using {d};"));

            codeLines.AddRange(new List<string>()
            {
                "",
                "namespace Nebulator.UserScript",
                "{",
               $"    public partial class {_scriptName} : ScriptBase",
                "    {"
            });

            return codeLines;
        }

        /// <summary>
        /// Create the boilerplate file bottom stuff.
        /// </summary>
        /// <returns></returns>
        List<string> GenBottomOfFile()
        {
            // Create the common contents.
            List<string> codeLines = new()
            {
                "    }",
                "}"
            };

            return codeLines;
        }
        #endregion
    }
}
