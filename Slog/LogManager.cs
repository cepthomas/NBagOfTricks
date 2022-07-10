using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NBagOfTricks;


namespace NBagOfTricks.Slog
{
    /// <summary>Global server.</summary>
    public sealed class LogManager
    {
        #region Fields
        /// <summary>Lazy singleton. https://csharpindepth.com/Articles/Singleton.</summary>
        private static readonly Lazy<LogManager> _instance = new(() => new LogManager());

        /// <summary>All loggers. Key is client supplied name.</summary>
        static readonly Dictionary<string, Logger> _loggers = new();

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
        public static event EventHandler<LogEventArgs>? LogEvent;
        #endregion

        #region Public functions
        /// <summary>
        /// Create a new client side logger.
        /// </summary>
        /// <param name="name">Logger ID. Can be shared.</param>
        /// <returns>Minty fresh logger.</returns>
        public static Logger CreateLogger(string name)
        {
            if (!_loggers.ContainsKey(name))
            {
                Logger logger = new(name);
                _loggers[name] = logger;
            }
            return _loggers[name];
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

                                if (LogEvent is not null && le.Level >= MinLevelNotif)
                                {
                                    string s = $"{slevel} {le.LoggerName} {fn}({le.SourceLine}) {le.Message}";
                                    LogEvent.Invoke(null, new LogEventArgs() { Level = le.Level, Message = s });
                                }
                            }

                            writer?.Close();
                            writer?.Dispose();
                        }

                        // Check file size every minute or so.
                        if ((DateTime.Now - _housekeepTime).TotalSeconds > 70)
                        {
                            _housekeepTime = DateTime.Now;
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
                    catch (Exception)
                    {
                        // TODO something or just leave it alone?
                        throw;
                    }

                    // Rest a bit.
                    await Task.Delay(50);
                }
            }, token);
        }

        /// <summary>
        /// Be polite, shut it down.
        /// </summary>
        public static void Stop()
        {
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
}