using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Ephemera.NBagOfTricks
{
    /// <summary>
    /// Fast pixel read/write. Borrowed from https://stackoverflow.com/a/34801225.
    /// </summary>
    public sealed class PixelBitmap : IDisposable
    {
        #region Fields
        /// <summary>Unmanaged buffer.</summary>
        readonly Int32[] _buff;

        /// <summary>Unmanaged buffer handle.</summary>
        GCHandle _hBuff;

        /// <summary>Resource management.</summary>
        bool _disposed = false;
        #endregion

        #region Properties
        /// <summary>Managed image for client consumption.</summary>
        public Bitmap Bitmap { get; init; }
        #endregion

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public PixelBitmap(int width, int height)
        {
            _buff = new Int32[width * height];
            _hBuff = GCHandle.Alloc(_buff, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, _hBuff.AddrOfPinnedObject());
        }

        /// <summary>
        /// Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources.
        /// </summary>
        ~PixelBitmap()
        {
            Dispose(false);
        }

        /// <summary>
        /// Boilerplate.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Boilerplate.
        /// </summary>
        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // called via myClass.Dispose(). 
                    // OK to use any private object references
                    // Dispose managed state (managed objects).
                    Bitmap.Dispose();
                }

                // Release unmanaged resources.
                _hBuff.Free();

                _disposed = true;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="colour"></param>
        public void SetPixel(int x, int y, Color colour)
        {
            int index = x + (y * Bitmap.Width);
            int col = colour.ToArgb();

            _buff[index] = col;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void SetPixel(int x, int y, int a, int r, int g, int b)
        {
            // Check args!
            // The byte-ordering of the 32-bit ARGB value is AARRGGBB. The most significant byte (MSB), represented by AA, is the alpha component value.
            int index = x + (y * Bitmap.Width);
            int hcol = (byte)a << 24 | (byte)r << 16 | (byte)g << 8 | (byte)b;
            _buff[index] = hcol;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPixel(int x, int y)
        {
            int index = x + (y * Bitmap.Width);
            int col = _buff[index];
            Color result = Color.FromArgb(col);

            return result;
        }
    }
}
