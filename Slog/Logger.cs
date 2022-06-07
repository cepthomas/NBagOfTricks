using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;


namespace NBagOfTricks.Slog
{
    /// <summary>Client creates as many of these as needed.</summary>
    public class Logger  
    {
        #region Properties
        /// <summary>ID for this logger.</summary>
        public string Name { get; init; } = "";
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Client assigned name.</param>
        public Logger(string name)
        {
            Name = name;
        }
        #endregion

        #region Public functions
        /// <summary>
        /// Basic log function.
        /// </summary>
        /// <param name="level">Specific level.</param>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void Log(Level level, string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            AddEntry(level, msg, file, line);
        }

        /// <summary>
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void LogTrace(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            AddEntry(Level.Trace, msg, file, line);
        }

        /// <summary>
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void LogDebug(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            AddEntry(Level.Debug, msg, file, line);
        }

        /// <summary>
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void LogInfo(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            AddEntry(Level.Info, msg, file, line);
        }

        /// <summary>
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void LogWarn(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            AddEntry(Level.Warn, msg, file, line);
        }

        /// <summary>
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void LogError(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            AddEntry(Level.Error, msg, file, line);
        }

        /// <summary>
        /// Log an exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="msg">Extra info.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void Log(Exception ex, string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            StringBuilder sb = new($"{ex.Message} {msg}");
            sb.Append(msg);
            sb.Append(Environment.NewLine);

            if(ex.StackTrace is not null)
            {
                sb.Append(ex.StackTrace);
                sb.Append(Environment.NewLine);
            }

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                sb.Append(ex.Message);
                sb.Append(Environment.NewLine);
            }

            AddEntry(Level.Error, sb.ToString(), file, line);
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
        void AddEntry(Level level, string msg, string file, int line)
        {
            file = Path.GetFileName(file);

            LogEntry le = new();

            le.timestamp = DateTime.Now;
            le.level = level;
            le.name = Name; // logger name
            le.file = file; // source file
            le.line = line; // source line
            le.message = msg;

            // Manager expedites.
            LogManager.LogThis(le);
        }
        #endregion
    }
}