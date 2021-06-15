using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace NBagOfTricks.UI
{
    /// <summary>
    /// Static general utility functions.
    /// </summary>
    public static class GraphicsUtils
    {
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            //a holder for the result
            Bitmap result = new Bitmap(width, height);

            // set the resolutions the same to avoid cropping due to resolution differences
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                // set the resize quality modes to high quality
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                // draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            return result;
        }

        /// <summary>
        /// Colorize a bitmap.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="newcol"></param>
        /// <returns></returns>
        public static Bitmap ColorizeBitmap(Image original, Color newcol)
        {
            Bitmap origbmp = original as Bitmap;
            Bitmap newbmp = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < newbmp.Height; y++) // This is not very effecient! Use a buffer...
            {
                for (int x = 0; x < newbmp.Width; x++)
                {
                    // Get the pixel from the image.
                    Color acol = origbmp.GetPixel(x, y);

                    // Test for not background.
                    if (acol.A > 0)
                    {
                        Color c = Color.FromArgb(acol.A, newcol.R, newcol.G, newcol.B);
                        newbmp.SetPixel(x, y, c);
                    }
                }
            }

            return newbmp;
        }

        /// <summary>
        /// Helper to get next contrast color in the sequence.
        /// From http://colorbrewer2.org qualitative.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="dark">Dark or light series, usually dark.</param>
        /// <returns></returns>
        public static Color GetSequenceColor(int i, bool dark = true)
        {
            Color col = Color.Black;

            switch (i % 8)
            {
                case 0: col = dark ? Color.FromArgb(27, 158, 119) : Color.FromArgb(141, 211, 199); break;
                case 1: col = dark ? Color.FromArgb(217, 95, 2) : Color.FromArgb(255, 255, 179); break;
                case 2: col = dark ? Color.FromArgb(117, 112, 179) : Color.FromArgb(190, 186, 218); break;
                case 3: col = dark ? Color.FromArgb(231, 41, 138) : Color.FromArgb(251, 128, 114); break;
                case 4: col = dark ? Color.FromArgb(102, 166, 30) : Color.FromArgb(128, 177, 211); break;
                case 5: col = dark ? Color.FromArgb(230, 171, 2) : Color.FromArgb(253, 180, 98); break;
                case 6: col = dark ? Color.FromArgb(166, 118, 29) : Color.FromArgb(179, 222, 105); break;
                case 7: col = dark ? Color.FromArgb(102, 102, 102) : Color.FromArgb(252, 205, 229); break;
            }

            return col;
        }

        /// <summary>
        /// Mix two colors.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static Color HalfMix(Color one, Color two)
        {
            return Color.FromArgb(
                (one.A + two.A) >> 1,
                (one.R + two.R) >> 1,
                (one.G + two.G) >> 1,
                (one.B + two.B) >> 1);
        }
    }
}
