using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace Ephemera.NBagOfTricks
{
    /// <summary>Simple/cheap profiling.</summary>
    public class TimeIt
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
}
