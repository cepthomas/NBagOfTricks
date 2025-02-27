using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;


namespace Ephemera.NBagOfTricks.Slog
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
        /// General function.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void Log(LogLevel level, string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (Enable)
            {
                AddEntry(level, msg, file, line);
            }
        }

        /// <summary>
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void Trace(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (Enable)
            {
                AddEntry(LogLevel.Trace, msg, file, line);
            }
        }

        /// <summary>
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void Debug(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (Enable)
            {
                AddEntry(LogLevel.Debug, msg, file, line);
            }
        }

        /// <summary>
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void Info(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (Enable)
            {
                AddEntry(LogLevel.Info, msg, file, line);
            }
        }

        /// <summary>
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void Warn(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (Enable)
            {
                AddEntry(LogLevel.Warn, msg, file, line);
            }
        }

        /// <summary>
        /// Convenience function.
        /// </summary>
        /// <param name="msg">Content.</param>
        /// <param name="file">Ignore - compiler use.</param>
        /// <param name="line">Ignore - compiler use.</param>
        public void Error(string msg, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            // Always log errors.
            AddEntry(LogLevel.Error, msg, file, line);
        }

        /// <summary>
        /// Log a caught exception. Adds info about who raised it
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="msg">Extra info.</param>
        public void Exception(Exception ex, string msg = "")
        {
            msg = msg.Length == 0 ? "" : " " + msg;
            // Find out who threw it.
            StackTrace st = new(ex, true);
            if (st.FrameCount > 0)
            {
                StackFrame? stf = st.GetFrame(0);
                string stfn = (stf != null && stf.GetFileName() != null) ? stf.GetFileName()! : "???";
                int stline = stf == null ? -1 : stf.GetFileLineNumber();
                AddEntry(LogLevel.Error, $"{ex.Message}{msg}", stfn, stline);
            }
            else
            {
                AddEntry(LogLevel.Error, $"{ex.Message}{msg}", "???", -1);
            }
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
            file = Path.GetFileName(file);
            LogEntry le = new(DateTime.Now, level, Name, file, line, msg);
            // Manager expedites.
            LogManager.LogThis(le);
        }
        #endregion
    }
}