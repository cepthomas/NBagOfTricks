using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NBagOfTricks.Utils;


namespace NBagOfTricks.CommandProcessor
{
    /// <summary>Argument Options.</summary>
    public enum Arg { Req, Opt }

    /// <summary>Parameter Options.</summary>
    public enum Param { Req, Opt, None }

    /// <summary>Main processor.</summary>
    public class Processor
    {
        #region Properties
        /// <summary>Denotes the start of an argument.</summary>
        public static string ArgumentPrefix { get; set; } = "-";

        /// <summary>All the commands.</summary>
        public List<Command> Commands { get; set; } = new List<Command>();

        /// <summary>Missing args etc.</summary>
        public List<string> Errors { get; private set; } = new List<string>();
        #endregion

        /// <summary>
        /// Parse the cmd string using our definitions.
        /// </summary>
        /// <param name="cmdString">String to parse.</param>
        /// <returns>The main command name or empty if failed.</returns>
        public string Parse(string cmdString)
        {
            Errors.Clear();
            Commands.ForEach(c => c.Errors.Clear());
            string cmdname = "";

            List<string> parts = cmdString.SplitQuotedString();

            if (parts.Count > 0)
            {
                // Find the cmd in our list.
                var vcmd = from c in Commands
                           where c.Name.Contains(parts[0])
                           select c;

                if (vcmd.Any())
                {
                    Command cmd = vcmd.First();
                    cmdname = cmd.Name[0];
                    parts.RemoveAt(0); // strip cmd name
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
                //Errors.Add($"Empty command");
                cmdname = "";
            }

            return Errors.Count == 0 ? cmdname : "";
        }

        /// <summary>Format the usage help text.</summary>
        /// <param name="scmd">Specific command or list of commands if empty.</param>
        /// <returns></returns>
        public string GetUsage(string scmd = "")
        {
            StringBuilder sb = new StringBuilder();

            if(scmd != "")
            {
                // Find the cmd in our list.
                var vcmd = from c in Commands
                           where c.Name.Contains(scmd)
                           select c;

                if (vcmd.Any())
                {
                    List<string> argInfo = new List<string>();

                    Command cmd = vcmd.First();

                    sb.Append($"Usage: {string.Join(" | ", cmd.Name)}");

                    string pmark = "";

                    foreach (var a in cmd.Args)
                    {
                        switch(a.ArgReq)
                        {
                            case Arg.Opt:
                                switch (a.ParamReq)
                                {
                                    case Param.None: sb.Append($" [-{a.Name}]"); break;
                                    case Param.Opt:  sb.Append($" [-{a.Name} [{pmark}]]"); break;
                                    case Param.Req:  sb.Append($" [-{a.Name} {pmark}]"); break;
                                }
                                break;

                            case Arg.Req:
                                switch (a.ParamReq)
                                {
                                    case Param.None: sb.Append($" -{a.Name}"); break;
                                    case Param.Opt:  sb.Append($" -{a.Name} [{pmark}]"); break;
                                    case Param.Req:  sb.Append($" -{a.Name} {pmark}"); break;
                                }
                                break;
                        }

                        argInfo.Add($"  {a.Name}: {a.Description}");
                    }

                    sb.Append(cmd.TailInfo != "" ? $" {cmd.TailInfo}" : "");
                    sb.AppendLine("");

                    argInfo.ForEach(ai => sb.AppendLine(ai));
                }
                else
                {
                    // NG - drop through.
                    scmd = "";
                }
            }

            if (scmd == "")
            {
                sb.AppendLine("Commands:");
                Commands.ForEach(c => sb.AppendLine($"  {string.Join(" | ", c.Name)}: {c.Description}"));
            }

            return sb.ToString();
        }
    }

    /// <summary>One possible command.</summary>
    public class Command
    {
        #region Properties - filled in by client
        /// <summary>
        /// The command name(s). The first one is the main command name and aliases follow.
        /// If it's null, there is no separate command name.
        /// </summary>
        public string[] Name { get; set; } = null;

        /// <summary>For usage.</summary>
        public string Description { get; set; } = null;

        /// <summary>Possible arguments for this command.</summary>
        public Arguments Args { get; set; } = new Arguments();

        /// <summary>Handler for processing stuff at the end, typically file names.</summary>
        public Func<string, bool> TailFunc { get; set; } = null;

