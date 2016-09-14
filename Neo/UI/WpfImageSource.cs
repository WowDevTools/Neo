using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Neo.IO.Files.Texture;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Neo.UI
{
    static class WpfImageSource
    {
        private static readonly BitmapSource ErrorBitmap;

        public unsafe static BitmapSource FromBgra(int width, int height, uint[] colors)
        {
            fixed (uint* ptr = colors)
                return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, new IntPtr(ptr), colors.Length * 4, width * 4);
        }

        public static BitmapSource FromBgra(int width, int height, byte[] colors)
        {
            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, colors, width * 4);
        }

        public static BitmapSource FromTexture(string fileName)
        {
            var texLoadInfo = TextureLoader.LoadToArgbImage(fileName);
            return texLoadInfo == null ? ErrorBitmap : FromBgra(texLoadInfo.Width, texLoadInfo.Height, texLoadInfo.Layers[0]);
        }

        public static BitmapSource FromTexture(string file, int width, int height)
        {
            var texLoadInfo = TextureLoader.LoadToBestMatchingImage(file, width, height);
            return texLoadInfo == null ? ErrorBitmap : FromBgra(width, height, texLoadInfo.Layers[0]);
        }

        public static BitmapSource FromGdiImage(Bitmap bmp)
        {
            var bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var ret = BitmapSource.Create(bmp.Width, bmp.Height, 96, 96, PixelFormats.Bgra32, null, bmpd.Scan0, bmp.Width * bmp.Height * 4, bmp.Width * 4);
            bmp.UnlockBits(bmpd);

            return ret;
        }

        static WpfImageSource()
        {
            var bmp = new Bitmap(200, 200, PixelFormat.Format32bppArgb);
            var g = System.Drawing.Graphics.FromImage(bmp);
            var font = new Font("Segoe UI", 26);
            g.Clear(System.Drawing.Color.Black);
            var len = g.MeasureString("Invalid image", font);
            g.DrawString("Invalid Image", font, System.Drawing.Brushes.Red, new PointF(100 - len.Width / 2.0f, 100 - len.Height / 2.0f));
            font.Dispose();
            g.Flush();
            g.Dispose();

            var bmpd = bmp.LockBits(new Rectangle(0, 0, 200, 200), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            ErrorBitmap = BitmapSource.Create(200, 200, 96, 96, PixelFormats.Bgra32, null, bmpd.Scan0, 200*200*4, 200*4);
            bmp.UnlockBits(bmpd);
            bmp.Dispose();
        }
    }
}
