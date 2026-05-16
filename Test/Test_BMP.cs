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
    public class PBMP_MAIN : TestSuite
    {
        public override void RunSuite()
        {
            Info("Test PixelBitmap functions.");

            // Basics.
            {
                // Create a default gradient bitmap.
                int size = 256;
                using PixelBitmap pbmp = new(size, size);
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        var clr = Color.FromArgb(255, x, y, 100);
                        pbmp.SetPixel(x, y, clr);
                    }
                }

                // Diagnostics.
                var ls = pbmp.Dump(10, 20);
                Assert(ls.Count == 5120);

                // Write bitmap.
                var bmp = pbmp.GetBitmap();
                bmp.Save(Path.Join(Program.OutputDir, "pbmp_gradient.png"), ImageFormat.Png);

                // To grayscale.
                pbmp.ConvertToGrayscale();
                bmp = pbmp.GetBitmap();
                bmp.Save(Path.Join(Program.OutputDir, "pbmp_gradient_gray.png"), ImageFormat.Png);
            }

            // Colorize.
            {
                // 32 bit 256 X 256  stride 1024  65536px   file size:4096 bytes
                using PixelBitmap pbmp = new(Path.Join(Program.InputDir, "grayscale.png"));
                pbmp.Colorize(Color.Yellow, Color.FromArgb(0x80, 0x80, 0x80));
                using var bmp = pbmp.GetBitmap();
                bmp.Save(Path.Join(Program.OutputDir, "pbmp_colorize.png"), ImageFormat.Png);
            }

            // Other than 32 bit.
            // 24 bit. 224 X 249  stride 672  55776px  file size:5867 bytes
            {
                using PixelBitmap pbmp = new(Path.Join(Program.InputDir, "24bit.png")); // Format24bppRgb
                using var bmp = pbmp.GetBitmap();
                bmp.Save(Path.Join(Program.OutputDir, "pbmp_24bit.png"), ImageFormat.Png);
            }

            // 8 bit. 257 X 257 1415 bytes
            {
                using PixelBitmap pbmp = new(Path.Join(Program.InputDir, "8bit.png")); // actually Format32bppArgb
                using var bmp = pbmp.GetBitmap();
                bmp.Save(Path.Join(Program.OutputDir, "pbmp_8bit.png"), ImageFormat.Png);
            }
        }
    }
}
