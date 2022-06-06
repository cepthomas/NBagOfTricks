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
    #region Types
    /// <summary>Log level options.</summary>
    public enum Level { Trace = 0, Debug = 1, Info = 2, Warn = 3, Error = 4, Fatal = 5 }

    /// <summary>Log entry data container.</summary>
    internal struct LogEntry
    {
        public DateTime timestamp;
        public Level level;
        public string name; // logger name
        public string file; // source file
        public int line; // source line
        public string message;
    }

    public class LogEventArgs : EventArgs
    {
        public Level Level { get; set; } = Level.Info;
        public string Message { get; set; } = "";
    }
    #endregion

    /// <summary>Global server.</summary>
    public sealed class LogManager //TODOX un-static?
    {
        #region Fields
        /// <summary>Lazy singleton. https://csharpindepth.com/Articles/Singleton.</summary>
        private static readonly Lazy<LogManager> _instance = new(() => new LogManager());
        public static LogManager Instance { get { return _instance.Value; } }

        /// <summary>All loggers. Key is client supplied name.</summary>
        static readonly Dictionary<string, Logger> _loggers = new();

        /// <summary>Log record queue.</summary>
        static readonly ConcurrentQueue<LogEntry> _queue = new();

        /// <summary>For checking file rollover.</summary>
        static DateTime _housekeepTime = DateTime.Now;

        /// <summary>Constant width strings for visual aid.</summary>
        static readonly Dictionary<Level, string> _levelNames = new()
        {
            { Level.Trace, "TRC" },
            { Level.Debug, "DBG" },
            { Level.Info,  "INF" },
            { Level.Warn,  "WRN" },
            { Level.Error, "ERR" },
            { Level.Fatal, "!!!" }
        };

        /// <summary>Queue management.</summary>
        static readonly CancellationTokenSource _tokenSource = new();
        #endregion

        #region Properties
        /// <summary>Event filter for file.</summary>
        public static Level MinLevelFile { get; set; } = Level.Debug;

        /// <summary>Event filter for callback event.</summary>
        public static Level MinLevelNotif { get; set; } = Level.Info;

        /// <summary>Format for file records.</summary>
        public static string TimeFormat { get; set; } = "yyyy'-'MM'-'dd HH':'mm':'ss.fff";

        /// <summary>For diagnostics.</summary>
        public static int QueueSize { get { return _queue.Count; } }
        #endregion

        #region Events
        /// <summary>Callback event.</summary>
        public static event EventHandler<LogEventArgs>? Log;
        #endregion

        #region Public functions
        /// <summary>
        /// Create a new client side logger.
        /// </summary>
        /// <param name="name">Unique ID.</param>
        /// <returns>Minty fresh logger.</returns>
        public static Logger CreateLogger(string name)
        {
            if (_loggers.ContainsKey(name))
            {
                throw new ArgumentException("Invalid logger name", nameof(name));
            }
            else
            {
                Logger logger = new(name);
                _loggers[name] = logger;
                return logger;
            }
        }
        
        /// <summary>
        /// Inititialize builtin file logger.
        /// </summary>
        /// <param name="logFilePath">Builtin logger file path.</param>
        /// <param name="logSize">Builtin logger max size.</param>
        public static void Run(string logFilePath, int logSize)
        {
            if (logSize > 0)
            {
                // Check for validity by trying to open it.
                try
                {
                    using var writer = new StreamWriter(logFilePath, true);
                }
                catch (IOException)
                {
                    throw new ArgumentException("Invalid file name", nameof(logFilePath));
                }
            }

            var token = _tokenSource.Token;

            Task task = Task.Run(async () =>
            {
                bool run = true;
                while (run)
                {
                    if (token.IsCancellationRequested)
                    {
                        run = false;
                    }

                    if (!_queue.IsEmpty)
                    {
                        using var writer = new StreamWriter(logFilePath, true);

                        while (_queue.TryDequeue(out LogEntry le))
                        {
                            //cat = cat.Length >= CAT_SIZE ? cat.Left(CAT_SIZE) : cat.PadRight(CAT_SIZE);
                            var fn = Path.GetFileName(le.file);
                            var slevel = _levelNames[le.level];

                            if (writer is not null && le.level >= MinLevelFile)
                            {
                                string s = $"{DateTime.Now.ToString(TimeFormat)} {slevel} {le.name} {fn}({le.line}) {le.message}";
                                writer.WriteLine(s);
                                writer.Flush();
                            }

                            if (Log is not null && le.level >= MinLevelNotif)
                            {
                                string s = $"{slevel} {le.name} {fn}({le.line}) {le.message}";
                                Log.Invoke(null, new LogEventArgs() { Level = le.level, Message = s });
                            }
                        }
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

                        // Rest a bit.
                        await Task.Delay(50);
                    }
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

        /// <summary>
        /// Client logger wants to log something.
        /// </summary>
        /// <param name="le"></param>
        internal static void LogThis(LogEntry le)
        {
            _queue.Enqueue(le);
        }
    }
}