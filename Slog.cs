using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Ephemera.NBagOfTricks
{
    #region Types
    /// <summary>Log level options.</summary>
    public enum LogLevel { Trace = 0, Debug = 1, Info = 2, Warn = 3, Error = 4 }

    /// <summary>Internal log entry data container.</summary>
    internal record LogEntry(DateTime Timestamp, LogLevel Level, string LoggerName, string SourceFile, int SourceLine, string Message);

    /// <summary>Log event for notification.</summary>
    public class LogMessageEventArgs : EventArgs
    {
        public LogLevel Level { get; set; } = LogLevel.Info;
        public string Message { get; set; } = "";
        public string ShortMessage { get; set; } = "";
    }
    #endregion

    /// <summary>Global server.</summary>
    public sealed class LogManager
    {
        #region Fields
        /// <summary>Lazy singleton. https://csharpindepth.com/Articles/Singleton.</summary>
        private static readonly Lazy<LogManager> _instance = new(() => new LogManager());

        /// <summary>All loggers. Key is client supplied name.</summary>
        static readonly Dictionary<string, Logger> _loggers = [];

        /// <summary>Log record queue.</summary>
        static readonly ConcurrentQueue<LogEntry> _queue = new();

        /// <summary>For checking file rollover.</summary>
        static DateTime _housekeepTime = DateTime.Now;

        /// <summary>Constant width strings for visual aid.</summary>
        static readonly Dictionary<LogLevel, string> _levelNames = new()
        {
            { LogLevel.Trace, "TRC" },
            { LogLevel.Debug, "DBG" },
            { LogLevel.Info,  "INF" },
            { LogLevel.Warn,  "WRN" },
            { LogLevel.Error, "ERR" }
        };

        /// <summary>Queue management.</summary>
        static bool _running = false;

        /// <summary>Queue management.</summary>
        static CancellationTokenSource _tokenSource = new();
        #endregion

        #region Properties
        /// <summary>Singleton accessor.</summary>
        public static LogManager Instance { get { return _instance.Value; } }

        /// <summary>Event filter for file.</summary>
        public static LogLevel MinLevelFile { get; set; } = LogLevel.Debug;

        /// <summary>Event filter for callback event.</summary>
        public static LogLevel MinLevelNotif { get; set; } = LogLevel.Info;

        /// <summary>Format for file records.</summary>
        public static string TimeFormat { get; set; } = "yyyy'-'MM'-'dd HH':'mm':'ss.fff";

        /// <summary>For diagnostics.</summary>
        public static int QueueSize { get { return _queue.Count; } }
        #endregion

        #region Events
        /// <summary>Callback event.</summary>
        public static event EventHandler<LogMessageEventArgs>? LogMessage;
        #endregion

        #region Public functions
        /// <summary>
        /// Create a new client side logger.
        /// </summary>
        /// <param name="name">Logger ID. Can be shared.</param>
        /// <returns>Minty fresh logger.</returns>
        public static Logger CreateLogger(string name)
        {
            if (!_loggers.TryGetValue(name, out Logger? value))
            {
                Logger logger = new(name);
                value = logger;
                _loggers[name] = value;
            }
            return value;
        }

        /// <summary>
        /// Inititialize builtin file logger.
        /// </summary>
        /// <param name="logFilePath">Builtin logger file path.</param>
        /// <param name="logSize">Builtin logger max size. 0 means no file logger, just notifications.</param>
        public static void Run(string logFilePath = "", int logSize = 0)
        {
            if (logFilePath != "" && logSize > 0)
            {
                // Check for validity by trying to open it.
                try
                {
                    using var writer = new StreamWriter(logFilePath, true);
                }
                catch (IOException)
                {
                    throw new ArgumentException($"Invalid log file name: {logFilePath}");
                }
            }

            _running = true;

            _tokenSource = new(); // reset
            var token = _tokenSource.Token;

            Task task = Task.Run(async () =>
            {
                while (_running)
                {
                    if (token.IsCancellationRequested)
                    {
                        _running = false;
                    }

                    try
                    {
                        if (!_queue.IsEmpty)
                        {
                            StreamWriter? writer = null;

                            if (logFilePath != "" && logSize > 0)
                            {
                                writer = new StreamWriter(logFilePath, true);
                            }

                            while (_queue.TryDequeue(out LogEntry? le))
                            {
                                var fn = Path.GetFileName(le.SourceFile);
                                var slevel = _levelNames[le.Level];

                                if (writer is not null && le.Level >= MinLevelFile)
                                {
                                    string s = $"{DateTime.Now.ToString(TimeFormat)} {slevel} {le.LoggerName} {fn}({le.SourceLine}) {le.Message}";
                                    writer.WriteLine(s);
                                    writer.Flush();
                                }

                                if (LogMessage is not null && le.Level >= MinLevelNotif)
                                {
                                    string smsg = $"{slevel} {le.Message}";
                                    string msg = $"{slevel} {le.LoggerName} {fn}({le.SourceLine}) {le.Message}";
                                    LogMessage.Invoke(null, new LogMessageEventArgs() { Level = le.Level, Message = msg, ShortMessage = smsg });
                                }
                            }

                            writer?.Close();
                            writer?.Dispose();
                        }

                        // Check file size every minute or so.
                        if ((DateTime.Now - _housekeepTime).TotalSeconds > 70)
                        {
                            _housekeepTime = DateTime.Now;
                            if(logFilePath != "")
                            {
                                FileInfo fi = new(logFilePath);
                                if (fi.Exists && fi.Length > logSize)
                                {
                                    // Assemble the backup file name.
                                    var sext = Path.GetExtension(fi.FullName);
                                    var snow = DateTime.Now.ToString("yyyy_MM_dd_HH_mm");
                                    var newfn = fi.FullName.Replace(sext, $"_{snow}{sext}");
                                    File.Copy(fi.FullName, newfn);
                                    // Delete the original as it will be created fresh in the next iteration.
                                    File.Delete(fi.FullName);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Do something or just leave it alone?
                        throw;
                    }

                    // Rest a bit.
                    await Task.Delay(50);
                }
            }, token);
        }

        /// <summary>
        /// Flush queue.
        /// </summary>
        public static void Flush()
        {
            while (!_queue.IsEmpty)
            {
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Be polite, shut it down.
        /// </summary>
        public static void Stop()
        {
            Flush();
            _tokenSource.Cancel();
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Client logger wants to log something.
        /// </summary>
        /// <param name="le"></param>
        internal static void LogThis(LogEntry le)
        {
            if(_running) // don't fill a dead queue.
            {
                _queue.Enqueue(le);
            }
        }
        #endregion
    }

    /// <summary>Client creates as many of these as needed.</summary>
    /// <remarks>
    /// Constructor.
    /// </remarks>
    /// <param name="name">Client assigned name.</param>
    public class Logger(string name)
    {
        #region Properties
        /// <summary>ID for this logger.</summary>
        public string Name { get; init; } = name;

        /// <summary>Turn logger on or off.</summary>
        public bool Enable { get; set; } = true;

        #endregion

        #region Public functions
        #endregion

        #region Convenience functions
        public void Trace(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            AddEntry(LogLevel.Trace, msg, file, line);
        }

        public void Debug(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            AddEntry(LogLevel.Debug, msg, file, line);
        }

        public void Info(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            AddEntry(LogLevel.Info, msg, file, line);
        }

        public void Warn(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            AddEntry(LogLevel.Warn, msg, file, line);
        }

        public void Error(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            AddEntry(LogLevel.Error, msg, file, line);
        }

        public void Exception(Exception ex, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            AddEntry(LogLevel.Error, $"{ex.Message}", file, line);
            AddEntry(LogLevel.Error, $"{new StackTrace(ex, true)}", file, line);
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Private common function.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        void AddEntry(LogLevel level, string msg, string file, int line)
        {
            if (Enable || level == LogLevel.Error)
            {
                // Sanity check for script clients.
                level = (LogLevel)MathUtils.Constrain((int)level, (int)LogLevel.Trace, (int)LogLevel.Error);

                file = Path.GetFileName(file);
                LogEntry le = new(DateTime.Now, level, Name, file, line, msg);
                // Manager expedites.
                LogManager.LogThis(le);
            }
        }
        #endregion
    }
}