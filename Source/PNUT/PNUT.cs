using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Diagnostics;


// This file contains the most recent incarnation of a super simple unit test framework.
// The very original was based on Quicktest (http://www.tylerstreeter.net, http://quicktest.sourceforge.net/).
// Since then is has gone through many iterations and made many users happy. Now here's a .NET version.
// The original license is GNU Lesser General Public License OR BSD-style, which allows unrestricted use of the Quicktest code.


namespace NBagOfTricks.PNUT
{
    /// <summary>
    /// Generate a human readable or junit format output.
    /// </summary>
    public enum OutputFormat { Readable, Xml };

    /// <summary>
    /// Accumulates general test info.
    /// </summary>
    public class TestContext
    {
        /// <summary></summary>
        public OutputFormat Format { get; set; } = OutputFormat.Readable;

        /// <summary></summary>
        public string CurrentSuiteId { get; set; } = "???";

        /// <summary></summary>
        public bool CurrentSuitePass { get; set; } = true;

        /// <summary></summary>
        public int NumSuitesRun { get; set; } = 0;

        /// <summary></summary>
        public int NumSuitesFailed { get; set; } = 0;

        /// <summary></summary>
        public bool CurrentCasePass { get; set; } = true;

        /// <summary></summary>
        public int NumCasesRun { get; set; } = 0;

        /// <summary></summary>
        public int NumCasesFailed { get; set; } = 0;

        /// <summary></summary>
        public List<string> OutputLines { get; set; } = new List<string>();
        
        /// <summary></summary>
        public List<string> PropertyLines { get; set; } = new List<string>();
    }

    /// <summary>
    /// Specific exception type.
    /// </summary>
    class AssertException : Exception
    {
        public string File { get; }
        public int Line { get; }

        public AssertException(string msg, string file, int line) : base(msg)
        {
            File = file;
            Line = line;
        }
    }

    /// <summary>
    /// The orchestrator of the test execution.
    /// </summary>
    public class TestRunner
    {
        /// <summary>Format string.</summary>
        const string TIME_FORMAT = @"hh\:mm\:ss\.fff";

        /// <summary>Format string.</summary>
        const string DATE_TIME_FORMAT = "yyyy'-'MM'-'dd HH':'mm':'ss";

        /// <summary>Format string.</summary>
        const string DATE_TIME_FORMAT_MSEC = "yyyy'-'MM'-'dd HH':'mm':'ss.fff";

        /// <summary>The test context.</summary>
        public TestContext Context { get; } = new TestContext();

        /// <summary>
        /// Normal constructor.
        /// </summary>
        public TestRunner(OutputFormat fmt)
        {
            Context.Format = fmt;
        }

