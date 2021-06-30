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
    public class TOOLS_BIN_SNIFF : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests file sniffer.");

            string fn = @".\Files\nonascii.txt";

            var res = Tools.SniffBin(fn);

            UT_EQUAL(res.Count, 1);
            UT_EQUAL(res[0], "row:3 col:8 val:11(B) b:0");
        }
    }
}
