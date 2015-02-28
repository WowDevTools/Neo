using System;
using SharpDX;
using SharpDX.Direct3D11;

namespace WoWEditor6.Graphics
{
    class TextureArray : IDisposable
    {
        private Texture2D mTexture;
        private ShaderResourceView mView;
        private GxContext mDevice;

        public TextureArray(GxContext context)
        {
            mDevice = context;
        }

        ~TextureArray()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (mTexture != null)
            {
                mTexture.Dispose();
                mTexture = null;
            }

            if (mView != null)
            {
                mView.Dispose();
                mView = null;
            }

            mDevice = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void LoadFromData(SharpDX.DXGI.Format format, int width, int height, int numTextures, int numMips, params DataBox[] datas)
        {
            var textureDesc = new Texture2DDescription
            {
                ArraySize = numTextures,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = format,
                Height = height,
                Width = width,
                MipLevels = numMips,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };

            if (mTexture != null)
                mTexture.Dispose();
            mTexture = new Texture2D(mDevice.Device, textureDesc, datas);

            var srvd = new ShaderResourceViewDescription
            {
                Format = format,
                Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2DArray,
                Texture2DArray = new ShaderResourceViewDescription.Texture2DArrayResource
                {
                    ArraySize = numTextures,
                    FirstArraySlice = 0,
                    MipLevels = -1,
                    MostDetailedMip = 0
                }
            };

            if (mView != null)
                mView.Dispose();

            mView = new ShaderResourceView(mDevice.Device, mTexture, srvd);
        }
    }
}
