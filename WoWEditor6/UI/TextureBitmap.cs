using System;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using WoWEditor6.IO.Files.Texture;
using WoWEditor6.Utils;

namespace WoWEditor6.UI
{
    class TextureBitmap : IDisposable
    {
        private TextureLoadInfo mLoadInfo;
        private Bitmap mBitmap;

        public bool IsLoaded { get; private set; }

        public int Width { get { return mLoadInfo.Width; } }
        public int Height { get { return mLoadInfo.Height; } }

        public event Action<TextureBitmap> LoadComplete;
        public event Action<TextureBitmap, byte[]> OnBeforeLoad;

        public static implicit operator Bitmap(TextureBitmap bmp)
        {
            return bmp != null ? bmp.mBitmap : null;
        }

        public TextureBitmap()
        {
            mBitmap = new Bitmap(InterfaceManager.Instance.Surface.RenderTarget, new Size2(1, 1), new BitmapProperties
            {
                DpiX = 96.0f,
                DpiY = 96.0f,
                PixelFormat = new PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied)
            });
        }

        public void LoadFromFile(string file)
        {
            Load(file);
        }

        public void Dispose()
        {
            if (mBitmap != null)
                mBitmap.Dispose();
        }

        private async void Load(string file)
        {
            mLoadInfo = await Task<TextureLoadInfo>.Factory.StartNew(() => TextureLoader.LoadFirstLayer(file));
            if (mLoadInfo.Format != Format.R8G8B8A8_UNorm)
                DecompressData();

            if (OnBeforeLoad != null)
                OnBeforeLoad(this, mLoadInfo.Layers[0]);

            InterfaceManager.Instance.Dispatcher.Invoke(OnSyncLoad);
        }

        private void OnSyncLoad()
        {
            if (mBitmap != null)
                mBitmap.Dispose();

            mBitmap = new Bitmap(InterfaceManager.Instance.Surface.RenderTarget, new Size2(mLoadInfo.Width, mLoadInfo.Height), new BitmapProperties()
            {
                DpiX = 96.0f,
                DpiY = 96.0f,
                PixelFormat = new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)
            });

            mBitmap.CopyFromMemory(mLoadInfo.Layers[0], mLoadInfo.Width * 4);
            IsLoaded = true;
            if (LoadComplete != null)
                LoadComplete(this);
        }

        private void DecompressData()
        {
            mLoadInfo.Layers[0] = DxtHelper.Decompress(mLoadInfo.Width, mLoadInfo.Height, mLoadInfo.Layers[0],
            mLoadInfo.Format);
            mLoadInfo.Format = Format.R8G8B8A8_UNorm;
        }
    }
}
