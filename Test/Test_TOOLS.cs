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
    public class TOOLS_FORMAT : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests code formatter.");

            var s = @"
            namespace NBagOfTricks.Test
            {
            public class TOOLS_FORMAT : TestSuite
            {
             /*
             Make a new algorithmic squiggle
             */
             void newDoodle() {
               background(0);
               shapes = round(random(5,10));
               for (int i=0; i<shapes; i++) {
                 stroke(randomBrightColor());
                 strokeWeight(random(3,10));
                 double r = random(0,1.0);
                 if (r<0.25) { // 25% chance to draw a circle
                   fill(random(0,255), random(0,255), random(0,255), random(0,255));
                   double size = random(width/8, width/4);
                   ellipse(random(minW,maxW), random(minH,maxH), size, size);
                 } else { // otherwise draw a 2-4 vertex line
                   int vertices = round(random(5.0,10.0));
                   beginShape();
                   fill(0,0); // transparent fill
                   for (int j=0; j<vertices; j++) {
                     if (curves) {
                       //curveVertex(random(minW,maxW), random(minH,maxH));
                     } else {
                       vertex(random(minW,maxW), random(minH,maxH));
                     }
                   }
                   endShape();
                 }
               }
             }}}
            ";

            var ls = s.SplitByTokens(Environment.NewLine);

            var res = Tools.FormatSourceCode(ls);
        }
    }

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
