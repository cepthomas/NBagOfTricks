using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using NBagOfTricks.PNUT;


namespace NBagOfTricks.Test
{
    public class MMTEX : TestSuite
    {
        /// <summary>Fast timer.</summary>
        MmTimerEx _timer = new MmTimerEx();

        /// <summary>Measurer.</summary>
        readonly TimingAnalyzer _tan = new TimingAnalyzer();

        /// <summary>State.</summary>
        bool _running = false;

        /// <summary>How many.</summary>
        const int NUM_SAMPLES = 1000;

        public override void RunSuite()
        {
            UT_INFO("Tests MmTimerEx functions.");

            // Fast mm timer under test.
            _timer = new MmTimerEx();

            _timer.SetTimer(10, TimerElapsedEvent);

            _running = true;
            _tan.SampleSize = NUM_SAMPLES;
            _tan.Grab(); // this starts the tan.
            _timer.Start();

            // Do some background work.
            //for(int i = 0; _running; i++)
            //{
            //    for(int j = 0; j < 100; j++)
            //    {
            //        string s = $"abcdef {i} {j}";
            //        //Console.WriteLine(s);
            //    }
            //}
            while (_running) ;

            // Done - dump what we found.
            List<string> ls = new List<string> { $"Msec" };

            // Time ordered.
            _tan.Times.ForEach(t => ls.Add($"{t}"));
            File.WriteAllLines(@"..\..\out\intervals_ordered.csv", ls);

            // Sorted by (rounded) times.
            Dictionary<double, int> _bins = new Dictionary<double, int>();
            for (int i = 0; i < _tan.Times.Count; i++)
            {
                var t = Math.Round(_tan.Times[i], 2);
                _bins[t] = _bins.ContainsKey(t) ? _bins[t] + 1 : 1;
            }
            ls.Clear();
            ls.Add($"Msec,Count");
            var vv = _bins.Keys.ToList();
            vv.Sort();
            vv.ForEach(v => ls.Add($"{v},{_bins[v]}"));
            File.WriteAllLines(@"..\..\out\intervals_sorted.csv", ls);

            //Debug.WriteLine(_tan);
            //Count:1000 Mean:9.982 Max:10.977 Min:9.040 SD:0.096

            // Resource clean up.
            _timer.Stop();
            _timer.Dispose();
        }

        /// <summary>
        /// Multimedia timer tick handler.
        /// </summary>
        void TimerElapsedEvent(double totalElapsed, double periodElapsed)
        {
            if (_tan.Grab())
            {
                // All done.
                _running = false;
                _timer.Stop();
            }
        }
    }
}
