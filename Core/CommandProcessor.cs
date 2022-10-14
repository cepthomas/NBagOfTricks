using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;


namespace Ephemera.NBagOfTricks
{
    /// <summary>Argument and value options: None, required, optional.</summary>
    public enum ArgOptType { Req, Opt, None }

    /// <summary>Main processor.</summary>
    public class Processor
    {
        #region Properties
        /// <summary>Denotes the start of an argument name.</summary>
        public static string ArgumentPrefix { get; set; } = "-";

        /// <summary>All the commands.</summary>
        public Commands Commands { get; set; } = new Commands();

        /// <summary>Selected command name.</summary>
        public string CommandName { get; private set; } = "";

        /// <summary>Missing args etc.</summary>
        public List<string> Errors { get; private set; } = new List<string>();
        #endregion

        /// <summary>
        /// Parse the cmd string using our definitions.
        /// </summary>
        /// <param name="cmdString">String to parse.</param>
        /// <param name="skipFirst">Ignore first string, usually the exe name.</param>
        /// <returns>The main command name or empty if failed.</returns>
        public bool Parse(string cmdString, bool skipFirst = false)
        {
            Errors.Clear();
            Commands.ForEach(c => c.Errors.Clear());

            List<string> parts = cmdString.SplitQuotedString();
            if(parts.Count > 0 && skipFirst)
            {
                parts.RemoveAt(0);
            }

            if (parts.Count > 0)
            {
                // Find the cmd in our list.
                var vcmd = from c in Commands
                           where string.IsNullOrEmpty(c.Name) || c.NameParts.Contains(parts[0])
                           select c;

                if (vcmd.Any())
                {
                    Command cmd = vcmd.First();
                    if(string.IsNullOrEmpty(cmd.Name))
                    {
                        // No command name.
                        CommandName = "";
                    }
                    else
                    {
                        CommandName = cmd.NameParts[0];
                        parts.RemoveAt(0);
                    }
                    cmd.Parse(parts);
                    Errors.AddRange(cmd.Errors);
                }
                else
                {
                    Errors.Add($"Invalid command:{parts[0]}");
                }
            }
            else
            {
                Errors.Add($"Empty command");
            }

            return Errors.Count == 0;
        }

        /// <summary>Format the usage help text.</summary>
        /// <param name="scmd">Specific command or list of commands if empty.</param>
        /// <returns></returns>
        public string GetUsage(string scmd = "")
        {
            List<string> lines = new();

            // Find the cmd in our list.
            var vcmd = from c in Commands
                        where string.IsNullOrEmpty(c.Name) || c.NameParts.Contains(scmd)
                        select c;

            if (vcmd.Any())
            {
                Command cmd = vcmd.First();
                StringBuilder sb = new($"Usage: {string.Join("|", cmd.NameParts)}");

                foreach (var a in cmd.Args)
                {
                    string sval = "";
                    if (a.ValOpt == ArgOptType.Req) sval = $" val";
                    else if (a.ValOpt == ArgOptType.Opt) sval = $" [val]";

                    if (a.ArgOpt == ArgOptType.Req) sb.Append($" -{a.Name}{sval}");
                    else if (a.ArgOpt == ArgOptType.Opt) sb.Append($" [-{a.Name}{sval}]");
                    else sb.Append("!Invalid arg!");
                }

                // Tail info?
                if(cmd.FileFunc is not null)
                {
                    sb.Append(" file(s)...");
                }
                lines.Add(sb.ToString());

                foreach (var a in cmd.Args)
                {
                    lines.Add($"  {a.Name}: {a.Description}");
                }
            }
            else
            {
                // NG - Show all commands.
                lines.Add("Commands:");
                Commands.ForEach(c => lines.Add($"  {string.Join("|", c.NameParts)}: {c.Description}"));
            }

            return string.Join(Environment.NewLine, lines);
        }
    }

    /// <summary>One possible command.</summary>
    public class Command
    {
        #region Properties - filled in by client
        /// <summary>
        /// The command name(s) space separated. The first one is the main command name and aliases follow.
        /// If it's empty or null, there is no separate command name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>For usage.</summary>
        public string Description { get; set; } = "";

        /// <summary>Possible arguments for this command.</summary>
        public Arguments Args { get; set; } = new();

        /// <summary>Handler for processing stuff at the end, typically file names.</summary>
        public Func<string, bool>? FileFunc { get; set; } = null;
        #endregion

        #region Properties - filled in by Parser
        /// <summary>Missing args etc.</summary>
        public List<string> Errors { get; internal set; } = new List<string>();

        /// <summary>Split version of Name.</summary>
        public List<string> NameParts { get; internal set; } = new List<string>();
        #endregion

