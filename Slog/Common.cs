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
    public enum LogLevel { Trace = 0, Debug = 1, Info = 2, Warn = 3, Error = 4 }

    /// <summary>Internal log entry data container.</summary>
    internal struct LogEntry
    {
        public DateTime timestamp;
        public LogLevel level;
        public string name; // logger name
        public string file; // source file
        public int line; // source line
        public string message;
    }

    /// <summary>Log event for notification.</summary>
    public class LogEventArgs : EventArgs
    {
        public LogLevel Level { get; set; } = LogLevel.Info;
        public string Message { get; set; } = "";
    }
    #endregion

    /// <summary>Experimental class to log enter/exit scope. Use syntax "using new Scoper(...);"</summary>
    public class Scoper : IDisposable
    {
        readonly Logger _logger;
        readonly string _id;

        public Scoper(Logger logger, string id)
        {
            _logger = logger;
            _id = id;
            _logger.Trace($"{_id}: Enter scope");
        }

        public void Dispose()
        {
            _logger.Trace($"{_id}: Leave scope");
        }
    }
}