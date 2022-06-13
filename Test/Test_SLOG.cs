using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using NBagOfTricks;
using NBagOfTricks.Slog;
using NBagOfTricks.PNUT;


namespace NBagOfTricks.Test
{
    public class SLOG_BASIC : TestSuite
    {
        readonly Logger _logger1 = LogManager.CreateLogger("TestLogger1");
        readonly Logger _logger2 = LogManager.CreateLogger("TestLogger2");
        readonly List<string> _cbText = new();
        const string SLOG_FILE = @"..\..\out\slog.log.txt";

        public override void RunSuite()
        {
            UT_INFO("Tests slog.");

            _cbText.Clear();
            File.Delete(SLOG_FILE);
            LogManager.MinLevelFile = LogLevel.Debug;
            LogManager.MinLevelNotif = LogLevel.Info;
            LogManager.LogEvent += LogManager_LogEvent;
            LogManager.Run(SLOG_FILE, 1000);

            _logger1.Info("11111 file:Y cb:Y");
            _logger2.Debug("22222 file:Y cb:N");
            WaitForEmptyQueue();
            _logger1.Trace("33333 file:N cb:N");

            // Force exception.
            try
            {
                int x = 0;
                int y = 100 / x;
            }
            catch (Exception ex)
            {
                _logger2.Exception(ex, "44444 file:Y cb:Y");
            }

            //////////
            WaitForEmptyQueue();
            LogManager.MinLevelNotif = LogLevel.Trace;
            _logger1.Trace("55555 file:N cb:Y");

            //////////
            WaitForEmptyQueue();
            LogManager.Stop();

            ////////// Look at what we have.
            UT_EQUAL(_cbText.Count, 3);
            UT_TRUE(_cbText[0].Contains("11111"));
            UT_TRUE(_cbText[1].Contains("44444"));
            UT_TRUE(_cbText[2].Contains("55555"));

            var ftext = File.ReadAllLines(SLOG_FILE);
            UT_EQUAL(ftext.Length, 4);
            UT_TRUE(ftext[0].Contains("11111"));
            UT_TRUE(ftext[1].Contains("22222"));
            UT_TRUE(ftext[2].Contains("44444"));
        }

        void WaitForEmptyQueue()
        {
            while (LogManager.QueueSize > 0)
            {
                Thread.Sleep(50);
            }
        }

        void LogManager_LogEvent(object? sender, LogEventArgs e)
        {
            _cbText.Add(e.Message);
        }
    }

    public class SLOG_SCOPER : TestSuite
    {
        readonly Logger _loggerS = LogManager.CreateLogger("TestLoggerS");
        const string SLOG_FILE = @"..\..\out\slog.log.txt";

        public override void RunSuite()
        {
            File.Delete(SLOG_FILE);
            LogManager.MinLevelFile = LogLevel.Trace;
            LogManager.MinLevelNotif = LogLevel.Trace;
            LogManager.Run(SLOG_FILE, 1000);

            int i = 0;
            using Scoper s1 = new(_loggerS, "111");
            if (i++ < 100)
            {
                using Scoper s2 = new(_loggerS, "222");
                if (i++ < 100)
                {
                    using Scoper s3 = new(_loggerS, "333");
                }
            }
            if (i++ < 100)
            {
                using Scoper s4 = new(_loggerS, "444");
            }
            using Scoper s5 = new(_loggerS, "555");
            if (i++ < 100)
            {
                using Scoper s6 = new(_loggerS, "666");
            }
        }
    }
}