        /// <summary>
        /// Run selected cases.
        /// </summary>
        /// <param name="which">List of names of test cases to run. If the test case names begin with these values they will run.</param>
        public void RunSuites(string[] which)
        {
            // Locate the test cases.
            Dictionary<string, TestSuite> suites = new Dictionary<string, TestSuite>();

            // GetExecutingAssembly()

            foreach (Type t in Assembly.GetCallingAssembly().GetTypes())
            {
                if (t.BaseType != null && t.BaseType.Name.Contains("TestSuite"))
                {
                    // It's a test suite. Is it requested?
                    foreach (string ssuite in which)
                    {
                        if (t.Name.StartsWith(ssuite))
                        {
                            suites.Add(t.Name, Activator.CreateInstance(t) as TestSuite);
                        }
                    }
                }
            }

            DateTime startTime = DateTime.Now;

            // Run through to execute suites.
            foreach (string ss in suites.Keys)
            {
                Context.CurrentSuiteId = ss;
                TestSuite  tc = suites[ss];
                tc.Context = Context;
                Context.CurrentSuitePass = true;
                Context.CurrentCasePass = true;
                Context.PropertyLines.Clear();

                // Document the start of the suite.
                switch(Context.Format)
                {
                    case OutputFormat.Xml:
                        tc.RecordVerbatim($"    <testsuite name = {ss}>");
                        break;

                    case OutputFormat.Readable:
                        tc.RecordVerbatim($"Suite {ss}");
                        break;
                }

                try
                {
                    // Run the suite.
                    tc.RunSuite();
                }
                catch (AssertException ex)
                {
                    // Deliberate exception.
                    tc.RecordResult(false, ex.Message, ex.File, ex.Line);
                }
                catch (Exception ex)
                {
                    // Out of scope exception. Top frame contains the cause.
                    StackTrace st = new StackTrace(ex, true);
                    StackFrame frame = st.GetFrame(0);

                    int line = frame.GetFileLineNumber();
                    string fn = Path.GetFileName(frame.GetFileName());
                    string msg = $"{ex.Message} ({fn}:{line})";

                    tc.RecordResult(false, msg, fn, line);
                }

                // Completed the suite, update the counts.
                Context.NumSuitesRun++;
                Context.NumCasesRun += tc.CaseCnt;
                Context.NumCasesFailed += tc.CaseFailCnt;

                switch (Context.Format)
                {
                    case OutputFormat.Xml:
                        // Any properties?
                        if (Context.PropertyLines.Count() > 0)
                        {
                            tc.RecordVerbatim($"        <properties>");
                            Context.PropertyLines.ForEach(l => tc.RecordVerbatim(l));
                            tc.RecordVerbatim($"        </properties>");
                        }

                        tc.RecordVerbatim($"    </testsuite>");
                        break;

                    case OutputFormat.Readable:
                        Context.OutputLines.Add($"");
                        break;
                }
            }

            // Finished the test run, prepare the summary.
            DateTime endTime = DateTime.Now;
            TimeSpan dur = endTime - startTime;
            string sdur = dur.ToString(TIME_FORMAT);
            List<string> preamble = new List<string>();

            switch (Context.Format)
            {
                case OutputFormat.Xml:
                    preamble.Add($"<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    preamble.Add($"<testsuites tests={Context.NumCasesRun} failures={Context.NumCasesFailed} time={sdur} >");
                    break;

                case OutputFormat.Readable:
                    string pass = Context.NumCasesFailed > 0 ? "Fail" : "Pass";

                    preamble.Add($"#------------------------------------------------------------------");
                    preamble.Add($"# Unit Test Report");
                    preamble.Add($"# Start Time: {startTime.ToString(DATE_TIME_FORMAT_MSEC)}");
                    preamble.Add($"# Duration: {sdur}");
                    //preamble.Add($"# Suites Run: {Context.NumSuitesRun}");
                    //preamble.Add($"# Suites Failed: {Context.NumSuitesFailed}");
                    preamble.Add($"# Cases Run: {Context.NumCasesRun}");
                    preamble.Add($"# Cases Failed: {Context.NumCasesFailed}");
                    preamble.Add($"# Test Result: {pass}");
                    preamble.Add($"#------------------------------------------------------------------");
                    break;
            }

            Context.OutputLines.InsertRange(0, preamble);
            if(Context.Format == OutputFormat.Xml)
            {
                Context.OutputLines.Add($"</testsuites>");
            }

            Context.OutputLines.ForEach(l => Console.WriteLine(l));
        }
    }

    /// <summary>
    /// Defining class for an individual test suite.
    /// </summary>
    public abstract class TestSuite
    {
        #region Properties
        /// <summary>Accumulated count.</summary>
        public int CaseCnt { get; set; } = 0;

        /// <summary>Accumulated count.</summary>
        public int CaseFailCnt { get; set; } = 0;

        /// <summary>Common context info.</summary>
        public TestContext Context { get; set; } = null;
        #endregion

        #region Definitions
        const string UNKNOWN_FILE = "???";
        const int UNKNOWN_LINE = -1;
        #endregion

        /// <summary>
        /// All test case specifications must supply this.
        /// </summary>
        public abstract void RunSuite();

