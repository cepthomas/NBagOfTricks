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
            void ClearCapture()
            {
                txtFiles.Clear();
                docFiles.Clear();
                args.Clear();
            }

            // Create the specs for the command processor.
            var cp = new Processor();

            ///////// Happy path //////////
            cp.Commands = new Commands
            {
                { "help h ?", "what do you want to know", new Arguments
                    {
                        { "cmd", "specific command or empty for all commands",  false, false,  (v) => { Console.Write(cp.GetUsage(v)); return true; } },
                    }
                },


            };

            cp.Commands.Add(new Command()
            {
                Name = "help h ?",
                Description = "what do you want to know",    
                Args = new Arguments
                {
                    { "cmd", "specific command or empty for all commands",  false, false,  (v) => { Console.Write(cp.GetUsage(v)); return true; } },
                }
            });

            cp.Commands.Add(new Command()
            {
                Name = "\"real main cmd\" go r",
                Description = "just a command that does amazing things with some arguments",
                Args = new Arguments
                {
                    { "abc", "optional arg with no value",  false, false, (v) => { args.Add("abc", v);  return true; } },
                    { "def", "required arg with no value",  true,  false, (v) => { args.Add("def", v); return true; } },
                    { "ghi", "optional arg with value",     false, true,  (v) => { args.Add("ghi", v); return true; } },
                    { "jkl", "required arg with value",     true,  true,  (v) => { args.Add("jkl", v);  return true; } },
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
                Name = "dooda d",
                Description = "another command",
                Args = new Arguments
                {
                    { "darg1", "up side",      false, true,   (v) => { args.Add("darg1", v);  return true; } },
                    { "darg2", "down side",    false, false,  (v) => { args.Add("darg2", ""); return true; } },
                }
            });

            /////// Basic processing ///////
            ClearCapture();
            string testCmd = "\"real main cmd\" -def -jkl \"thing one\" -ghi purple -abc InputFile1.txt InputFile2.doc InputFile3.doc";
            UT_TRUE(cp.Parse(testCmd));
            UT_EQUAL(cp.CommandName, "real main cmd");
            cp.Errors.ForEach(err => UT_INFO(err));

            UT_EQUAL(txtFiles.Count, 1);
            UT_EQUAL(txtFiles[0], "InputFile1.txt");

            UT_EQUAL(docFiles.Count, 2);
            UT_EQUAL(docFiles[0], "InputFile2.doc");
            UT_EQUAL(docFiles[1], "InputFile3.doc");

            UT_EQUAL(args.Count, 4);
            UT_TRUE(args.ContainsKey("abc"));
            UT_EQUAL(args["abc"], "");
            UT_TRUE(args.ContainsKey("def"));
            UT_EQUAL(args["def"], "");
            UT_TRUE(args.ContainsKey("ghi"));
            UT_EQUAL(args["ghi"], "purple");
            UT_TRUE(args.ContainsKey("jkl"));
            UT_EQUAL(args["jkl"], "thing one");

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
            UT_EQUAL(cp.GetUsage().Length, 170);
            UT_EQUAL(cp.GetUsage("?").Length, 79);
            UT_EQUAL(cp.GetUsage("dooda").Length, 78);
            UT_EQUAL(cp.GetUsage("realcmd").Length, 170);

            UT_EQUAL(cp.GetUsage().Substring(0, 70).Replace(Environment.NewLine, "#"), "Commands:#  help | h | ?: what do you want to know#  real main cmd |");
            UT_EQUAL(cp.GetUsage("?").Substring(0, 70).Replace(Environment.NewLine, "#"), "Usage: help | h | ? [-cmd]#  cmd: specific command or empty for all c");
            UT_EQUAL(cp.GetUsage("dooda").Substring(0, 70).Replace(Environment.NewLine, "#"), "Usage: dooda | d [-darg1 val] [-darg2]#  darg1: up side#  darg2: dow");
            UT_EQUAL(cp.GetUsage("real main cmd").Substring(0, 70).Replace(Environment.NewLine, "#"), "Usage: real main cmd | go | r [-abc] -def [-ghi val] -jkl val Filename");
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
                Name = "cmderrors g",
                Description = "Just a command that does amazing things with some arguments",
                Args = new Arguments
                {
                    { "def", "required arg missing",             true, false,  (v) => { return true; } },
                    { "jkl", "arg with missing required value",  true, true,   (v) => { return true; } },
                    { "abc", "extraneous value",                 true, false,  (v) => { return true; } },
                    { "ghi", "arg with bad validate",            false, false, (v) => { return false; } },
                }
            });


            /////// Basic processing ///////
            string testCmd = "cmderrors -unexpctedarg -abc xtra -jkl -ghi some1";
            UT_FALSE(cp.Parse(testCmd));
            UT_EQUAL(cp.CommandName, "cmderrors");

            UT_EQUAL(cp.Errors.Count, 5);
            //cp.Errors.ForEach(err => UT_INFO(err));
            UT_TRUE(cp.Errors.Contains("Unexpected arg:unexpctedarg"));
            UT_TRUE(cp.Errors.Contains("Extraneous value:xtra"));
            UT_TRUE(cp.Errors.Contains("Extraneous value:some1"));
            UT_TRUE(cp.Errors.Contains("Missing value for arg:jkl"));
            //UT_TRUE(cp.Errors.Contains("Problem with arg:ghi"));//missing
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
            Dictionary<string, string> args = new Dictionary<string, string>();
            void ClearCapture()
            {
                txtFiles.Clear();
                docFiles.Clear();
                args.Clear();
            }

            // Create the specs for the command processor.
            var cp = new Processor();

            cp.Commands.Add(new Command()
            {
                // Name = null,
                Description = "No specific command, use default",
                Args = new Arguments
                {
                    { "abc", "optional arg with no value",      false, false, (v) => { args.Add("abc", v);  return true; } },
                    { "def", "required arg with no value",      true,  false, (v) => { args.Add("def", v); return true; } },
                    { "ghi", "optional arg with req value",     false, true,  (v) => { args.Add("ghi", v); return true; } },
                    { "jkl", "required arg with req value",     true,  true,  (v) => { args.Add("jkl", v);  return true; } },
                },
                TailInfo = "Filename(s) to process",
                TailFunc = (v) =>
                {
                    if (v.EndsWith(".txt")) { txtFiles.Add(v); return true; }
                    else if (v.EndsWith(".doc")) { docFiles.Add(v); return true; }
                    else { return false; }
                }
            });

            /////// Basic valid processing ///////
            ClearCapture();
            string testCmd = "-def -jkl some1 -ghi some2 -abc InputFile1.txt InputFile2.doc InputFile3.doc";
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
            UT_TRUE(args.ContainsKey("abc"));
            UT_EQUAL(args["abc"], "");
            UT_TRUE(args.ContainsKey("def"));
            UT_EQUAL(args["def"], "");
            UT_TRUE(args.ContainsKey("ghi"));
            UT_EQUAL(args["ghi"], "some2");
            UT_TRUE(args.ContainsKey("jkl"));
            UT_EQUAL(args["jkl"], "some1");
        }
    }
}
