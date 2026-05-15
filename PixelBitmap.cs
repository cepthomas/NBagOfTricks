using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;


namespace Ephemera.NBagOfTricks
{
    /// <summary>Fast pixel read/write. Some borrowed from https://stackoverflow.com/a/34801225.</summary>
    public sealed class PixelBitmap : IDisposable
    {
        #region Fields
        /// <summary>Unmanaged buffer.</summary>
        readonly int[] _buff;

        /// <summary>Geometry.</summary>
        readonly int _width;

        /// <summary>Geometry.</summary>
        readonly int _height;

        /// <summary>Unmanaged buffer handle.</summary>
        GCHandle _hBuff;

        /// <summary>Resource management.</summary>
        bool _disposed = false;
        #endregion

        #region Lifecycle
        /// <summary>Normal constructor.</summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public PixelBitmap(int width, int height)
        {
            _width = width;
            _height = height;
            _buff = new int[width * height];
            _hBuff = GCHandle.Alloc(_buff, GCHandleType.Pinned);
        }

        /// <summary>Constructor from GDI Bitmap.</summary>
        /// <param name="bmp"></param>
        public PixelBitmap(Bitmap bmp) : this(bmp.Width, bmp.Height)
        {
            // Lock the source.
            BitmapData bmpData = bmp.LockBits(new(0, 0, _width, _height), ImageLockMode.ReadOnly, bmp.PixelFormat);

            // Copy the RGB values into the alloc array.
            Marshal.Copy(bmpData.Scan0, _buff, 0, _buff.Length);

            // Unlock the source.
            bmp.UnlockBits(bmpData);
        }

        /// <summary>Constructor from graphics file.</summary>
        /// <param name="fn">Filename</param>
        public PixelBitmap(string fn) : this((Bitmap)Bitmap.FromFile(fn))
        {
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
                }

                // Release unmanaged resources.
                _hBuff.Free();

                _disposed = true;
            }
        }
        #endregion

        #region API
        /// <summary>Set one pixel.</summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="clr"></param>
        public void SetPixel(int x, int y, Color clr)
        {
            int index = x + (y * _width);
            int col = clr.ToArgb();
            _buff[index] = col;
        }

        /// <summary>Get a pixel.</summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPixel(int x, int y)
        {
            int index = x + (y * _width);
            int col = _buff[index];
            Color result = Color.FromArgb(col);
            return result;
        }

        /// <summary>Get GDI Bitmap of the buffer. Client must manage lifetime.</summary>
        /// <returns></returns>
        public Bitmap GetBitmap()
        {
            var bmp = new Bitmap(_width, _height, _width * 4, PixelFormat.Format32bppArgb, _hBuff.AddrOfPinnedObject());
            return bmp;
        }

        /// <summary>Colorize a monochrome bitmap.</summary>
        /// <param name="newcol"></param>
        /// <param name="toReplace">Optional source color to replace. Defaults to black.</param>
        public void Colorize(Color newcol, Color toReplace = default)
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    // Get the pixel from the image.
                    Color acol = GetPixel(x, y);
                    if (acol.R == toReplace.R && acol.G == toReplace.G && acol.B == toReplace.B)
                    {
                        SetPixel(x, y, Color.FromArgb(acol.A, newcol.R, newcol.G, newcol.B));
                    }
                }
            }
        }

        /// <summary>De-colorize.</summary>
        public void ConvertToGrayscale()
        {
            // Conversion algo (worst to best)
            // - Simple averaging.
            // - channel-dependent luminance perception
            //   Y = R * 0.2126 + G * 0.7152 + B * 0.0722; // 0.0 to 255.0
            // - gamma-compression-corrected approximation:
            //   Y = 0.299 R + 0.587 G + 0.114 B
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    // Get the pixel from the image. 0 is fully transparent, and 255 is fully opaque
                    Color acol = GetPixel(x, y);
                    var lum = (int)(acol.R * 0.299 + acol.G * 0.587 + acol.B * 0.114);
                    SetPixel(x, y, Color.FromArgb(acol.A, lum, lum, lum));
                }
            }
        }

        /// <summary>For debug purposes.</summary>
        /// <param name="firstRow"></param>
        /// <param name="numRows"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public List<(int x, int y, Color color)> Dump(int firstRow, int numRows, string info)
        {
            List<(int x, int y, Color color)> res = [];

            for (int y = firstRow; y < firstRow + numRows && y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    // Get the pixel from the image.
                    Color acolor = GetPixel(x, y);
                    res.Add((x, y, acolor));
                }
            }

            return res;
        }
        #endregion
    }
}
