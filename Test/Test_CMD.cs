using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using NBagOfTricks.PNUT;
using NBagOfTricks.CommandProcessor;


namespace NBagOfTricks.Test
{
    ////////////////////////////////////////////////////////////////////////
    public class CMD_HAPPY : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Test the happy path for the Command Processor.");

            // Captured values.
            List<string> txtFiles = new List<string>();
            List<string> docFiles = new List<string>();
            Dictionary<string, string> args = new Dictionary<string, string>();
            void ClearArgs()
            {
                txtFiles.Clear();
                docFiles.Clear();
                args.Clear();
            }

            // Create the specs for the command processor.
            var cp = new Processor();

            ///////// Happy path //////////
            cp.Commands.Add(new Command()
            {
                Name = new string[] { "help", "h", "?" },
                Description = "what do you want to know",    
                Args = new Arguments
                {
                    { "cmd", "specific command or empty for all commands",  Arg.Opt, Param.Opt,  (v) => { Console.Write(cp.GetUsage(v)); return true; } },
                }
            });

            cp.Commands.Add(new Command()
            {
                Name = new string[]{ "real main cmd", "go", "r" },
                Description = "just a command that does amazing things with some arguments",
                Args = new Arguments
                {
                    { "abc", "arg with optional param",     Arg.Opt, Param.Opt,  (v) => { args.Add("abc", v);  return true; } },
                    { "def", "arg no param",                Arg.Req, Param.None, (v) => { args.Add("def", ""); return true; } },
                    { "ghi", "arg without optional param",  Arg.Opt, Param.Opt,  (v) => { args.Add("ghi", ""); return true; } },
                    { "jkl", "arg with requred parameter",  Arg.Req, Param.Req,  (v) => { args.Add("jkl", v);  return true; } },
                },
                TailInfo = "Filename(s) to process",
                TailFunc = (v) =>
                {
                    if (v.EndsWith(".txt")) { txtFiles.Add(v); return true; }
                    else if (v.EndsWith(".doc")) { docFiles.Add(v); return true; }
                    else { return false; }
                }
            });

            cp.Commands.Add(new Command()
            {
                Name = new string[] { "dooda", "d" },
                Description = "another command",
                Args = new Arguments
                {
                    { "darg1", "up side",      Arg.Opt, Param.Opt,  (v) => { args.Add("darg1", v); return true; } },
                    { "darg2", "down side",    Arg.Opt, Param.Opt,  (v) => { args.Add("darg2", v); return true; } },
                }
            });

            /////// Basic processing ///////
            ClearArgs();
            string testCmd = "\"real main cmd\" -def -jkl \"thing one\" -ghi -abc mememe InputFile1.txt InputFile2.doc InputFile3.doc";
            string cmd = cp.Parse(testCmd);

            UT_EQUAL(cp.Errors.Count, 0);
            cp.Errors.ForEach(err => UT_INFO(err));

            UT_EQUAL(cmd, "real main cmd");

            UT_EQUAL(txtFiles.Count, 1);
            UT_EQUAL(txtFiles[0], "InputFile1.txt");

            UT_EQUAL(docFiles.Count, 2);
            UT_EQUAL(docFiles[0], "InputFile2.doc");
            UT_EQUAL(docFiles[1], "InputFile3.doc");

            UT_EQUAL(args.Count, 4);
            UT_TRUE(args.ContainsKey("abc"));
            UT_EQUAL(args["abc"], "mememe");
            UT_TRUE(args.ContainsKey("def"));
            UT_EQUAL(args["def"], "");
            UT_TRUE(args.ContainsKey("ghi"));
            UT_EQUAL(args["ghi"], "");
            UT_TRUE(args.ContainsKey("jkl"));
            UT_EQUAL(args["jkl"], "thing one");

            /////// Alias ///////
            ClearArgs();
            testCmd = "d -darg1 \"shiny monster\" -darg2";
            UT_EQUAL(cp.Parse(testCmd), "dooda");

            UT_EQUAL(args.Count, 2);
            UT_TRUE(args.ContainsKey("darg1"));
            UT_EQUAL(args["darg1"], "shiny monster");
            UT_TRUE(args.ContainsKey("darg2"));
            UT_EQUAL(args["darg2"], "");

            /////// Usage ///////

            //UT_EQUAL(cp.GetUsage().Length, 169);
            //UT_EQUAL(cp.GetUsage("?").Length, 83);
            //UT_EQUAL(cp.GetUsage("dooda").Length, 85);
            //UT_EQUAL(cp.GetUsage("realcmd").Length, 199);

