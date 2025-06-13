using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace Ephemera.NBagOfTricks
{
    /// <summary>Simple/cheap profiling.</summary>
    public class TimeIt // TODO combine these two?
    {
        long _startTick = 0;
        long _lastTick = 0;

        public List<string> Captures { get; set; } = [];

        public TimeIt()
        {
            _startTick = Stopwatch.GetTimestamp();
            _lastTick = _startTick;
        }

        /// <summary>
        /// Measure one time.
        /// </summary>
        /// <param name="msg"></param>
        public void Snap(string msg)
        {
            long tick = Stopwatch.GetTimestamp();

            var durMsec = FormatTicks(tick - _lastTick);
            var totMsec = FormatTicks(tick - _startTick);
            var s = $"dur:{durMsec} tot:{totMsec} {msg}";
            Captures.Add(s);

            _lastTick = tick;
        }

        /// <summary>
        /// Conversion for stopwatch values.
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        string FormatTicks(long ticks)
        {
            double dur = 1000.0 * ticks / Stopwatch.Frequency;
            return dur.ToString("000.000");
        }
    }


    /// <summary>
    /// Diagnostics for timing measurement and analysis.
    /// </summary>
    public class TimingAnalyzer
    {
        #region Fields
        /// <summary>The internal timer.</summary>
        readonly Stopwatch _watch = new();

        /// <summary>Last grab time for calculating diff.</summary>
        long _lastTicks = -1;

        /// <summary>Delay at start.</summary>
        int _skipCount = 0;
        #endregion

        #region Properties
        /// <summary>Number of data points to grab for statistics.</summary>
        public long SampleSize { get; set; } = 10;

        /// <summary>Number of initial data points to exclude from stats.</summary>
        public int Skip { get; set; } = 0;

        /// <summary>Accumulated data points.</summary>
        public List<double> Times { get; set; } = new List<double>();

        /// <summary>Mean in msec.</summary>
        public double Mean { get; set; } = 0;

        /// <summary>Min in msec.</summary>
        public double Min { get; set; } = 0;

        /// <summary>Max in msec.</summary>
        public double Max { get; set; } = 0;

        /// <summary>SD in msec.</summary>
        public double SD { get; set; } = 0;
        #endregion

        /// <summary>
        /// Make readable.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Count:{Times.Count} Mean:{Mean:F3} Max:{Max:F3} Min:{Min:F3} SD:{SD:F3}";
        }

        /// <summary>
        /// Stop accumulator.
        /// </summary>
        public void Stop()
        {
            _watch.Stop();
            Times.Clear();
            _lastTicks = -1;
        }

        /// <summary>
        /// Execute this before measuring the duration of something.
        /// </summary>
        public void Arm()
        {
            if (!_watch.IsRunning)
            {
                _lastTicks = -1;
                _watch.Start();
            }

            _lastTicks = _watch.ElapsedTicks;
        }

        /// <summary>
        /// Do one read since Arm().
        /// </summary>
        /// <returns></returns>
        public double ReadOne()
        {
            double dt = 0.0;
            long t = _watch.ElapsedTicks; // snap!

            if (Times.Count >= SampleSize)
            {
                Times.Clear();
            }

            if (_lastTicks != -1 && _skipCount-- < 0)
            {
                dt = TicksToMsec(t - _lastTicks);
                Times.Add(dt);
            }
            _lastTicks = t;
            return dt;
        }

        /// <summary>
        /// Grab a data point. Also auto starts the timer.
        /// </summary>
        /// <returns>New stats are available.</returns>
        public bool Grab()
        {
            bool stats = false;

            if(!_watch.IsRunning)
            {
                Times.Clear();
                _lastTicks = -1;
                _watch.Start();
            }

            if (Times.Count >= SampleSize)
            {
                Times.Clear();
            }

            long et = _watch.ElapsedTicks; // snap!

            if (_lastTicks != -1 && _skipCount-- < 0)
            {
                double dt = TicksToMsec(et - _lastTicks);
                Times.Add(dt);
            }
            _lastTicks = et;

            if (Times.Count >= SampleSize)
            {
                // Process the collected stuff.
                Mean = Times.Average();
                Max = Times.Max();
                Min = Times.Min();
                SD = MathUtils.StandardDeviation(Times.ConvertAll(v => v));

                stats = true;
            }

            return stats;
        }

        /// <summary>
        /// Detail of captured values.
        /// </summary>
        /// <returns>String csv.</returns>
        public string Dump()
        {
            List<string> ls = new();

            // Time ordered.
            ls.Add("Ordered");
            Times.ForEach(t => ls.Add($"{t}"));

            // Sorted by (rounded) times.
            Dictionary<double, int> _bins = new();
            for (int i = 0; i < Times.Count; i++)
            {
                var t = Math.Round(Times[i], 2);
                _bins[t] = _bins.TryGetValue(t, out int value) ? value + 1 : 1;
            }
            ls.Add($"Msec,Count");
            var vv = _bins.Keys.ToList();
            vv.Sort();
            vv.ForEach(v => ls.Add($"{v},{_bins[v]}"));

            return string.Join(Environment.NewLine, ls);
        }

        /// <summary>
        /// Conversion for stopwatch values.
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        double TicksToMsec(long ticks)
        {
            return 1000.0 * ticks / Stopwatch.Frequency;
        }
    }

}
