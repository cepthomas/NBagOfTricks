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


namespace Ephemera.NBagOfTricks.Test
{
    public class BMP_MAIN : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests bitmap manipulation functions.");

            var dir = MiscUtils.GetSourcePath();
            var inputDir = Path.Join(dir, "Files");
            var outputDir = Path.Join(dir, "out");

            var bmpIn = (Bitmap)Image.FromFile(Path.Join(inputDir, "color-wheel.png")); // 500 x 500

            // Resize bitmap.
            var bmpResize = BitmapUtils.ResizeBitmap(bmpIn, 300, 200);
            bmpResize.Save(Path.Join(outputDir, "resize.png"), ImageFormat.Png);

            // Convert grayscale.
            var bmpGray = BitmapUtils.ConvertToGrayscale(bmpIn);
            bmpGray.Save(Path.Join(outputDir, "grayscale.png"), ImageFormat.Png);

            // Colorize.
            var bmpColorize = BitmapUtils.ColorizeBitmap(bmpIn, Color.Yellow, Color.FromArgb(0x00, 0x80, 0x00));
            bmpColorize.Save(Path.Join(outputDir, "colorize.png"), ImageFormat.Png);
        }
    }
}
