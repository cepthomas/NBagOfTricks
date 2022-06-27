using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace NBagOfTricks
{
    /// <summary>
    /// Dumps: objects, dictionaries of objects, or lists of objects.
    /// Output format is modified json.
    /// </summary>
    public class Dumper
    {
        /// <summary>Output writer.</summary>
        readonly TextWriter? _writer = null;

        /// <summary>Output indent.</summary>
        int _indent = 0;

        /// <summary>Output indent size.</summary>
        readonly int _indentSize = 4;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="writer">Output stream</param>
        public Dumper(TextWriter writer)
        {
            _writer = writer;
        }

        /// <summary>
        /// Top level writer.
        /// </summary>
        /// <param name="obj"></param>
        public void Write(object obj)
        {
            switch(obj)
            {
                case Dictionary<string, object> dict:
                    Write(dict);
                    break;

                case List<object> list:
                    Write(list);
                    break;

                default: // simple
                    string s = $"{obj}";
                    WriteIndented(s);
                    break;
            }
        }

        /// <summary>
        /// Write a dictionary of objects.
        /// </summary>
        /// <param name="obj"></param>
        public void Write(Dictionary<string, object> obj)
        {
            WriteIndented("{");
            _indent++;

            foreach (var element in obj)
            {
                switch (element.Value)
                {
                    case Dictionary<string, object> _:
                        string s = $"{element.Key} : ";
                        WriteIndented(s);
                        Write(element.Value);
                        break;

                    case List<object> _:
                        Write(element.Value);
                        break;

                    default: // simple
                        string ss = $"{element.Key} : {element.Value}";
                        WriteIndented(ss);
                        break;
                }
            }

            _indent--;
            WriteIndented("}");
        }

        /// <summary>
        /// Write a list of objects.
        /// </summary>
        /// <param name="obj"></param>
        public void Write(List<object> obj)
        {
            WriteIndented("[");
            _indent++;

            foreach (var element in obj)
            {
                switch (element)
                {
                    case Dictionary<string, object> _:
                        Write(element);
                        break;

                    case List<object> _:
                        Write(element);
                        break;

                    default: // simple
                        string s = $"{element}";
                        WriteIndented(s);
                        break;
                }
            }

            _indent--;
            WriteIndented("]");
        }

        /// <summary>
        /// Common output formatter.
        /// </summary>
        /// <param name="s"></param>
        void WriteIndented(string s)
        {
            string sindent = new(' ', _indent * _indentSize);
            _writer!.WriteLine($"{sindent}{s}");
        }
    }
}
