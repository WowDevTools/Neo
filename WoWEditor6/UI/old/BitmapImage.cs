using System;
using System.Drawing;
using System.Drawing.Imaging;
using SharpDX;
using SharpDX.Direct2D1;
using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace WoWEditor6.UI
{
    class BitmapImage : IDisposable
    {
        private Bitmap mBitmap;
        private int mWidth;
        private int mHeight;
        private readonly BitmapProperties mProperties;
        private DataStream mData;
        private bool mChanged;

        public BitmapImage(int width, int height, BitmapProperties properties)
        {
            mWidth = width;
            mHeight = height;
            mProperties = properties;
            mData = new DataStream(width * height * 4, true, true);
        }

        public void UpdateData(uint[] colors)
        {
            if (colors.Length != mWidth * mHeight)
                throw new ArgumentException("Invalid amount of pixels for bitmap");

            lock(mData)
            {
                mData.WriteRange(colors);
                mData.Position = 0;
                mChanged = true;
            }
        }

        public unsafe void UpdateData(Image image)
        {
            if ((image is System.Drawing.Bitmap) == false)
                return;

            lock(mData)
            {
                if(image.Width != mWidth || image.Height != mHeight)
                {
                    if (mBitmap != null)
                        mBitmap.Dispose();
                    mData.Dispose();
                    mData = new DataStream(image.Width * image.Height * 4, true, true);
                    mWidth = image.Width;
                    mHeight = image.Height;

                    mBitmap = new Bitmap(InterfaceManager.Instance.Surface.RenderTarget, new Size2(mWidth, mHeight),
                        mProperties);
                }

                mData.Position = 0;
                var data = new byte[mWidth * mHeight * 4];
                fixed(byte* ptr = data)
                {
                    var bmp = (System.Drawing.Bitmap)image;
                    var bmpd = bmp.LockBits(new System.Drawing.Rectangle(0, 0, mWidth, mHeight),
                        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);


                    UnsafeNativeMethods.CopyMemory(ptr, (byte*)bmpd.Scan0.ToPointer(),
                        mWidth * mHeight * 4);
                    bmp.UnlockBits(bmpd);
                }

                if(mProperties.PixelFormat.AlphaMode == AlphaMode.Premultiplied)
                {
                    for(var i = 0; i < mWidth * mHeight * 4; i += 4)
                    {
                        data[i] = (byte) (data[i] * data[i + 3] / 255.0f);
                        data[i + 1] = (byte) (data[i + 1] * data[i + 3] / 255.0f);
                        data[i + 2] = (byte) (data[i + 2] * data[i + 3] / 255.0f);
                    }
                }

                mBitmap.CopyFromMemory(data, mWidth * 4);
            }
        }

        public Bitmap GetBitmap()
        {
            lock(mData)
            {
                if (mBitmap == null)
                    mBitmap = new Bitmap(InterfaceManager.Instance.Surface.RenderTarget, new Size2(mWidth, mHeight), mProperties);

                if (!mChanged) return mBitmap;

                mBitmap.CopyFromMemory(mData.DataPointer, mWidth * 4);
                mChanged = false;
            }

            return mBitmap;
        }

        public void Dispose()
        {
        }
    }
}
