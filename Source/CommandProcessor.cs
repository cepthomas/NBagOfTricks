using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;


namespace NBagOfTricks.CommandProcessor
{
    public enum Arg { Req, Opt }

    public enum Param { Req, Opt, None }

    /// <summary>Main processor.</summary>
    public class Processor
    {
        #region Properties
        /// <summary>Designate the set of values that are used to denote the start of an argument.</summary>
        public static char[] ArgumentPrefixList { get; set; } = new char[] { '-', '/' };

        /// <summary>All the commands.</summary>
        public List<Command> Commands { get; set; } = new List<Command>();

        /// <summary>Missing args etc.</summary>
        public List<string> Errors { get; private set; } = new List<string>();
        #endregion

        /// <summary>Parse the cmd string against our definitions.</summary>
        public bool Parse(string args)
        {
            Errors.Clear();

            List<string> parts = args.SplitByToken(" ");

            if (parts.Count > 0)
            {
                // Find the cmd in our list.
                var vcmd = from c in Commands
                           where c.Name.SplitByToken(" ").Contains(parts[0])
                           select c;

                if (vcmd.Any())
                {
                    Command cmd = vcmd.First();
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
                Errors.Add($"Empty command");
            }

            return Errors.Count == 0;
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
                           where c.Name.SplitByToken(" ").Contains(scmd)
                           select c;

                if (vcmd.Any())
                {
                    List<string> argInfo = new List<string>();

                    Command cmd = vcmd.First();

                    sb.Append($"Usage: {cmd.Name.Replace(" ", " | ")}");

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

                    sb.Append($" {cmd.Tail}");
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
                Commands.ForEach(c => sb.AppendLine($"  {c.Name.Replace(" ", " | ")}: {c.Description}"));
            }

            return sb.ToString();
        }
    }

    /// <summary>One possible command.</summary>
    public class Command
    {
        #region Properties - filled in by client
        /// <summary>The command name. Spaces separate aliases.</summary>
        public string Name { get; set; } = "???";

        /// <summary>For usage.</summary>
        public string Description { get; set; } = "???";

        /// <summary>For usage. Optional</summary>
        public string Tail { get; set; } = "";

        /// <summary>Possible arguments for this command.</summary>
        public Arguments Args { get; set; } = null;

        /// <summary>Handler for stuff at the end.</summary>
        public Func<string, bool> TailFunc { get; set; } = null;
        #endregion

        #region Properties - filled in by Parser
        /// <summary>Missing args etc.</summary>
        public List<string> Errors { get; private set; } = new List<string>();
        #endregion

        /// <summary>Parse the argument collection.</summary>
        public void Parse(List<string> args)
        {
            Argument currentArg = null;

            for (int i = 0; i < args.Count; i++)
            {
                string sarg = args[i].Trim();

                if (Processor.ArgumentPrefixList.Contains(sarg[0])) ///// New argument.
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
                    string argName = sarg.Substring(1, sarg.Length - 1);

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
                            if (TailFunc?.Invoke(sarg) == false)
                            {
                                Errors.Add($"Problem with tail:{Name}");
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
                    if (TailFunc?.Invoke(sarg) == false)
                    {
                        Errors.Add($"Problem with tail:{Name}");
                    }
                }
            }

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
        public string Name { get; set; } = "???";

        /// <summary>For usage.</summary>
        public string Description { get; set; } = "???";

        /// <summary>Argument requirement.</summary>
        public Arg ArgReq { get; set; } = Arg.Opt;

        /// <summary>Parameter requirement.</summary>
        public Param ParamReq { get; set; } = Param.None;

        /// <summary>How to process the arg. Can include validation.</summary>
        public Func<string, bool> ArgFunc { get; set; } = null;
        #endregion

        #region Properties - filled in by Parser
        /// <summary>Does it appear in the command line.</summary>
        public bool Valid { get; set; } = false;
        #endregion
    }

    /// <summary>Specialized container. Has Add() to support initialization.</summary>
    public class Commands : List<Command>
    {
        public void Add(string name, string desc, Arguments args = null)
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