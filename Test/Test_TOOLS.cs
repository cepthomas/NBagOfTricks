using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.PNUT;


namespace Ephemera.NBagOfTricks.Test
{
    public class TOOLS_BIN_SNIFF : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests file sniffer.");

            string fn = @"..\..\Files\nonascii.txt";

            var res = Tools.SniffBin(fn);

            UT_EQUAL(res.Count, 1);
            UT_EQUAL(res[0], "row:3 col:8 val:11(B) b:0");
        }
    }

    public class TOOLS_MD_TO_HTML : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests markdown to html.");

            var docs = MusicDefinitions.FormatDoc();

            //Tools.MarkdownToHtml(docs, Tools.MarkdownMode.Simple, true);
            //Tools.MarkdownToHtml(docs, Tools.MarkdownMode.DarkApi, true);
            Tools.MarkdownToHtml(docs, Tools.MarkdownMode.LightApi, true);
        }
    }
}
