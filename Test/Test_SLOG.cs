using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.PNUT;


namespace NBagOfTricks.Test
{
    class Defs
    {
        public static string SLOG_FILE = Path.Combine(MiscUtils.GetSourcePath(), "out", "slog_log.txt");
    }

    public class SLOG_BASIC : TestSuite
    {
        readonly Logger _logger1 = LogManager.CreateLogger("TestLogger1");
        readonly Logger _logger2 = LogManager.CreateLogger("TestLogger2");
        readonly List<string> _cbText = new();

        public override void RunSuite()
        {
            UT_INFO("Tests slog.");

            _cbText.Clear();
            File.Delete(Defs.SLOG_FILE);
            LogManager.MinLevelFile = LogLevel.Debug;
            LogManager.MinLevelNotif = LogLevel.Info;
            LogManager.LogMessage += LogManager_LogMessage;
            LogManager.Run(Defs.SLOG_FILE, 1000);

            _logger1.Info("11111 file:Y cb:Y");
            _logger2.Debug("22222 file:Y cb:N");
            LogManager.Flush();
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
            LogManager.Flush();
            LogManager.MinLevelNotif = LogLevel.Trace;
            _logger1.Trace("55555 file:N cb:Y");

            //////////
            LogManager.Flush();
            LogManager.Stop();

            ////////// Look at what we have.
            UT_EQUAL(_cbText.Count, 3);
            UT_TRUE(_cbText[0].Contains("11111"));
            UT_TRUE(_cbText[1].Contains("44444"));
            UT_TRUE(_cbText[2].Contains("55555"));

            var ftext = File.ReadAllLines(Defs.SLOG_FILE);
            UT_EQUAL(ftext.Length, 3);
            UT_TRUE(ftext[0].Contains("11111"));
            UT_TRUE(ftext[1].Contains("22222"));
            UT_TRUE(ftext[2].Contains("44444"));
        }

        void LogManager_LogMessage(object? sender, LogMessageEventArgs e)
        {
            _cbText.Add(e.Message);
        }
    }

    public class SLOG_EXC : TestSuite
    {
        readonly Logger _logger8 = LogManager.CreateLogger("TestLogger8");

        public override void RunSuite()
        {
            UT_INFO("Tests exception.");

            LogManager.MinLevelFile = LogLevel.Debug;
            LogManager.MinLevelNotif = LogLevel.Debug;
            LogManager.Run(Defs.SLOG_FILE, 1000);

            try
            {
                throw new Exception("");
            }
            catch (Exception ex)
            {
                _logger8.Exception(ex, "ABC");
            }
        }
    }

    public class SLOG_FLUSH : TestSuite
    {
        readonly Logger _logger9 = LogManager.CreateLogger("TestLogger9");

        public override void RunSuite()
        {
            UT_INFO("Tests flush.");

            LogManager.MinLevelFile = LogLevel.Debug;
            LogManager.MinLevelNotif = LogLevel.Debug;
            LogManager.Run(Defs.SLOG_FILE, 1000);

            for (int i = 0; i < 50; i++)
            {
                _logger9.Info($"Entry{i}");
                if (i == 25)
                {
                    Debug.WriteLine($"1  {LogManager.QueueSize}");
                    LogManager.Flush();
                    Debug.WriteLine($"2  {LogManager.QueueSize}");
                }
            }
            Debug.WriteLine($"3  {LogManager.QueueSize}");
            LogManager.Flush();
            Debug.WriteLine($"4  {LogManager.QueueSize}");
        }
    }

    public class SLOG_SCOPER : TestSuite
    {
        readonly Logger _loggerS = LogManager.CreateLogger("TestLoggerS");

        public override void RunSuite()
        {
            UT_INFO("Tests scoper.");

            //File.Delete(SLOG_FILE);
            LogManager.MinLevelFile = LogLevel.Trace;
            LogManager.MinLevelNotif = LogLevel.Trace;
            LogManager.Run(Defs.SLOG_FILE, 1000);

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
