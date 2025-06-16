using System;
using System.Collections.Generic;
using System.IO;
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
        public Dictionary<string, IniSection> Contents { get; } = [];

        /// <summary>Do it.</summary>
        public IniReader(string fn)
        {
            IniSection? currentSection = null;
            int lineNum = 0;

            foreach (var inline in File.ReadAllLines(fn))
            {
                lineNum++;

                ///// Clean up line, strip comments.
                var cmt = inline.IndexOf(';');
                var line = cmt >= 0 ? inline[0..cmt] : inline;

                line = line.Trim();

                // Ignore empty lines.
                if (line.Length == 0)
                {
                    continue;
                }

                ///// New section?
                if (line[0] == '[')
                {
                    if (line[^1 ] == ']')
                    {
                        // New section.
                        if (currentSection is not null) // the first
                        {
                            if (currentSection.Values.Count > 0)
                            {
                                // Save last.
                                Contents[currentSection.Name] = currentSection;
                                //currentValues = new();
                            }
                            else
                            {
                                throw new IniSyntaxException($"IniSection {currentSection.Name} has no elements", lineNum);
                            }
                        }

                        var sectionName = line[1..^1];
                        if (Contents.ContainsKey(sectionName))
                        {
                            throw new IniSyntaxException($"Duplicate section: {inline}", lineNum);
                        }

                        currentSection = new() { Name = sectionName };
                    }
                    else
                    {
                        throw new IniSyntaxException($"Invalid section: {inline}", lineNum);
                    }
                    continue;
                }

                ///// Just a value.
                if (currentSection is null)
                {
                    throw new IniSyntaxException($"Global values not supported: {inline}", lineNum);
                }

                var parts = line.SplitByToken("="); // TODO support escaped '='
                if (parts.Count !=2)
                {
                    throw new IniSyntaxException($"Invalid value: {inline}", lineNum);
                }

                // Remove any quotes.
                var lhs = parts[0].Replace("\"", "");
                var rhs = parts[1].Replace("\"", "");

                if (currentSection.Values.ContainsKey(lhs))
                {
                    throw new IniSyntaxException($"Duplicate key: {inline}", lineNum);
                }

                currentSection.Values.Add(lhs, rhs);
            }

            // Anything left?
            if (currentSection is not null)
            {
                Contents[currentSection.Name] = currentSection;
            }
        }
    }
}