        /// <summary>Parse the argument collection.</summary>
        public void Parse(List<string> args)
        {
            string sarg = "";

            for (int i = 0; i < args.Count; i++)
            {
                Argument? currentArg = null;
                sarg = args[i].Trim();

                if (sarg.StartsWith(Processor.ArgumentPrefix)) ///// New argument.
                {
                    string argName = sarg.Replace(Processor.ArgumentPrefix, "");

                    // Find the new arg in our list.
                    var qry = from ca in Args
                              where ca.Name == argName
                              select ca;

                    if (qry.Any()) // found it
                    {
                        currentArg = qry.First();
                        currentArg.Valid = true;
                    }
                    else
                    {
                        Errors.Add($"Unexpected arg:{argName}");
                        currentArg = null;
                    }

                    if (currentArg is not null)
                    {
                        string lookAhead = i + 1 < args.Count ? args[i + 1] : "";

                        switch(currentArg.ValOpt)
                        {
                            case ArgOptType.Req:
                                if (!lookAhead.StartsWith(Processor.ArgumentPrefix))
                                {
                                    if (currentArg.ArgFunc?.Invoke(lookAhead) == false)
                                    {
                                        Errors.Add($"Problem with arg:{currentArg.Name}");
                                        currentArg = null;
                                    }
                                    else
                                    {
                                        i++; // bump ahead
                                    }
                                }
                                else
                                {
                                    Errors.Add($"Missing required value for arg:{currentArg.Name}");
                                    currentArg = null;
                                }
                                break;

                            case ArgOptType.Opt:
                                if (!lookAhead.StartsWith(Processor.ArgumentPrefix))
                                {
                                    // Valid (maybe) arg val. Execute any requested func.
                                    if (currentArg.ArgFunc?.Invoke(lookAhead) == false)
                                    {
                                        Errors.Add($"Problem with arg:{currentArg.Name}");
                                        currentArg = null;
                                    }
                                    else
                                    {
                                        i++; // bump ahead
                                    }
                                }
                                // else ignore
                                break;

                            case ArgOptType.None:
                                // Execute any requested func with no val.
                                if (currentArg.ArgFunc?.Invoke("") == false)
                                {
                                    Errors.Add($"Problem with arg:{currentArg.Name}");
                                    currentArg = null;
                                }
                                break;
                        }
                    }
                }
                else // it's a file or extra thing
                {
                    if(FileFunc is not null)
                    {
                        if (FileFunc.Invoke(sarg) == false)
                        {
                            Errors.Add($"Problem with:{sarg}");
                        }
                    }
                    else
                    {
                        Errors.Add($"Extraneous value:{sarg}");
                    }
                }
            }

            // Look for missing required args.
            foreach (Argument ca in Args)
            {
                if (ca.ArgOpt == ArgOptType.Req && !ca.Valid)
                {
                    Errors.Add($"Missing arg:{ca.Name}");
                }
            }
        }
    }

    /// <summary>Client fills in specs and CommandParser does the rest.</summary>
    public class Argument
    {
        #region Properties - filled in by client
        /// <summary>The command line value.</summary>
        public string Name { get; internal set; } = "";

        /// <summary>For usage.</summary>
        public string Description { get; internal set; } = "";

        /// <summary>Argument options.</summary>
        public ArgOptType ArgOpt { get; internal set; } = ArgOptType.None;

        /// <summary>Value options.</summary>
        public ArgOptType ValOpt { get; internal set; } = ArgOptType.None;

        /// <summary>How to process the arg. Can include validation - returns true/false.</summary>
        public Func<string, bool>? ArgFunc { get; internal set; } = null;
        #endregion

        #region Properties - filled in by Parser
        /// <summary>Does it appear in the command line.</summary>
        public bool Valid { get; internal set; } = false;
        #endregion
    }

    /// <summary>Specialized container. Has Add() to support initialization.</summary>
    public class Commands : List<Command>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        /// <param name="args"></param>
        /// <param name="func"></param>
        public void Add(string name, string desc, Arguments args, Func<string, bool>? func = null)
        {
            var cmd = new Command()
            {
                Name = name,
                Description = desc,
                Args = args,
                FileFunc = func
            };
            cmd.NameParts = name.SplitQuotedString();
            Add(cmd);
        }
    }

    /// <summary>Specialized container. Has Add() to support initialization.</summary>
    public class Arguments : List<Argument>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        /// <param name="argopt"></param>
        /// <param name="valopt"></param>
        /// <param name="func"></param>
        public void Add(string name, string desc, ArgOptType argopt, ArgOptType valopt, Func<string, bool>? func = null)
        {
            var arg = new Argument()
            {
                Name = name,
                Description = desc,
                ArgOpt = argopt,
                ValOpt = valopt,
                ArgFunc = func
            };
            Add(arg);
        }
    }
}