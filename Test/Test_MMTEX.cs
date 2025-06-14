using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Ephemera.NBagOfTricks.PNUT;


namespace Ephemera.NBagOfTricks.Test
{
    public class MMTEX : TestSuite
    {
        /// <summary>Fast timer.</summary>
        MmTimerEx _timer = new();

        /// <summary>Measurer.</summary>
        readonly TimeIt _tmit = new();

        /// <summary>State.</summary>
        bool _running = false;

        /// <summary>How many.</summary>
        const int NUM_SAMPLES = 1000;

        public override void RunSuite()
        {
            UT_INFO("Test MmTimerEx functions.");

            // Fast mm timer under test.
            _timer = new MmTimerEx();

            _timer.SetTimer(10, TimerElapsedEvent);

            _running = true;
            _tmit.SampleSize = NUM_SAMPLES;
            _tmit.Grab(); // this starts the tan.
            _timer.Start();

            while (_running);

            // Done - dump what we found.
            var fn = Path.Combine(MiscUtils.GetSourcePath(), "out", "tan_dump.csv");
            File.WriteAllText(fn, _tmit.Dump());

            // Resource clean up.
            _timer.Stop();
            _timer.Dispose();
        }

        /// <summary>
        /// Multimedia timer tick handler.
        /// </summary>
        void TimerElapsedEvent(double totalElapsed, double periodElapsed)
        {
            if (_tmit.Grab())
            {
                // All done.
                _running = false;
                _timer.Stop();
            }
        }
    }
}
