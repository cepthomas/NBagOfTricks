using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.PNUT;


namespace Ephemera.NBagOfTricks.Test
{
    public class COLOR_CONV : TestSuite
    {
        public override void RunSuite()
        {
            Info("Test color conversion functions.");

            //ColorDialog d = new();
            //d.ShowDialog();
            //return;
        }

        readonly List<string> _ross =
        [
            "You have freedom here.",
            "The only guide is your heart.",
            "We can always carry this a step further.",
            "There's really no end to this.",
            "Let's give him a friend too.",
            "Everybody needs a friend.",
            "Follow the lay of the land.",
            "It's most important.",
            "Only eight colors that you need.",
            "Now we can begin working on lots.",
            "Even the worst thing we can do here is good.",
            "Let's do it again then, what the heck.",
            "Everything's not great in life.",
            "Use what happens naturally, don't fight it.",
            "How do you make a round circle?",
            "That's your challenge for the day."
        ];

        static string FormatForHtml(Color clr)
        {
            return $"#{clr.R:x2}{clr.G:x2}{clr.B:x2}";
        }
    }

    public class COLOR_ANSI : TestSuite
    {
        public override void RunSuite()
        {
            Info("Test ANSI color functions.");

            //char ESC = 27;
            // One of: ESC[IDm  ESC[38;5;IDm  ESC[48;5;IDm  ESC[38;2;R;G;Bm  ESC[48;2;R;G;Bm

            var (fg, bg) = ColorUtils.ColorFromAnsi("bad string");
            Assert(fg.IsEmpty);
            Assert(bg.IsEmpty);

            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[34m");
            Assert(fg.Name == "ff00007f");
            Assert(bg.Name == "0");

            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[45m");
            Assert(fg.Name == "0");
            Assert(bg.Name == "ff7f007f");

            // system
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[38;5;12m");
            Assert(fg.Name == "ff0000ff");
            Assert(bg.Name == "0");

            // id
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[38;5;122m");
            Assert(fg.Name == "ff87ffd7");
            Assert(bg.Name == "0");

            // grey
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[38;5;249m");
            Assert(fg.Name == "ffb2b2b2");
            Assert(bg.Name == "0");

            // id bg
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[48;5;231m");
            Assert(fg.Name == "0");
            Assert(bg.Name == "ffffffff");

            //ESC[38;2;R;G;Bm
            // rgb
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[38;2;204;39;187m");
            Assert(fg.Name == "ffcc27bb");
            Assert(bg.Name == "0");

            // rgb invert
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[48;2;19;0;222m");
            Assert(fg.Name == "0");
            Assert(bg.Name == "ff1300de");
        }
    }
}
