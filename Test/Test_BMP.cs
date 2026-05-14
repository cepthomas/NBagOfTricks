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
            Info("Test system Bitmap manipulation functions.");

            var bmpIn = (Bitmap)Image.FromFile(Path.Join(Program.InputDir, "color_wheel.png")); // 500 x 500

            // Resize bitmap.
            var bmpResize = bmpIn.Resize(300, 200);
            bmpResize.Save(Path.Join(Program.OutputDir, "resize.png"), ImageFormat.Png);

            //// Convert grayscale.
            //var bmpGray = bmpIn.ConvertToGrayscale();
            //bmpGray.Save(Path.Join(Program.OutputDir, "grayscale.png"), ImageFormat.Png);

            //// Colorize.
            //var bmpColorize = bmpIn.Colorize(Color.Yellow, Color.FromArgb(0x00, 0x80, 0x00));
            //bmpColorize.Save(Path.Join(Program.OutputDir, "colorize.png"), ImageFormat.Png);
        }
    }

    public class BMP_PIXELBMP : TestSuite
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
            pbmp1.GetBitmap().Save(Path.Join(Program.OutputDir, "pbmp_gradient.png"), ImageFormat.Png);
            var ls = pbmp1.Dump(10, 20, "howdy doody");

            // Grayscale.
            pbmp1.ConvertToGrayscale();
            pbmp1.GetBitmap().Save(Path.Join(Program.OutputDir, "pbmp_gray.png"), ImageFormat.Png);

            //pbmp1.Colorize(Color.Red, Color.FromArgb(0x00, 0x80, 0x00));
            //pbmp1.GetBitmap().Save(Path.Join(Program.OutputDir, "xxx.png"), ImageFormat.Png);

            // Colorize.
            //using PixelBitmap pbmp2 = new(Path.Join(Program.InputDir, "grayscale.png"));
            using PixelBitmap pbmp2 = new(Path.Join(Program.InputDir, "cogwheel.png"));
            pbmp2.Colorize(Color.Red, Color.Black);
            pbmp2.GetBitmap().Save(Path.Join(Program.OutputDir, "pbmp_colorize.png"), ImageFormat.Png);


        }
    }
}
