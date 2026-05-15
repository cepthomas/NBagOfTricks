using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json.Serialization;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Ephemera.NBagOfTricks.PNUT;
using Ephemera.NBagOfTricks;


namespace Ephemera.NBagOfTricks.Test
{
    public class BMP_MAIN : TestSuite
    {
        public override void RunSuite()
        {
            Info("Test PixelBitmap functions.");

            // Create a default gradient bitmap.
            int size = 256;
            using PixelBitmap pbmp1 = new(size, size);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    var clr = Color.FromArgb(255, x, y, 100);
                    pbmp1.SetPixel(x, y, clr);
                }
            }

            // Diagnostics.
            var ls = pbmp1.Dump(10, 20);
            Assert(ls.Count == 5120);

            // Write bitmap.
            using var bmp1 = pbmp1.GetBitmap();
            bmp1.Save(Path.Join(Program.OutputDir, "pbmp_gradient.png"), ImageFormat.Png);

            // Grayscale.
            pbmp1.ConvertToGrayscale();
            using var bmp2 = pbmp1.GetBitmap();
            bmp2.Save(Path.Join(Program.OutputDir, "pbmp_gradient_gray.png"), ImageFormat.Png);

            // Colorize.
            using PixelBitmap pbmp2 = new(Path.Join(Program.InputDir, "grayscale.png"));
            pbmp2.Colorize(Color.Yellow, Color.FromArgb(0x80, 0x80, 0x80));
            //using PixelBitmap pbmp2 = new(Path.Join(Program.InputDir, "cogwheel.png"));
            //pbmp2.Colorize(Color.Red, Color.Black);
            using var bmp3 = pbmp2.GetBitmap();
            bmp3.Save(Path.Join(Program.OutputDir, "pbmp_colorize.png"), ImageFormat.Png);
        }
    }
}
