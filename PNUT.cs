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


namespace Ephemera.NBagOfTricks.PNUT
{
    /// <summary>
    /// Specific exception type.
    /// </summary>
    class TestFailException(string file, int line) : Exception()
    {
        public string File { get; } = file;
        public int Line { get; } = line;
    }

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
        public bool StopOnFail { get; set; } = false;

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
        public List<string> OutputLines { get; set; } = [];
        
        /// <summary></summary>
        public List<string> PropertyLines { get; set; } = [];
    }

    /// <summary>
    /// The orchestrator of the test execution.
    /// </summary>
    public class TestRunner
    {
        /// <summary>Format string.</summary>
        const string TIME_FORMAT = @"hh\:mm\:ss\.fff";

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
            Dictionary<string, TestSuite> suites = [];

            foreach (Type t in Assembly.GetCallingAssembly().GetTypes())
            {
                if (t.BaseType is not null && t.BaseType.Name.Contains("TestSuite"))
                {
                    // It's a test suite. Is it requested?
                    foreach (string ssuite in which)
                    {
                        if (t.Name.StartsWith(ssuite))
                        {
                            object? o = Activator.CreateInstance(t);
                            if (o is not null)
                            {
                                suites.Add(t.Name, (TestSuite)o);
                            }
                            else
                            {
                                Context.OutputLines.Add($"Couldn't create {t.BaseType.Name}");
                            }
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
                catch (TestFailException)
                {
                    // Stop on fail set.
                    tc.RecordVerbatim($"^^^^^^^^^^^^^^^^ Stop on fail");
                }
                catch (Exception ex)
                {
                    tc.RecordVerbatim($"^^^^^^^^^^^^^^^^ Unexpected exception: {ex.Message}");
                    tc.RecordVerbatim($"{ ex.StackTrace}");
                }

                // Completed the suite, update the counts.
                Context.NumSuitesRun++;
                Context.NumCasesRun += tc.CaseCnt;
                Context.NumCasesFailed += tc.CaseFailCnt;

                switch (Context.Format)
                {
                    case OutputFormat.Xml:
                        // Any properties?
                        if (Context.PropertyLines.Count > 0)
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
            List<string> preamble = [];

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
        #region Internal Properties
        /// <summary>Accumulated count.</summary>
        public int CaseCnt { get; set; } = 0;

        /// <summary>Accumulated count.</summary>
        public int CaseFailCnt { get; set; } = 0;

        /// <summary>Common context info.</summary>
        public TestContext Context { get; set; } = new();
        #endregion

        #region Internal Functions
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

                if (Context.StopOnFail)
                {
                    throw new TestFailException(file, line);
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
        #endregion

        #region Test Suite Callable Functions
        /// <summary>
        /// Toggle the fatal bail out mechanism.
        /// </summary>
        /// <param name="value">True/false</param>
        public void StopOnFail(bool value)
        {
            Context.StopOnFail = value;
        }

        /// <summary>
        /// Print some info to the report.
        /// </summary>
        /// <param name="message">Info text</param>
        protected void Info(string message)
        {
            if (Context.Format == OutputFormat.Readable)
            {
                RecordVerbatim($"> {message}");
            }
        }

        /// <summary>
        /// Add an element to the property collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void Property<T>(string name, T value)
        {
            RecordProperty(name, value is null ? "null" : value.ToString()!);
        }

        /// <summary>
        /// General purpose assert. Step fails if condition is false.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        /// <param name="expr"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool Assert(
            bool condition,
            string? message = null,
            [CallerArgumentExpression(nameof(condition))] string expr = "???",
            [CallerFilePath] string file = "???",
            [CallerLineNumber] int line = -1)
        {
            RecordResult(condition, $"[{expr}] [{message}]", file, line);
            return condition;
        }

        /// <summary>
        /// Does the Action code throw?
        /// </summary>
        /// <param name="exType"></param>
        /// <param name="act"></param>
        /// <param name="message"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected bool Throws(
            Type exType,
            Action act,
            string? message = null,
            [CallerFilePath] string file = "???",
            [CallerLineNumber] int line = -1)
        {
            bool pass = true;
            bool didThrow = false;

            try { act.Invoke(); }
            catch (Exception ex) { didThrow = ex.GetType() == exType; }

            if (!didThrow)
            {
                RecordResult(false, $"[Did not throw {exType}] [{message}]", file, line);
                pass = false;
            }
            else
            {
                RecordResult(true, $"", file, line);
            }
            return pass;
        }

        /// <summary>
        /// Does the Action code not throw?
        /// </summary>
        /// <param name="act"></param>
        /// <param name="message"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected bool ThrowsNot(
            Action act,
            string? message = null,
            [CallerFilePath] string file = "???",
            [CallerLineNumber] int line = -1)
        {
            bool pass = true;
            //bool throws = false;
            Exception? exType = null;

            try { act.Invoke(); }
            catch (Exception ex) { exType = ex; }

            if (exType is not null)
            {
                RecordResult(false, $"[Did throw {exType}] [{message}]", file, line);
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
