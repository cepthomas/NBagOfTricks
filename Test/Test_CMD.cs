using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using NBagOfTricks.PNUT;
using NBagOfTricks.Core;


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
            void ClearCapture()
            {
                txtFiles.Clear();
                docFiles.Clear();
                args.Clear();
            }

            // Create the specs for the command processor.
            var cp = new Processor();

            cp.Commands = new Commands
            {
                {
                    "help h ?",
                    "what do you want to know",
                    new Arguments
                    {
                        { "cmd", "specific command or empty for all commands",  ArgOptType.Opt, ArgOptType.Opt,  (v) => { Console.Write(cp.GetUsage(v)); return true; } },
                    }
                },
                {
                    "\"real main cmd\" go r",
                    "just a command that does amazing things with some arguments",
                    new Arguments
                    {
                        { "abc1", "optional arg with no value",          ArgOptType.Opt, ArgOptType.None, (v) => { args.Add("abc1", v); return true; } },
                        { "def1", "required arg with no value",          ArgOptType.Req, ArgOptType.None, (v) => { args.Add("def1", v); return true; } },
                        { "ghi1", "optional arg with optional value",    ArgOptType.Opt, ArgOptType.Opt,  (v) => { args.Add("ghi1", v); return true; } },
                        { "jkl1", "required arg with value",             ArgOptType.Req, ArgOptType.Req,  (v) => { args.Add("jkl1", v); return true; } }
                    },
                    // Files func
                    (v) =>
                    {
                        if (v.EndsWith(".txt")) { txtFiles.Add(v); return true; }
                        else if (v.EndsWith(".doc")) { docFiles.Add(v); return true; }
                        else { return false; }
                    }
                },
                {
                    "dooda d",
                    "another command",
                    new Arguments
                    {
                        { "darg1", "up side",      ArgOptType.Opt, ArgOptType.Req,   (v) => { args.Add("darg1", v);  return true; } },
                        { "darg2", "down side",    ArgOptType.Req, ArgOptType.Opt,  (v) => { args.Add("darg2", ""); return true; } },
                    }
                }
            };

            /////// Basic processing ///////
            ClearCapture();
            string testCmd = "\"real main cmd\" -def1 -jkl1 \"thing one\" -ghi1 purple -abc1 InputFile1.txt InputFile2.doc InputFile3.doc";
            UT_TRUE(cp.Parse(testCmd));
            UT_EQUAL(cp.CommandName, "real main cmd");
            cp.Errors.ForEach(err => UT_INFO(err));

            UT_EQUAL(txtFiles.Count, 1);
            UT_EQUAL(txtFiles[0], "InputFile1.txt");

            UT_EQUAL(docFiles.Count, 2);
            UT_EQUAL(docFiles[0], "InputFile2.doc");
            UT_EQUAL(docFiles[1], "InputFile3.doc");

            UT_EQUAL(args.Count, 4);
            UT_TRUE(args.ContainsKey("abc1"));
            UT_EQUAL(args["abc1"], "");
            UT_TRUE(args.ContainsKey("def1"));
            UT_EQUAL(args["def1"], "");
            UT_TRUE(args.ContainsKey("ghi1"));
            UT_EQUAL(args["ghi1"], "purple");
            UT_TRUE(args.ContainsKey("jkl1"));
            UT_EQUAL(args["jkl1"], "thing one");

            /////// Alias ///////
            ClearCapture();
            testCmd = "d -darg1 \"shiny monster\" -darg2";
            UT_TRUE(cp.Parse(testCmd));
            UT_EQUAL(cp.CommandName, "dooda");

            UT_EQUAL(args.Count, 2);
            UT_TRUE(args.ContainsKey("darg1"));
            UT_EQUAL(args["darg1"], "shiny monster");
            UT_TRUE(args.ContainsKey("darg2"));
            UT_EQUAL(args["darg2"], "");

            /////// Usage ///////
            UT_EQUAL(cp.GetUsage().Length, 158);
            UT_EQUAL(cp.GetUsage("?").Length, 79);
            UT_EQUAL(cp.GetUsage("dooda").Length, 78);
            UT_EQUAL(cp.GetUsage("realcmd").Length, 158);

            //UT_INFO("==========================================");
            //UT_INFO(cp.GetUsage());
            //UT_INFO("==========================================");
            //UT_INFO(cp.GetUsage("?"));
            //UT_INFO("==========================================");
            //UT_INFO(cp.GetUsage("dooda"));
            //UT_INFO("==========================================");
            //UT_INFO(cp.GetUsage("real main cmd"));
            //UT_INFO("==========================================");

            UT_EQUAL(cp.GetUsage().Substring(0, 80).Replace(Environment.NewLine, "#"),
                "Commands:#  help|h|?: what do you want to know#  real main cmd|go|r: just a co");
            UT_EQUAL(cp.GetUsage("?").Substring(0, 70).Replace(Environment.NewLine, "#"),
                "Usage: help|h|? [-cmd [val]]#  cmd: specific command or empty for all");
            UT_EQUAL(cp.GetUsage("dooda").Substring(0, 70).Replace(Environment.NewLine, "#"),
                "Usage: dooda|d [-darg1 val] -darg2 [val]#  darg1: up side#  darg2: d");
            UT_EQUAL(cp.GetUsage("real main cmd").Substring(0, 80).Replace(Environment.NewLine, "#"),
                "Usage: real main cmd|go|r [-abc1] -def1 [-ghi1 [val]] -jkl1 val file(s)...#  ab");
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

            cp.Commands = new Commands
            {
                {
                    "cmderrors g",
                    "Just a command that does amazing things with some arguments",
                    new Arguments
                    {
                        { "def2", "required arg missing",            ArgOptType.Req, ArgOptType.Opt,  (v) => { return true; } },
                        { "jkl2", "arg with missing required value", ArgOptType.Req, ArgOptType.Req,  (v) => { return true; } },
                        { "abc2", "extraneous value",                ArgOptType.Req, ArgOptType.None, (v) => { return true; } },
                        { "ghi2", "arg with bad validate",           ArgOptType.Req, ArgOptType.Req,  (v) => { return false; } },
                    }
                }
            };

            /////// Basic processing ///////
            string testCmd = "my.exe cmderrors -unexpctedarg -jkl2 -abc2 xtra -ghi2 some1";

            UT_FALSE(cp.Parse(testCmd, true));
            UT_EQUAL(cp.CommandName, "cmderrors");

            UT_EQUAL(cp.Errors.Count, 6);
            //cp.Errors.ForEach(err => UT_INFO(err));
            UT_TRUE(cp.Errors.Contains("Unexpected arg:unexpctedarg")); //y
            UT_TRUE(cp.Errors.Contains("Extraneous value:xtra")); //n
            UT_TRUE(cp.Errors.Contains("Extraneous value:some1")); //n
            UT_TRUE(cp.Errors.Contains("Missing required value for arg:jkl2")); //n
            UT_TRUE(cp.Errors.Contains("Problem with arg:ghi2"));//y
            UT_TRUE(cp.Errors.Contains("Missing arg:def2")); //y

            testCmd = "badcmd -fff";
            cp.Parse(testCmd);

            UT_EQUAL(cp.Errors.Count, 1);
            //cp.Errors.ForEach(err => UT_INFO(err));
            UT_TRUE(cp.Errors.Contains("Invalid command:badcmd"));

            /////// Usage ///////
            UT_EQUAL(cp.GetUsage("ng-cmd").Length, 85);
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
            Dictionary<string, string> args = new Dictionary<string, string>();
            void ClearCapture()
            {
                txtFiles.Clear();
                docFiles.Clear();
                args.Clear();
            }

            // Create the specs for the command processor.
            var cp = new Processor();
            cp.Commands = new Commands
            {
                {
                    "",
                    "No specific command, use default",
                    new Arguments
                    {
                        { "abc3", "optional arg with no value",  ArgOptType.Opt, ArgOptType.None, (v) => { args.Add("abc3", v); return true; } },
                        { "def3", "required arg with no value",  ArgOptType.Req, ArgOptType.None, (v) => { args.Add("def3", v); return true; } },
                        { "ghi3", "optional arg with value",     ArgOptType.Opt, ArgOptType.Opt,  (v) => { args.Add("ghi3", v); return true; } },
                        { "jkl3", "required arg with value",     ArgOptType.Req, ArgOptType.Req,  (v) => { args.Add("jkl3", v); return true; } }
                    },
                    // Files func
                    (v) =>
                    {
                        if (v.EndsWith(".txt")) { txtFiles.Add(v); return true; }
                        else if (v.EndsWith(".doc")) { docFiles.Add(v); return true; }
                        else { return false; }
                    }
                }
            };

            /////// Basic valid processing ///////
            ClearCapture();
            string testCmd = "-def3 -jkl3 some1 -ghi3 some2 -abc3 InputFile1.txt InputFile2.doc InputFile3.doc";
            cp.Parse(testCmd);

            UT_EQUAL(cp.Errors.Count, 0);
            //cp.Errors.ForEach(err => UT_INFO(err));
            UT_EQUAL(cp.CommandName, "");

            UT_EQUAL(txtFiles.Count, 1);
            UT_EQUAL(txtFiles[0], "InputFile1.txt");

            UT_EQUAL(docFiles.Count, 2);
            UT_EQUAL(docFiles[0], "InputFile2.doc");
            UT_EQUAL(docFiles[1], "InputFile3.doc");

            UT_EQUAL(args.Count, 4);
            UT_TRUE(args.ContainsKey("abc3"));
            UT_EQUAL(args["abc3"], "");
            UT_TRUE(args.ContainsKey("def3"));
            UT_EQUAL(args["def3"], "");
            UT_TRUE(args.ContainsKey("ghi3"));
            UT_EQUAL(args["ghi3"], "some2");
            UT_TRUE(args.ContainsKey("jkl3"));
            UT_EQUAL(args["jkl3"], "some1");
        }
    }
}
