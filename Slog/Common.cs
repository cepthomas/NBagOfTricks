using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ephemera.NBagOfTricks;


namespace Ephemera.NBagOfTricks.Slog
{
    #region Types
    /// <summary>Log level options.</summary>
    public enum LogLevel { Trace = 0, Debug = 1, Info = 2, Warn = 3, Error = 4 }

    /// <summary>Internal log entry data container.</summary>
    internal record LogEntry(DateTime Timestamp, LogLevel Level, string LoggerName, string SourceFile, int SourceLine, string Message);

    /// <summary>Log event for notification.</summary>
    public class LogEventArgs : EventArgs
    {
        public LogLevel Level { get; set; } = LogLevel.Info;
        public string Message { get; set; } = "";
    }
    #endregion

    /// <summary>Experimental class to log enter/exit scope. Use syntax "using new Scoper(...);"</summary>
    public sealed class Scoper : IDisposable
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