            UT_TRUE(cp.GetUsage().StartsWith("xxx"));
            UT_TRUE(cp.GetUsage("?").StartsWith("xxx"));
            UT_TRUE(cp.GetUsage("dooda").StartsWith("xxx"));
            UT_TRUE(cp.GetUsage("real main cmd").StartsWith("xxx"));
        }
    }

    ////////////////////////////////////////////////////////////////////////
    public class CMD_SAD : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Test the failure modes of Command Processor.");

            // Create the specs for the command processor.
            var cp = new Processor();

            ///////// Sad path //////////
            cp.Commands.Add(new Command()
            {
                Name = new string[] { "cmderrors", "g" },
                Description = "Just a command that does amazing things with some arguments",
                Args = new Arguments
                {
                    { "def", "required arg missing",             Arg.Req, Param.None, (v) => { return true; } },
                    { "jkl", "arg with missing required param",  Arg.Req, Param.Req,  (v) => { return true; } },
                    { "abc", "extraneous param",                 Arg.Req, Param.None, (v) => { return true; } },
                    { "ghi", "arg with bad validate",            Arg.Opt, Param.Opt,  (v) => { return false; } },
                }
            });


            /////// Basic processing ///////
            string testCmd = "cmderrors -unexpctedarg -abc xtra -jkl -ghi some1";

            UT_EQUAL(cp.Parse(testCmd), "");
            UT_EQUAL(cp.Errors.Count, 5);
            //cp.Errors.ForEach(err => UT_INFO(err));
            UT_TRUE(cp.Errors.Contains("Unexpected arg:unexpctedarg"));
            UT_TRUE(cp.Errors.Contains("Extraneous value:xtra"));
            UT_TRUE(cp.Errors.Contains("Missing param for arg:jkl"));
            UT_TRUE(cp.Errors.Contains("Problem with arg:ghi"));
            UT_TRUE(cp.Errors.Contains("Missing arg:def"));

            testCmd = "badcmd -fff";
            cp.Parse(testCmd);

            UT_EQUAL(cp.Errors.Count, 1);
            //cp.Errors.ForEach(err => UT_INFO(err));
            UT_TRUE(cp.Errors.Contains("Invalid command:badcmd"));

            /////// Usage ///////
            UT_EQUAL(cp.GetUsage("ng-cmd").Length, 89);
        }
    }

    ////////////////////////////////////////////////////////////////////////
    public class CMD_NONE : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Test the no-specific-command.");

            // Captured values
            List<string> txtFiles = new List<string>();
            List<string> docFiles = new List<string>();
            bool abcFlag = false;
            string abcObj = "";
            bool defFlag = false;
            bool ghiFlag = false;
            bool jklFlag = false;
            string jklObj = "";

            // Create the specs for the command processor.
            var cp = new Processor();

            cp.Commands.Add(new Command()
            {
                Name = null,
                Description = "Just a default command",
                Args = new Arguments
                {
                    { "abc", "arg with optional param",     Arg.Opt, Param.Opt,  (v) => { abcFlag = true; abcObj = v; return true; } },
                    { "def", "arg no param",                Arg.Req, Param.None, (v) => { defFlag = true; return true; } },
                    { "ghi", "arg without optional param",  Arg.Opt, Param.Opt,  (v) => { ghiFlag = true; return true; } },
                    { "jkl", "arg with requred parameter",  Arg.Req, Param.Req,  (v) => { jklFlag = true; jklObj = v; return true; } },
                },
                TailInfo = "Filename(s) to process",
                TailFunc = (v) =>
                {
                    if (v.EndsWith(".txt")) { txtFiles.Add(v); return true; }
                    else if (v.EndsWith(".doc")) { docFiles.Add(v); return true; }
                    else { return false; }
                }
            });

            /////// Basic processing ///////
            string testCmd = "-def -jkl some1 -ghi -abc some2 InputFile1.txt InputFile2.doc InputFile3.doc";
            cp.Parse(testCmd);

            UT_EQUAL(cp.Errors.Count, 0);
            //cp.Errors.ForEach(err => UT_INFO(err));

            UT_EQUAL(txtFiles.Count, 1);
            UT_EQUAL(txtFiles[0], "InputFile1.txt");

            UT_EQUAL(docFiles.Count, 2);
            UT_EQUAL(docFiles[0], "InputFile2.doc");
            UT_EQUAL(docFiles[1], "InputFile3.doc");

            UT_TRUE(abcFlag);
            UT_TRUE(defFlag);
            UT_TRUE(ghiFlag);
            UT_TRUE(jklFlag);

            UT_EQUAL(abcObj, "some2");
            UT_EQUAL(jklObj, "some1");

            UT_TRUE(cp.GetUsage().StartsWith("xxx"));
            UT_TRUE(cp.GetUsage("?").StartsWith("xxx"));
        }
    }
}