        /// <summary>For usage. Optional</summary>
        public string TailInfo { get; set; } = "";
        #endregion

        #region Properties - filled in by Parser
        /// <summary>Missing args etc.</summary>
        public List<string> Errors { get; internal set; } = new List<string>();
        #endregion

        /// <summary>Parse the argument collection.</summary>
        public void Parse(List<string> args)
        {
            Argument currentArg = null;
            string sarg = "";

            for (int i = 0; i < args.Count; i++)
            {
                sarg = args[i].Trim();

                if (sarg.StartsWith(Processor.ArgumentPrefix)) ///// New argument.
                {
                    // Clean up any in process.
                    if (currentArg != null)
                    {
                        switch (currentArg.ParamReq)
                        {
                            case Param.None:
                            case Param.Opt:
                                // Ok to not have a param.

                                // Execute any requested func.
                                if(currentArg.ArgFunc?.Invoke("") == false)
                                {
                                    Errors.Add($"Problem with arg:{currentArg.Name}");
                                }

                                currentArg = null; // done
                                break;

                            case Param.Req:
                                Errors.Add($"Missing param for arg:{currentArg.Name}");
                                currentArg = null;
                                break;
                        }

                        currentArg = null;
                    }

                    ///// Start new cmd.
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
                }
                else if (currentArg != null) ///// Finish command in process.
                {
                    switch (currentArg.ParamReq)
                    {
                        case Param.Req:
                        case Param.Opt:
                            if (currentArg.ArgFunc?.Invoke(sarg) == false)
                            {
                                Errors.Add($"Problem with arg:{currentArg.Name}");
                            }
                            break;

                        case Param.None: // it's a file or other thing
                            if (TailFunc != null)
                            {
                                if (TailFunc.Invoke(sarg) == false)
                                {
                                    Errors.Add($"Problem with tail:{Name[0]}");
                                }
                            }
                            else
                            {
                                Errors.Add($"Extraneous value:{sarg}");
                            }

                            if (currentArg.ArgFunc?.Invoke("") == false)
                            {
                                Errors.Add($"Problem with arg:{currentArg.Name}");
                            }
                            break;
                    }

                    currentArg = null; // done
                }
                else // it's a file or other thing
                {
                    if (TailFunc != null)
                    {
                        if (TailFunc.Invoke(sarg) == false)
                        {
                            Errors.Add($"Problem with tail:{Name[0]}");
                        }
                    }
                    else
                    {
                        Errors.Add($"Extraneous value:{sarg}");
                    }
                }
            }

            //if (currentArg != null) ///// Cleanup command in process.
            //{
            //    switch (currentArg.ParamReq)
            //    {
            //        case Param.Req:
            //        case Param.Opt:
            //            if (currentArg.ArgFunc?.Invoke(sarg) == false)
            //            {
            //                Errors.Add($"Problem with arg:{currentArg.Name}");
            //            }
            //            break;
            //    }
            //}

            // Look for missing required args.
            foreach (Argument ca in Args)
            {
                if (ca.ArgReq == Arg.Req && !ca.Valid)
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
        public string Name { get; set; } = null;

        /// <summary>For usage.</summary>
        public string Description { get; set; } = null;

        /// <summary>Argument requirement.</summary>
        public Arg ArgReq { get; set; } = Arg.Opt;

        /// <summary>Parameter requirement.</summary>
        public Param ParamReq { get; set; } = Param.None;

        /// <summary>How to process the arg. Can include validation - returns true/false.</summary>
        public Func<string, bool> ArgFunc { get; set; } = null;
        #endregion

        #region Properties - filled in by Parser
        /// <summary>Does it appear in the command line.</summary>
        public bool Valid { get; internal set; } = false;
        #endregion
    }

    /// <summary>Specialized container. Has Add() to support initialization.</summary>
    public class Commands : List<Command>
    {
        public void Add(string[] name, string desc, Arguments args = null)
        {
            Add(new Command()
            {
                Name = name,
                Description = desc,
                Args = args
            });
        }
    }

    /// <summary>Specialized container. Has Add() to support initialization.</summary>
    public class Arguments : List<Argument>
    {
        public void Add(string name, string desc, Arg areq, Param preq, Func<string, bool> func = null)
        {
            Add(new Argument()
            {
                Name = name,
                Description = desc,
                ArgReq = areq,
                ParamReq = preq,
                ArgFunc = func
            });
        }
    }
}