using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using NBagOfTricks.PNUT;
using NBagOfTricks.CommandProcessor;


namespace NBagOfTricks.Test
{
    public class CMD_HAPPY : TestSuite
    {
        ////////////////////////////////////////////////////////////////////////
        public override void RunSuite()
        {
            UT_INFO("Test the happy path for the Command Processor.");

            // Captured values
            List<string> txtFiles = new List<string>();
            List<string> docFiles = new List<string>();
            bool abcFlag = false;
            string abcObj = "";    
            bool defFlag = false;
            bool ghiFlag = false;
            bool jklFlag = false;
            string jklObj = "";
            string doodaFlag = "";
            string doodaParam = "";

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
                Name = new string[]{ "realcmd", "go", "r" },
                Description = "Just a command that does amazing things with some arguments",
                Tail = "Files to process...",
                Args = new Arguments
                {
                    { "abc", "arg with optional param",     Arg.Opt, Param.Opt,  (v) => { abcFlag = true; abcObj = v; return true; } },
                    { "def", "arg no param",                Arg.Req, Param.None, (v) => { defFlag = true; return true; } },
                    { "ghi", "arg without optional param",  Arg.Opt, Param.Opt,  (v) => { ghiFlag = true; return true; } },
                    { "jkl", "arg with requred parameter",  Arg.Req, Param.Req,  (v) => { jklFlag = true; jklObj = v; return true; } },
                },
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
                Description = "just another command",
                Args = new Arguments
                {
                    { "thing1", "up side",         Arg.Opt, Param.Opt,  (v) => { doodaFlag = "thing1"; doodaParam = v; return true; } },
                    { "thing2", "down side",       Arg.Opt, Param.Opt,  (v) => { doodaFlag = "thing2"; doodaParam = v; return true; } },
                }
            });


            /////// Basic processing ///////
            string testCmd = "realcmd -def -jkl some1 -ghi -abc some2 InputFile1.txt InputFile2.doc InputFile3.doc";
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


            /////// Alias ///////
            testCmd = "d -thing2 shiny";
            UT_EQUAL(cp.Parse(testCmd), "dooda");

            UT_EQUAL(cp.Errors.Count, 0);
            UT_EQUAL(doodaFlag, "thing2");
            UT_EQUAL(doodaParam, "shiny");


            /////// Usage ///////
            //UT_INFO(cp.GetUsage());
            //UT_INFO(cp.GetUsage("?"));
            //UT_INFO(cp.GetUsage("dooda"));
            //UT_INFO(cp.GetUsage("realcmd"));

            UT_EQUAL(cp.GetUsage().Length, 169);
            UT_EQUAL(cp.GetUsage("?").Length, 83);
            UT_EQUAL(cp.GetUsage("dooda").Length, 85);
            UT_EQUAL(cp.GetUsage("realcmd").Length, 199);
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
    //public class CMD_CONS : TestSuite
    //{
    //    Consolette _console = new Consolette();
    //    Processor _cp = new Processor();

    //    // Command parameters.
    //    string _cmdHelp = "";

    //    public override void RunSuite()
    //    {
    //        UT_INFO("Not really a unit test, more of an example.");

    //        _cp.Commands.Add(new Command()
    //        {
    //            Name = new string[] { "help", "h", "?" },
    //            Description = "What do you want to know?",
    //            Tail = "Optional cmd name",
    //            TailFunc = (v) => { _cmdHelp = v; return true; }
    //        });


    //        _cp.Commands.Add(new Command()
    //        {
    //            Name = new string[] { "clear", "cls", "c" },
    //            Description = "Clear screen.",
    //        });

    //        _console.UserCommand += Console_UserCommand;
    //        _console.Run();
    //    }

    //    void Console_UserCommand(object sender, Consolette.UserCommandArgs e)
    //    {
    //        _cmdHelp = "";

    //        List<string> cmdResults = new List<string>();

    //        try
    //        {
    //            string cmd = _cp.Parse(e.Command);

    //            switch (cmd)
    //            {
    //                case "help":
    //                    cmdResults.Add(_cp.GetUsage(_cmdHelp));
    //                    break;

    //                case "cls":
    //                    _console.Clear();
    //                    break;

    //                case "": // something wrong with the args...
    //                    cmdResults.Add("Errors:");
    //                    cmdResults.AddRange(_cp.Errors);
    //                    break;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            cmdResults.Add("Exception:");
    //            cmdResults.Add(ex.Message);
    //            cmdResults.Add(ex.StackTrace);
    //        }

    //        e.Response = string.Join(Environment.NewLine, cmdResults);
    //    }
    //}
}