        /// <summary>
        /// Record a test result.
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="message"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        public void RecordResult(bool pass, string message, string file, int line)
        {
            CaseCnt++;

            if (pass)
            {
                switch (Context.Format)
                {
                    case OutputFormat.Xml:
                        Context.OutputLines.Add($"        <testcase name=\"{Context.CurrentSuiteId}.{CaseCnt}\" classname=\"{Context.CurrentSuiteId}\" />");
                        break;

                    case OutputFormat.Readable:
                        break;
                }
            }
            else
            {
                CaseFailCnt++;

                switch (Context.Format)
                {
                    case OutputFormat.Xml:
                        Context.OutputLines.Add($"        <testcase name=\"{Context.CurrentSuiteId}.{CaseCnt}\" classname=\"{Context.CurrentSuiteId}\">");
                        Context.OutputLines.Add($"            <failure message=\"{file}:{line} {message}\"></failure>");
                        Context.OutputLines.Add($"        </testcase>");
                        break;

                    case OutputFormat.Readable:
                        Context.OutputLines.Add($"! ({file}:{line}) {Context.CurrentSuiteId}.{CaseCnt} {message}");
                        break;
                }
            }
        }

        /// <summary>
        /// Record a property into the report.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void RecordProperty(string name, string value)
        {
            switch (Context.Format)
            {
                case OutputFormat.Xml:
                    Context.PropertyLines.Add($"            <property name=\"{name}\" value=\"{value}\" />");
                    break;

                case OutputFormat.Readable:
                    Context.OutputLines.Add($"Property {name}:{value}");
                    break;
            }
        }

        /// <summary>
        /// Record a verbatim text line into the report.
        /// </summary>
        /// <param name="message"></param>
        public void RecordVerbatim(string message)
        {
            Context.OutputLines.Add(message);
        }

        #region Test functions - Boilerplate
        /// <summary>
        /// Print some info to the report.
        /// </summary>
        /// <param name="message">Info text</param>
        /// <param name="vars">Optional vars to print</param>
        protected void UT_INFO(string message, params object[] vars)
        {
            if(Context.Format == OutputFormat.Readable)
            {
                RecordVerbatim($"{message} {string.Join(", ", vars)}");
            }
        }

        /// <summary>
        /// Add an element to the property collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void UT_PROPERTY<T>(string name, T value)
        {
            RecordProperty(name, value.ToString());
        }
        #endregion

        #region Test functions - Basic
        /// <summary>
        /// Checks whether the given condition is true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected bool UT_TRUE(bool condition, [CallerFilePath] string file = UNKNOWN_FILE, [CallerLineNumber] int line = UNKNOWN_LINE)
        {
            bool pass = true;
            if (!condition)
            {
                RecordResult(false, $"condition should be true", file, line);
                pass = false;
            }
            else
            {
                RecordResult(true, $"", file, line);
            }
            return pass;
        }

        /// <summary>
        /// Checks whether the given condition is false.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected bool UT_FALSE(bool condition, [CallerFilePath] string file = UNKNOWN_FILE, [CallerLineNumber] int line = UNKNOWN_LINE)
        {
            bool pass = true;
            if (condition)
            {
                RecordResult(false, $"condition should be false", file, line);
                pass = false;
            }
            else
            {
                RecordResult(true, $"", file, line);
            }
            return pass;
        }

        /// <summary>
        /// Prints the condition and gens assert/exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        protected void UT_ASSERT<T>(T value1, T value2, [CallerFilePath] string file = UNKNOWN_FILE, [CallerLineNumber] int line = UNKNOWN_LINE) where T : IComparable
        {
            if (value1.CompareTo(value2) != 0)
            {
                throw new AssertException($"[{value1}] != [{value2}]", file, line);
            }
        }
        #endregion

