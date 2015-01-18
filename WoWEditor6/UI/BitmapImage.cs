using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct2D1;

namespace WoWEditor6.UI
{
    class BitmapImage : IDisposable
    {
        private static readonly List<BitmapImage> Images = new List<BitmapImage>();

        private Bitmap mBitmap;
        private readonly int mWidth;
        private readonly int mHeight;
        private readonly BitmapProperties mProperties;
        private readonly DataStream mData;
        private bool mChanged;

        public BitmapImage(int width, int height, BitmapProperties properties)
        {
            mWidth = width;
            mHeight = height;
            mProperties = properties;
            mData = new DataStream(width * height * 4, true, true);
            lock (Images)
                Images.Add(this);
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
            mBitmap?.Dispose();
            lock (Images)
                Images.Remove(this);
        }

        private void OnRenderTargetChanged()
        {
            lock (mData)
            {
                mChanged = true;
                mBitmap?.Dispose();
                mBitmap = null;
            }
        }

        public static void UpdateRenderTarget()
        {
            lock(Images)
            {
                foreach (var bmp in Images)
                    bmp.OnRenderTargetChanged();
            }
        }
    }
}
