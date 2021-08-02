using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NBagOfTricks;
using NBagOfTricks.PNUT;


namespace NBagOfTricks.Test
{
    public class UTILS_EXTENSIONS : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests various util functions.");

            string input = "12345 \"I HAVE SPACES\" aaa bbb  \"me too\" ccc ddd \"  and the last  \"";
            // Output: 12345,I HAVE SPACES,aaa,bbb,me too,ccc,ddd,and the last

            var splits = input.SplitQuotedString();

            UT_EQUAL(splits.Count, 8);
            UT_EQUAL(splits[0], "12345");
            UT_EQUAL(splits[1], "I HAVE SPACES");
            UT_EQUAL(splits[2], "aaa");
            UT_EQUAL(splits[3], "bbb");
            UT_EQUAL(splits[4], "me too");
            UT_EQUAL(splits[5], "ccc");
            UT_EQUAL(splits[6], "ddd");
            UT_EQUAL(splits[7], "  and the last  ");

            input = " \"aaa ttt uuu\" 84ss \"  dangling quote  ";
            splits = input.SplitQuotedString();

            UT_EQUAL(splits.Count, 3);
            UT_EQUAL(splits[0], "aaa ttt uuu");
            UT_EQUAL(splits[1], "84ss");
            UT_EQUAL(splits[2], "  dangling quote  ");
        }
    }

    public class UTILS_MISC : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests misc utils.");

            var dir = MiscUtils.GetAppDataDir("Foo");
            UT_EQUAL(dir, @"C:\Users\cepth\AppData\Local\Foo");

            dir = MiscUtils.GetAppDataDir("Bar", "CCCC");
            UT_EQUAL(dir, @"C:\Users\cepth\AppData\Local\CCCC\Bar");
        }
    }

    public class UTILS_MIDI_TIME : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests midi time utils.");

            // If we use ppq of 8 (32nd notes):
            // 100 bpm = 800 ticks/min = 13.33 ticks/sec = 0.01333 ticks/msec = 75.0 msec/tick
            //  99 bpm = 792 ticks/min = 13.20 ticks/sec = 0.0132 ticks/msec  = 75.757 msec/tick

            MidiTime mt = new MidiTime()
            {
                InternalPpq = 8,
                MidiPpq = 0,
                Tempo = 100
            };

            UT_CLOSE(mt.InternalPeriod(), 75.0, 0.001);

            mt.Tempo = 99;
            UT_CLOSE(mt.InternalPeriod(), 75.757, 0.001);

            // An example midi file: WICKGAME.MID is 3:45 long.
            // DeltaTicksPerQuarterNote (ppq): 384.
            // 100 bpm = 38,400 ticks/min = 640 ticks/sec = 0.64 ticks/msec = 1.5625 msec/tick.
            // Length is 144,000 ticks = 3.75 min = 3:45.
            // Smallest event is 4 ticks. So ppq is actually 96.

            mt.MidiPpq = 384;
            mt.Tempo = 100;
            UT_CLOSE(mt.MidiToSec(144000) / 60.0, 3.75, 0.001);

            // Ableton Live exports MIDI files with a resolution of 96 ppq, which means a 16th note can be divided into 24 steps.
            // DeltaTicksPerQuarterNote (ppq): 96.
            // 100 bpm = 9,600 ticks/min = 160 ticks/sec = 0.16 ticks/msec = 6.25 msec/tick.

            mt.MidiPpq = 96;
            mt.Tempo = 100;
            UT_CLOSE(mt.MidiPeriod(), 6.25, 0.001);
        }
    }
}
