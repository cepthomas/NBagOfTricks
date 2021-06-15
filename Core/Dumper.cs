using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace NBagOfTricks.Core
{
    /// <summary>
    /// Dumps: objects, dictionaries of objects, or lists of objects.
    /// Output format is modified json.
    /// </summary>
    public class Dumper
    {
        /// <summary>Output writer.</summary>
        TextWriter _writer = null;

        /// <summary>Output indent.</summary>
        int _indent = 0;

        /// <summary>Output indent size.</summary>
        int _indentSize = 4;

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
                    case Dictionary<string, object> dict:
                        string s = $"{element.Key} : ";
                        WriteIndented(s);
                        Write(element.Value);
                        break;

                    case List<object> list:
                        Write(element.Value);
                        break;

                    default: // simple
                        string ss = $"{element.Key} : {element.Value}";
                        WriteIndented(ss);
                        break;
                }



                //if (element.Value is Dictionary<string, object> || element.Value is List<object>)
                //{
                //    string s = $"{element.Key} : ";
                //    WriteIndented(s);
                //    Write(element.Value);
                //}
                //else // simple
                //{
                //    string s = $"{element.Key} : {element.Value}";
                //    WriteIndented(s);
                //}
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
                    case Dictionary<string, object> dict:
                        Write(element);
                        break;

                    case List<object> list:
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
            string sindent = new string(' ', _indent * _indentSize);
            _writer.WriteLine($"{sindent}{s}");
        }
    }
}
