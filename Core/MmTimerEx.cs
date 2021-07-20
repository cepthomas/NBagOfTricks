using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace NBagOfTricks
{
    /// <summary>
    /// The win multimedia timer is erratic. Using a one msec tick (fastest), the measured interval is +-100 us.
    /// Also note that it takes about 10 ticks to settle down after start.
    /// This component attempts to reduce the error by running at one msec and managing the requested periods manually.
    /// This is accomplished by using a Stopwatch to actually measure the elapsed time rather than trust the mm timer period.
    /// It seems to be an improvement.
    /// </summary>
    public class MmTimerEx : IDisposable
    {
        //class TimerInstance
        //{
        //    /// <summary>The requested time between events in msec.</summary>
        //    public int period = 10;

        //    /// <summary>Actual accumulated msec.</summary> 
        //    public double current = 0.0;
        //}

        #region Fields
        /// <summary>All the timer instances. Key is id.</summary>
        //readonly Dictionary<string, TimerInstance> _timers = new Dictionary<string, TimerInstance>();

        /// <summary>Used for more accurate timing.</summary>
        readonly Stopwatch _sw = new Stopwatch();

        /// <summary>Multimedia timer identifier. -1 is not inited, 0 is fail to init, other is valid id.</summary>
        int _timerID = -1;

        /// <summary>Indicates whether or not the timer is running.</summary>
        bool _running = false;

        /// <summary>Stopwatch support.</summary>
        long _startTicks = -1;

        /// <summary>Stopwatch support.</summary>
        long _lastTicks = -1;

        /// <summary>Indicates whether or not the timer has been disposed.</summary>
        bool _disposed = false;

        /// <summary>Msec for mm timer tick.</summary>
        const int MMTIMER_PERIOD = 1;

        /// <summary>Called by Windows when a mm timer event occurs.</summary>
        readonly NativeMethods.TimeProc _timeProc;
        #endregion


        #region Events
        ///// <summary>Occurs when the time period has elapsed.</summary>
        //public event EventHandler<TimerEventArgs> TimerElapsedEvent;

        ///// <summary>Timer event args.</summary>
        //public class TimerEventArgs : EventArgs
        //{
        //    /// <summary>Elapsed timers.</summary>
        //    public List<string> ElapsedTimers { get; set; } = new List<string>();
        //}
        #endregion




        //public event EventHandler<TimerEventArgs> TimerElapsedEventX;
        public delegate void TimeProcX(double totalElapsed, double periodElapsed);

        class TimerInstanceX
        {
            /// <summary>The requested time between events in msec.</summary>
            public int period = 10;

            /// <summary>User handler.</summary>
            public TimeProcX handler = null;

            /// <summary>Actual accumulated msec since last event.</summary> 
            public double elapsed = 0.0;
        }

        List<TimerInstanceX> _timersX = new List<TimerInstanceX>();

        public void SetTimer(int period, TimeProcX handler)
        {
            _timersX.Add(new TimerInstanceX
            {
                period = period,
                handler = handler
            });
        }




        #region Native Methods
        /// <summary>Win32 Multimedia Timer Functions.</summary>
        internal class NativeMethods
        {
            #pragma warning disable IDE1006 // Naming Styles

            /// <summary></summary>
            /// <param name="caps"></param>
            /// <param name="sizeOfTimerCaps"></param>
            /// <returns></returns>
            [DllImport("winmm.dll")]
            internal static extern int timeGetDevCaps(ref TimerCaps caps, int sizeOfTimerCaps);

            /// <summary>Start her up.</summary>
            /// <param name="delay">Event delay, in milliseconds.If this value is not in the range of the minimum and maximum event delays supported by the timer, the function returns an error.</param>
            /// <param name="resolution">Resolution of the timer event, in milliseconds. The resolution increases with smaller values; a resolution of 0 indicates periodic events should occur with the greatest possible accuracy. To reduce system overhead, however, you should use the maximum value appropriate for your application.</param>
            /// <param name="proc">Pointer to a callback function that is called once upon expiration of a single event or periodically upon expiration of periodic events.</param>
            /// <param name="user">User-supplied callback data.</param>
            /// <param name="mode">Timer event type.</param>
            /// <returns></returns>
            [DllImport("winmm.dll")]
            internal static extern int timeSetEvent(int delay, int resolution, TimeProc proc, IntPtr user, int mode);

            /// <summary></summary>
            /// <param name="id"></param>
            /// <returns></returns>
            [DllImport("winmm.dll")]
            internal static extern int timeKillEvent(int id);

            /// <summary>Called by Windows when a mm timer event occurs.</summary>
            internal delegate void TimeProc(int id, int msg, int user, int param1, int param2);

            /// <summary>Represents information about the multimedia timer capabilities.</summary>
            [StructLayout(LayoutKind.Sequential)]
            internal struct TimerCaps
            {
                /// <summary>Minimum supported period in milliseconds.</summary>
                public int periodMin;

                /// <summary>Maximum supported period in milliseconds.</summary>
                public int periodMax;
            }

            #pragma warning restore IDE1006 // Naming Styles
        }
        #endregion

        #region Lifecycle
        /// <summary>
        /// Initializes a new instance of the Timer class.
        /// </summary>
        public MmTimerEx()
        {
            if (!Stopwatch.IsHighResolution)
            {
                throw new Exception("High res performance counter is not available.");
            }

            // Initialize timer with default values.
            _timeProc = new NativeMethods.TimeProc(MmTimerCallback);
        }

        /// <summary>
        /// Frees timer resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Frees timer resources.
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Stop();
                    GC.SuppressFinalize(this);
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~MmTimerEx()
        {
            Dispose(false);
        }
        #endregion

        #region Public functions
        ///// <summary>
        ///// Add a new timer instance.
        ///// </summary>
        ///// <param name="id">Arbitrary id as string</param>
        ///// <param name="period">Period in msec </param>
        //public void SetTimer(string id, int period)
        //{
        //    _timers[id] = new TimerInstance
        //    {
        //        period = period,
        //        current = 0
        //    };
        //}

        /// <summary>
        /// Starts the periodic timer.
        /// </summary>
        public void Start()
        {
            foreach (TimerInstanceX t in _timersX)
            {
                t.elapsed = 0;
            }

            // Clean up first.
            Stop();

            // Create and start periodic timer. resolution is 0 or 1, mode is 1=TIME_PERIODIC
            _timerID = NativeMethods.timeSetEvent(MMTIMER_PERIOD, 0, _timeProc, IntPtr.Zero, 1);

            // If the timer was created successfully.
            if (_timerID > 0)
            {
                _sw.Start();
                _running = true;
            }
            else
            {
                _running = false;
                _timerID = -1;
                throw new Exception("Unable to start periodic multimedia Timer.");
            }
        }

        /// <summary>
        /// Stops timer.
        /// </summary>
        public void Stop()
        {
            // Stop and destroy timer.
            if(_timerID > 0)
            {
                int result = NativeMethods.timeKillEvent(_timerID);
                // result != TIMERR_NOERROR <> 0
                _timerID = -1;
            }
            _running = false;
            _sw.Stop();
        }
        #endregion

        #region Private functions
        /// <summary>
        /// System multimedia timer callback. Don't trust the accuracy of the mm timer so measure actual using a stopwatch.
        /// </summary>
        /// <param name="id">The identifier of the timer. The identifier is returned by the timeSetEvent function.</param>
        /// <param name="msg">Reserved.</param>
        /// <param name="user">The value that was specified for the dwUser parameter of the timeSetEvent function.</param>
        /// <param name="param1">Reserved.</param>
        /// <param name="param2">Reserved.</param>
        void MmTimerCallback(int id, int msg, int user, int param1, int param2)
        {
            if (_running)
            {
                if(_lastTicks != -1)
                {
                    // When are we?
                    long t = _sw.ElapsedTicks; // snap

                    double totalMsec = (t - _startTicks) * 1000D / Stopwatch.Frequency;
                    double periodMsec = (t - _lastTicks) * 1000D / Stopwatch.Frequency;
                    _lastTicks = t;

                    // Check for expirations. Allow for a bit of jitter around 0.
                    double allowance = 0.5; // msec


                    foreach (var timer in _timersX)
                    {
                        timer.elapsed += periodMsec;
                        if ((timer.period - timer.elapsed) < allowance)
                        {
                            // Notify.
                            timer.handler(totalMsec, timer.elapsed);
                            timer.elapsed = 0.0;
                        }
                    }

                    //foreach(string tid in _timers.Keys)
                    //{
                    //    TimerInstance timer = _timers[tid];
                    //    timer.current += msec;
                    //    if ((timer.period - timer.current) < allowance)
                    //    {
                    //        elapsed.Add(tid);
                    //        timer.current = 0.0;
                    //    }
                    //}

                    //if (elapsed.Count > 0)
                    //{
                    //    TimerElapsedEvent?.Invoke(this, new TimerEventArgs() { ElapsedTimers = elapsed });
                    //}
                }
                else
                {
                    // Starting.
                    _startTicks = _sw.ElapsedTicks;
                    _lastTicks = _startTicks;
                }
            }
        }
        #endregion
    }
}
