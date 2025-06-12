using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Ephemera.NBagOfTricks
{
    /// <summary>
    /// A watcher for multiple file changes. The underlying FileSystemWatcher is a bit sensitive to OS file system ops.
    /// </summary>
    public sealed class MultiFileWatcher : IDisposable
    {
        #region Events
        /// <summary>Reporting a change to listeners.</summary>
        public class FileChangeEventArgs : EventArgs
        {
            public HashSet<string> FileNames { get; set; } = new();
        }
        public event EventHandler<FileChangeEventArgs>? FileChange = null;
        #endregion

        #region Fields
        /// <summary>Detect changed files.</summary>
        readonly List<FileSystemWatcher> _watchers = new();

        /// <summary>Used to delay reporting to client as there can be multiple events for one file change.</summary>
        readonly System.Timers.Timer _timer = new();

        /// <summary>Set by subordinate watchers.</summary>
        readonly HashSet<string> _touchedFiles = new();

        /// <summary>The delay before reporting. Seems like a reasonable number for human edit interface.</summary>
        const int DELAY = 100;
        #endregion

        #region Properties
        public List<string> WatchedFiles
        {
            get
            {
                List<string> fns = new();
                _watchers.ForEach(w => fns.Add(Path.Join(w.Path, w.Filter)));
                return fns;
            }
        }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiFileWatcher()
        {
            _timer.Interval = DELAY;
            _timer.Enabled = true;
            _timer.Elapsed += Timer_Elapsed;
            _touchedFiles.Clear();
        }

        /// <summary>
        /// Handle timer tick. Sends event out if any constituents triggered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if(_touchedFiles.Count > 0)
            {
                FileChange?.Invoke(this, new FileChangeEventArgs() { FileNames = _touchedFiles });
                _touchedFiles.Clear();
            }
        }

        /// <summary>
        /// Add a new listener.
        /// </summary>
        /// <param name="path"></param>
        /// <return>Pass/fail</return>
        public bool Add(string path)
        {
            bool ok = false;

            try
            {
                string? npath = Path.GetDirectoryName(path);
                if(npath is not null)
                {
                    FileSystemWatcher watcher = new()
                    {
                        Path = npath,
                        Filter = Path.GetFileName(path),
                        EnableRaisingEvents = true,
                        NotifyFilter = NotifyFilters.LastWrite
                    };

                    watcher.Changed += Watcher_Changed;
                    _watchers.Add(watcher);
                    ok = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ok;
        }

        /// <summary>
        /// Remove all listeners.
        /// </summary>
        public void Clear()
        {
            _watchers.ForEach(w =>
            {
                w.Changed -= Watcher_Changed;
                w.Dispose();
            });
            _watchers.Clear();

            _touchedFiles.Clear();
            _timer.Interval = DELAY;
        }

        /// <summary>
        /// Handle underlying change notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            _touchedFiles.Add(e.FullPath);
            // Reset timer.
            _timer.Interval = DELAY;
        }

        /// <summary>
        /// Resource clean up.
        /// </summary>
        public void Dispose()
        {
            _watchers.ForEach(w => w.Dispose());
            _timer.Dispose();
        }
    }
}
