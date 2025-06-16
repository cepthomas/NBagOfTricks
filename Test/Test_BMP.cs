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
using Ephemera.NBagOfTricks;
using System.Drawing.Imaging;
using Ephemera.NBagOfTricks.PNUT;
using System.Windows.Forms;


namespace Ephemera.NBagOfTricks.Test
{
    public class BMP_MAIN : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Test system Bitmap manipulation functions.");

            var inputDir = Path.Join(MiscUtils.GetSourcePath(), "Files");
            var outputDir = Path.Join(MiscUtils.GetSourcePath(), "out");

            var bmpIn = (Bitmap)Image.FromFile(Path.Join(inputDir, "color-wheel.png")); // 500 x 500

            // Resize bitmap.
            var bmpResize = bmpIn.Resize(300, 200);
            bmpResize.Save(Path.Join(outputDir, "resize.png"), ImageFormat.Png);

            // Convert grayscale.
            var bmpGray = bmpIn.ConvertToGrayscale();
            bmpGray.Save(Path.Join(outputDir, "grayscale.png"), ImageFormat.Png);

            // Colorize.
            var bmpColorize = bmpIn.Colorize(Color.Yellow, Color.FromArgb(0x00, 0x80, 0x00));
            bmpColorize.Save(Path.Join(outputDir, "colorize.png"), ImageFormat.Png);
        }
    }

    public class BMP_PIXEL : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Test PixelBitmap functions.");

            var outputDir = Path.Join(MiscUtils.GetSourcePath(), "out");

            // Pixel bitmap.
            int size = 128;
            using PixelBitmap pbmp = new(size, size);
            foreach (var y in Enumerable.Range(0, size))
            {
                foreach (var x in Enumerable.Range(0, size))
                {
                    pbmp.SetPixel(x, y, Color.FromArgb(255, x * 2, y * 2, 150));
                }
            }
            pbmp.ClientBitmap.Save(Path.Join(outputDir, "pixels.png"), ImageFormat.Png);
        }
    }
}
