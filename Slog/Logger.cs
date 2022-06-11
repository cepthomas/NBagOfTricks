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

        /// <summary>Turn logger on or off.</summary>
        public bool Enable { get; set; } = true;
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
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void LogTrace(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (Enable)
            {
                AddEntry(Level.Trace, msg, file, line);
            }
        }

        /// <summary>
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void LogDebug(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (Enable)
            {
                AddEntry(Level.Debug, msg, file, line);
            }
        }

        /// <summary>
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void LogInfo(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (Enable)
            {
                AddEntry(Level.Info, msg, file, line);
            }
        }

        /// <summary>
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void LogWarn(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (Enable)
            {
                AddEntry(Level.Warn, msg, file, line);
            }
        }

        /// <summary>
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void LogError(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            // Always log errors.
            AddEntry(Level.Error, msg, file, line);
        }

        /// <summary>
        /// Log an exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="msg">Extra info.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void LogException(Exception ex, string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            // Always log exceptions.
            StringBuilder sb = new($"{ex.Message} {msg}");
            sb.Append(msg);

            if(ex.StackTrace is not null)
            {
                sb.Append(Environment.NewLine);
                sb.Append(ex.StackTrace);
            }

            while (ex.InnerException != null)
            {
                sb.Append(Environment.NewLine);
                ex = ex.InnerException;
                sb.Append(ex.Message);
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