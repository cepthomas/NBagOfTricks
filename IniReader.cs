using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ephemera.NBagOfTricks;


namespace Ephemera.NBagOfTricks
{
    /// <summary>Reporting user errors.</summary>
    public class IniSyntaxException(string message, int lineNum) : Exception(message)
    {
        public int LineNum { get; init; } = lineNum;
    }

    /// <summary>Contents of one section.</summary>
    public class IniSection
    {
        public string Name { get; set; } = "???";
        public Dictionary<string, string> Values { get; set; } = [];
    }

    /// <summary>Process the ini file.</summary>
    public class IniReader
    {
        /// <summary>What's in the file.</summary>
        readonly List<IniSection> _contents = [];

        /// <summary>All section names</summary>
        /// <returns>Names</returns>
        public List<string> GetSectionNames()
        {
            return [.. _contents.Select(c => c.Name)];
        }

        /// <summary>Get values for the section name.</summary>
        /// <param name="name"></param>
        /// <returns>The section contents or throws if name is invalid.</returns>
        public Dictionary<string, string> GetValues(string name)
        {
            var res = _contents.Where(c => c.Name == name);
            if (res.Any())
            {
                return res.First().Values;
            }
            else
            {
                throw new InvalidOperationException("Invalid section {name}");
            }
        }

        /// <summary>
        /// Process an ini file.
        /// </summary>
        /// <param name="fn"></param>
        public void ParseFile(string fn)
        {
            ParseString(File.ReadAllText(fn));
        }

        /// <summary>
        /// Process an ini string.
        /// </summary>
        /// <param name="s">Input</param>
        public void ParseString(string s)
        {
            IniSection? currentSection = null;
            int lineNum = 0;
            var ls = s.SplitByTokens(Environment.NewLine);

            foreach (var ln in ls)
            {
                lineNum++;

                ///// Clean up line, strip comments.
                var cmt = ln.IndexOf(';');
                var line = cmt >= 0 ? ln[0..cmt] : ln;

                line = line.Trim();

                // Ignore empty lines.
                if (line.Length == 0)
                {
                    continue;
                }

                ///// New section?
                if (line[0] == '[')
                {
                    if (line[^1] == ']')
                    {
                        // New section.
                        if (currentSection is not null) // the first is null
                        {
                            if (currentSection.Values.Count > 0)
                            {
                                // Save last.
                                _contents.Add(currentSection);
                                //currentValues = new();
                            }
                            else
                            {
                                throw new IniSyntaxException($"IniSection {currentSection.Name} has no elements", lineNum);
                            }
                        }

                        var sectionName = line[1..^1];
                        //if (Contents.ContainsKey(sectionName))
                        //{
                        //    throw new IniSyntaxException($"Duplicate section: {inline}", lineNum);
                        //}

                        currentSection = new() { Name = sectionName };
                    }
                    else
                    {
                        throw new IniSyntaxException($"Invalid section: {ln}", lineNum);
                    }
                    continue;
                }

                ///// Just a value.
                if (currentSection is null)
                {
                    throw new IniSyntaxException($"Global values not supported: {ln}", lineNum);
                }

                var parts = line.SplitByToken("="); // TODO support escaped '='
                if (parts.Count != 2)
                {
                    throw new IniSyntaxException($"Invalid value: {ln}", lineNum);
                }

                // Remove any quotes.
                var lhs = parts[0].Replace("\"", "");
                var rhs = parts[1].Replace("\"", "");

                if (currentSection.Values.ContainsKey(lhs))
                {
                    throw new IniSyntaxException($"Duplicate key: {ln}", lineNum);
                }

                currentSection.Values.Add(lhs, rhs);
            }

            // Anything left?
            if (currentSection is not null)
            {
                _contents.Add(currentSection);
            }
        }
    }
}
