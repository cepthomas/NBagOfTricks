using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NBagOfTricks;
using NBagOfTricks.PNUT;
using NBagOfTricks.ScriptCompiler;



namespace NBagOfTricks.Test
{
    public class COMP_SIMPLE : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests script compiler on a simple block of code.");

            // Compile script.
            ScriptCompilerCore compiler = new();

            compiler.CompileText(code);

            UT_EQUAL(compiler.Results.Count, 1);

            compiler.Results.ForEach(res => UT_INFO(res.ToString()));
        }

        readonly string code = @"//1
        using System;
        using System.Collections.Generic;
        using System.IO;
//5
        namespace NbotTest
        {
            public class Bag
            {
                /// <summary>The file name.</summary> //10
                string FileName { get; set; } = ""abcd"";

                public Dictionary<string, object> Values { get; set; } = new();
                //public Dictionary<string, object> Values { get; set; } = new Dictionary<string, object>();

                public bool Valid => false;
                //public bool Valid { get; set; } = false;

                #region HooHaa
                public double GetDouble(string owner, string valname, double defval) //20
                {
                    var ret = 123.45;
                    return ret;
                }

                public int GetInteger(string owner, string valname, int defval)
                {
                    int ret = int.MinValue;
                    return ret;
                }// 30
                #endregion
            }
            
            public record Person(string FirstName, string LastName);
        }";
    }

    //public class TestCompiler : ScriptCompilerCore
    //{
    //    ///// <summary>Channel info collected from the script.</summary>
    //    //public List<ChannelSpec> ChannelSpecs { get; init; } = new();
    //    /// <summary>Called before compiler starts.</summary>
    //    public override void PreCompile()
    //    {
    //        LocalDlls = new() { "NAudio", "NBagOfTricks", "NebOsc", "MidiLib", "Nebulator.Script" };
    //        Usings.Add("static NBagOfTricks.MusicDefinitions");
    //    }
    //    /// <summary>Called after compiler finished.</summary>
    //    public override void PostCompile()
    //    {
    //    }
    //    /// <summary>Called for each line in the source file before compiling.</summary>
    //    public override bool PreprocessLine(string sline, FileContext pcont)
    //    {
    //        bool handled = false;
    //        // Channel spec - grab it.
    //        if (sline.StartsWith("Channel"))
    //        {
    //            try
    //            {
    //                var parts = sline.Replace("\"", "").SplitByTokens("(),;");
    //            }
    //            catch (Exception)
    //            {
    //                Results.Add(new()
    //                {
    //                    ResultType = CompileResultType.Error,
    //                    Message = $"Bad statement:{sline}",
    //                    SourceFile = pcont.SourceFile,
    //                    LineNumber = pcont.LineNumber
    //                });
    //            }
    //            // Exclude from output file.
    //            handled = true;
    //        }
    //        return handled;
    //    }
    //}

}