        #region Test functions - Comparers
        /// <summary>
        /// Checks whether the first parameter is equal to the second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected bool UT_EQUAL<T>(T value1, T value2, [CallerFilePath] string file = UNKNOWN_FILE, [CallerLineNumber] int line = UNKNOWN_LINE) where T : IComparable
        {
            bool pass = true;
            if (value1.CompareTo(value2) != 0)
            {
                RecordResult(false, $"[{value1}] != [{value2}]", file, line);
                pass = false;
            }
            else
            {
                RecordResult(true, $"", file, line);
            }
            return pass;
        }

        /// <summary>
        /// Checks whether the first parameter is not equal to the second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected bool UT_NOT_EQUAL<T>(T value1, T value2, [CallerFilePath] string file = UNKNOWN_FILE, [CallerLineNumber] int line = UNKNOWN_LINE) where T : IComparable
        {
            bool pass = true;
            if (value1.CompareTo(value2) == 0)
            {
                RecordResult(false, $"[{value1}] == [{value2}]", file, line);
                pass = false;
            }
            else
            {
                RecordResult(true, $"", file, line);
            }
            return pass;
        }

        /// <summary>
        /// Checks whether the first parameter is less than the second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected bool UT_LESS<T>(T value1, T value2, [CallerFilePath] string file = UNKNOWN_FILE, [CallerLineNumber] int line = UNKNOWN_LINE) where T : IComparable
        {
            bool pass = true;
            if (value1.CompareTo(value2) != -1)
            {
                RecordResult(false, $"[{value1}] not less than [{value2}]", file, line);
                pass = false;
            }
            else
            {
                RecordResult(true, $"", file, line);
            }
            return pass;
        }

        /// <summary>
        /// Checks whether the first parameter is less than or equal to the second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected bool UT_LESS_OR_EQUAL<T>(T value1, T value2, [CallerFilePath] string file = UNKNOWN_FILE, [CallerLineNumber] int line = UNKNOWN_LINE) where T : IComparable
        {
            bool pass = true;
            if (value1.CompareTo(value2) == 1)
            {
                RecordResult(false, $"[{value1}] not less than or equal [{value2}]", file, line);
                pass = false;
            }
            else
            {
                RecordResult(true, $"", file, line);
            }
            return pass;
        }

        /// <summary>
        /// Checks whether the first parameter is greater than the second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected bool UT_GREATER<T>(T value1, T value2, [CallerFilePath] string file = UNKNOWN_FILE, [CallerLineNumber] int line = UNKNOWN_LINE) where T : IComparable
        {
            bool pass = true;
            if (value1.CompareTo(value2) != 1)
            {
                RecordResult(false, $"[{value1}] not greater than [{value2}]", file, line);
                pass = false;
            }
            else
            {
                RecordResult(true, $"", file, line);
            }
            return pass;
        }

        /// <summary>
        /// Checks whether the first parameter is greater than or equal to the second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected bool UT_GREATER_OR_EQUAL<T>(T value1, T value2, [CallerFilePath] string file = UNKNOWN_FILE, [CallerLineNumber] int line = UNKNOWN_LINE) where T : IComparable
        {
            bool pass = true;
            if (value1.CompareTo(value2) == -1)
            {
                RecordResult(false, $"[{value1}] not greater than or equal [{value2}]", file, line);
                pass = false;
            }
            else
            {
                RecordResult(true, $"", file, line);
            }
            return pass;
        }

        /// <summary>
        /// Checks whether the first parameter is within the given tolerance from the second parameter.
        /// This is useful for comparing floating point values.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="tolerance"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected bool UT_CLOSE(double value1, double value2, double tolerance, [CallerFilePath] string file = UNKNOWN_FILE, [CallerLineNumber] int line = UNKNOWN_LINE)
        {
            bool pass = true;
            if (Math.Abs(value1 - value2) > tolerance)
            {
                RecordResult(false, $"[{value1}] not close enough to [{value2}]", file, line);
                pass = false;
            }
            else
            {
                RecordResult(true, $"", file, line);
            }
            return pass;
        }
        #endregion
    }
}
