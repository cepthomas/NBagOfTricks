using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NBagOfTricks.Utils;


//TODO argval optional? like help?
///// <summary>Argument Options.</summary>
//public enum ArgReq { Req, Opt }

///// <summary>Parameter Options.</summary>
//public enum ValReq { Req, Opt, None }


namespace NBagOfTricks.CommandProcessor
{
    /// <summary>Main processor.</summary>
    public class Processor
    {
        #region Properties
        /// <summary>Denotes the start of an argument.</summary>
        public static string ArgumentPrefix { get; set; } = "-";

        /// <summary>Selected command.</summary>
        public string CommandName { get; set; } = "";

        /// <summary>All the commands.</summary>
        public List<Command> Commands { get; private set; } = new List<Command>();

        /// <summary>Missing args etc.</summary>
        public List<string> Errors { get; private set; } = new List<string>();
        #endregion

        /// <summary>
        /// Parse the cmd string using our definitions.
        /// </summary>
        /// <param name="cmdString">String to parse.</param>
        /// <returns>The main command name or empty if failed.</returns>
        public bool Parse(string cmdString)
        {
            Errors.Clear();
            Commands.ForEach(c => c.Errors.Clear());

            List<string> parts = cmdString.SplitQuotedString();

            if (parts.Count > 0)
            {
                // Find the cmd in our list.
                var vcmd = from c in Commands
                           where c.Name == null || c.Name.Contains(parts[0])
                           select c;

                if (vcmd.Any())
                {
                    Command cmd = vcmd.First();
                    CommandName = cmd.Name == null ? "" : cmd.Name[0];
                    parts.RemoveAt(0);
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
                           where c.Name.Contains(scmd)
                           select c;

                if (vcmd.Any())
                {
                    List<string> argInfo = new List<string>();

                    Command cmd = vcmd.First();

                    sb.Append($"Usage: {string.Join(" | ", cmd.Name)}");

                    foreach (var a in cmd.Args)
                    {
                        if (a.ArgReq && a.ArgValReq)
                        {
                            sb.Append($" -{a.Name} val");
                        }
                        else if (a.ArgReq && !a.ArgValReq)
                        {
                            sb.Append($" -{a.Name}");
                        }
                        else if (!a.ArgReq && a.ArgValReq)
                        {
                            sb.Append($" [-{a.Name} val]");
                        }
                        else if (!a.ArgReq && !a.ArgValReq)
                        {
                            sb.Append($" [-{a.Name}]");
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
                // Show all commands.
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
        /// If it's empty or null, there is no separate command name.
        /// </summary>
        public string[] Name { get; set; } = null;

        /// <summary>For usage.</summary>
        public string Description { get; set; } = null;

        /// <summary>Possible arguments for this command.</summary>
        public Arguments Args { get; set; } = new Arguments();

        /// <summary>Handler for processing stuff at the end, typically file names.</summary>
        public Func<string, bool> TailFunc { get; set; } = null;

        /// <summary>For usage. Optional.</summary>
        public string TailInfo { get; set; } = "";
        #endregion

        #region Properties - filled in by Parser
        /// <summary>Missing args etc.</summary>
        public List<string> Errors { get; internal set; } = new List<string>();
        #endregion

        /// <summary>Parse the argument collection.</summary>
        public void Parse(List<string> args)
        {
            string sarg = "";

            for (int i = 0; i < args.Count; i++)
            {
                Argument currentArg = null;
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

                    if(currentArg != null)
                    {
                        if (currentArg.ArgValReq)
                        {
                            i++; // look ahead

                            if (i < args.Count)
                            {
                                string val = args[i];

                                if(!val.StartsWith(Processor.ArgumentPrefix))
                                {
                                    // Execute any requested func.
                                    if (currentArg.ArgFunc != null)
                                    {
                                        if (currentArg.ArgFunc.Invoke(val) == false)
                                        {
                                            Errors.Add($"Problem with arg:{currentArg.Name}");
                                            currentArg = null;
                                        }
                                    }
                                    else
                                    {
                                        Errors.Add($"Missing func for arg:{currentArg.Name}");
                                        currentArg = null;
                                    }
                                }
                                else
                                {
                                    Errors.Add($"Missing value for arg:{currentArg.Name}");
                                    currentArg = null;
                                }
                            }
                            else
                            {
                                Errors.Add($"Missing value for arg:{currentArg.Name}");
                                currentArg = null;
                            }
                        }
                        else
                        {
                            // Execute any requested func with no val.
                            if (currentArg.ArgFunc?.Invoke("") == false)
                            {
                                Errors.Add($"Problem with arg:{currentArg.Name}");
                                currentArg = null;
                            }
                        }
                    }
                }
                else // it's a file or other thing
                {
                    if (TailFunc != null)
                    {
                        if (TailFunc.Invoke(sarg) == false)
                        {
                            Errors.Add($"Problem with tail:{sarg}");
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
                if (ca.ArgReq && !ca.Valid)
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
        public bool ArgReq { get; set; } = false;

        /// <summary>Value requirement.</summary>
        public bool ArgValReq { get; set; } = false;

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
        public void Add(string name, string desc, bool areq, bool avreq, Func<string, bool> func = null)
        {
            Add(new Argument()
            {
                Name = name,
                Description = desc,
                ArgReq = areq,
                ArgValReq = avreq,
                ArgFunc = func
            });
        }
    }
}