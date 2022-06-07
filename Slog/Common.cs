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

    /// <summary>Internal log entry data container.</summary>
    internal struct LogEntry
    {
        public DateTime timestamp;
        public Level level;
        public string name; // logger name
        public string file; // source file
        public int line; // source line
        public string message;
    }

    /// <summary>Log event for  log entry data container.</summary>
    public class LogEventArgs : EventArgs
    {
        public Level Level { get; set; } = Level.Info;
        public string Message { get; set; } = "";
    }

    public class Definitions
    {
        /// <summary>Verbose timestamp.</summary>
        public const string TIME_FORMAT = "yyyy'-'MM'-'dd HH':'mm':'ss.fff";
    }
    #endregion

    /// <summary>Experimental class to log enter/exit scope. Use syntax "using new Scoper(...);"</summary>
    public class Scoper : IDisposable
    {
        Logger _logger;
        string _id;

        public Scoper(Logger logger, string id)
        {
            _logger = logger;
            _id = id;
            _logger.LogTrace($"{_id}: Enter scope");
        }

        public void Dispose()
        {
            _logger.LogTrace($"{_id}: Leave scope");
        }
    }
}