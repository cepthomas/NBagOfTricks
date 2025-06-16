using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Ephemera.NBagOfTricks
{
    #region Extension methods for manipulating bmps - not for realtime
    public static class BitmapUtils
    {
        /// <summary>De-colorize.</summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ConvertToGrayscale(this Bitmap bmp)
        {
            Bitmap result = new(bmp.Width, bmp.Height);

            // Conversion algo (worst to best)
            // - Simple averaging.
            // - channel-dependent luminance perception
            //   Y = R * 0.2126 + G * 0.7152 + B * 0.0722; // 0.0 to 255.0
            // - gamma-compression-corrected approximation:
            //   Y = 0.299 R + 0.587 G + 0.114 B
            ColorMatrix mat = new(
            [
                [.30f, .30f, .30f,  0,  0],
                [.59f, .59f, .59f,  0,  0],
                [.11f, .11f, .11f,  0,  0],
                [   0,    0,    0,  1,  0],
                [   0,    0,    0,  0,  1]
            ]);

            // Identity matrix for dev.
            //ColorMatrix mat = new(
            //[
            //    [  1,  0,  0,  0,  0],
            //    [  0,  1,  0,  0,  0],
            //    [  0,  0,  1,  0,  0],
            //    [  0,  0,  0,  1,  0],
            //    [  0,  0,  0,  0,  1]
            //]);

            using (Graphics g = Graphics.FromImage(result))
            {
                using ImageAttributes attributes = new();
                attributes.SetColorMatrix(mat);
                g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
            }

            return result;
        }

        /// <summary>Resize the image to the specified width and height.</summary>
        /// <param name="bmp">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap Resize(this Bitmap bmp, int width, int height)
        {
            Bitmap result = new(width, height);
            result.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                // Set high quality.
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                // Draw the image.
                graphics.DrawImage(bmp, 0, 0, result.Width, result.Height);
            }

            return result;
        }

        /// <summary>Colorize a bitmap. Mainly for beautifying glyphicons.</summary>
        /// <param name="original"></param>
        /// <param name="newcol"></param>
        /// <param name="replace">Optional source color to replace. Defaults to black.</param>
        /// <returns></returns>
        public static Bitmap Colorize(this Bitmap original, Color newcol, Color replace = default)
        {
            Bitmap newbmp = new(original.Width, original.Height);

            for (int y = 0; y < newbmp.Height; y++) // This is not very efficient. Use a matrix instead.
            {
                for (int x = 0; x < newbmp.Width; x++)
                {
                    // Get the pixel from the image.
                    // 0 is fully transparent, and 255 is fully opaque
                    Color acol = original.GetPixel(x, y);

                    if (acol.R == replace.R && acol.G == replace.G && acol.B == replace.B)
                    {
                        acol = Color.FromArgb(acol.A, newcol.R, newcol.G, newcol.B);
                    }
                    newbmp.SetPixel(x, y, acol);
                }
            }

            return newbmp;
        }

        /// <summary>
        /// For debug purposes. This is not very efficient, could be improved.
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="firstRow"></param>
        /// <param name="numRows"></param>
        /// <param name="info"></param>
        public static List<(int x, int y, Color color)> Dump(this Bitmap bmp, int firstRow, int numRows, string info)
        {
            List<(int x, int y, Color color)> res = [];

            for (int y = firstRow; y < firstRow + numRows && y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    // Get the pixel from the image.
                    Color acolor = bmp.GetPixel(x, y);
                    res.Add((x, y, acolor));
                }
            }

            return res;
        }
    }
    #endregion

    #region Performant bitmap replacement
    /// <summary>Fast pixel read/write. Borrowed from https://stackoverflow.com/a/34801225.</summary>
    public sealed class PixelBitmap : IDisposable
    {
        #region Fields
        /// <summary>Unmanaged buffer.</summary>
        readonly int[] _buff;

        /// <summary>Unmanaged buffer handle.</summary>
        GCHandle _hBuff;

        /// <summary>Resource management.</summary>
        bool _disposed = false;
        #endregion

        #region Properties
        /// <summary>Managed image for client consumption.</summary>
        public Bitmap ClientBitmap { get; init; }
        #endregion

        #region Lifecycle
        /// <summary>Normal constructor.</summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public PixelBitmap(int width, int height)
        {
            _buff = new int[width * height];
            _hBuff = GCHandle.Alloc(_buff, GCHandleType.Pinned);
            ClientBitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, _hBuff.AddrOfPinnedObject());
        }

        /// <summary>Override finalizer only if Dispose(bool disposing) has code to free unmanaged resources.</summary>
        ~PixelBitmap()
        {
            Dispose(false);
        }

        /// <summary>Boilerplate.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Boilerplate.</summary>
        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed state/objects).
                    ClientBitmap.Dispose();
                }

                // Release unmanaged resources.
                _hBuff.Free();

                _disposed = true;
            }
        }
        #endregion

        /// <summary>Set one pixel.</summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="clr"></param>
        public void SetPixel(int x, int y, Color clr)
        {
            int index = x + (y * ClientBitmap.Width);
            int col = clr.ToArgb();
            _buff[index] = col;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPixel(int x, int y)
        {
            int index = x + (y * ClientBitmap.Width);
            int col = _buff[index];
            Color result = Color.FromArgb(col);
            return result;
        }
        #endregion
    }
}
