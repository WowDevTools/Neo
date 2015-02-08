using System;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using WoWEditor6.Graphics;
using WoWEditor6.UI.Components;
using Device1 = SharpDX.Direct3D10.Device1;
using Factory = SharpDX.Direct2D1.Factory;

namespace WoWEditor6.UI
{
    class DrawSurface
    {
        private Texture2D mTmpTexture;
        private SharpDX.Direct3D11.Texture2D mRealTexture;
        private readonly GxContext mDevice;
        private KeyedMutex mMutex10, mMutex11;

        private const long Key11 = 0xFF110000;

        public SharpDX.Direct3D11.ShaderResourceView NativeView { get; private set; }
        public SharpDX.DirectWrite.Factory DirectWriteFactory { get; private set; }
        public RenderTarget RenderTarget { get; private set; }
        public Factory Direct2DFactory { get; private set; }
        public Device1 D2DDevice { get; private set; }

        public DrawSurface(GxContext context)
        {
            DirectWriteFactory = new SharpDX.DirectWrite.Factory(SharpDX.DirectWrite.FactoryType.Isolated);
            Direct2DFactory = new Factory();
            mDevice = context;
        }

        public void GraphicsInit()
        {
            D2DDevice = new Device1(mDevice.Adapter, DeviceCreationFlags.BgraSupport,
                SharpDX.Direct3D10.FeatureLevel.Level_10_1);
        }

        public void OnResize(int width, int height)
        {
            if (mRealTexture != null)
                mRealTexture.Dispose();
            if (mTmpTexture != null)
                mTmpTexture.Dispose();

            mRealTexture = new SharpDX.Direct3D11.Texture2D(mDevice.Device, new SharpDX.Direct3D11.Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = SharpDX.Direct3D11.BindFlags.RenderTarget | SharpDX.Direct3D11.BindFlags.ShaderResource,
                CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Height = height,
                Width = width,
                MipLevels = 1,
                OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.SharedKeyedmutex,
                SampleDescription = new SampleDescription(1, 0),
                Usage = SharpDX.Direct3D11.ResourceUsage.Default
            });

            using (var resource = mRealTexture.QueryInterface<SharpDX.DXGI.Resource>())
                mTmpTexture = D2DDevice.OpenSharedResource<Texture2D>(resource.SharedHandle);

            if (NativeView != null)
                NativeView.Dispose();
            NativeView = new SharpDX.Direct3D11.ShaderResourceView(mDevice.Device, mRealTexture,
                new SharpDX.Direct3D11.ShaderResourceViewDescription
                {
                    Format = Format.B8G8R8A8_UNorm,
                    Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                    Texture2D = new SharpDX.Direct3D11.ShaderResourceViewDescription.Texture2DResource
                    {
                        MipLevels = 1,
                        MostDetailedMip = 0
                    }
                });

            if (RenderTarget != null)
                RenderTarget.Dispose();
            using (var surface = mTmpTexture.QueryInterface<Surface>())
                RenderTarget = new RenderTarget(Direct2DFactory, surface, new RenderTargetProperties()
                {
                    DpiX = 0.0f,
                    DpiY = 0.0f,
                    MinLevel = SharpDX.Direct2D1.FeatureLevel.Level_DEFAULT,
                    PixelFormat = new PixelFormat() { AlphaMode = AlphaMode.Premultiplied, Format = Format.Unknown },
                    Type = RenderTargetType.Hardware,
                    Usage = RenderTargetUsage.None
                });

            if (mMutex10 != null)
                mMutex10.Dispose();
            if (mMutex11 != null)
                mMutex11.Dispose();

            mMutex10 = mTmpTexture.QueryInterface<KeyedMutex>();
            mMutex11 = mRealTexture.QueryInterface<KeyedMutex>();

            Brushes.Initialize(RenderTarget);
            Fonts.Initialize(DirectWriteFactory);

            Button.Initialize();
            Frame.Initialize();

            // right now the texture is unowned and only a key of 0 will succeed.
            // after releasing it with a specific key said key then can be used for
            // further locking.
            mMutex10.Acquire(0, -1);
            mMutex10.Release(Key11);
        }

        public void RenderFrame(Action<RenderTarget> renderAction)
        {
            mMutex10.Acquire(Key11, -1);

            try
            {
                RenderTarget.BeginDraw();
                RenderTarget.Clear(new Color4(0, 0, 0, 0));
                if (renderAction != null)
                    renderAction(RenderTarget);
                RenderTarget.EndDraw();
            }
            finally
            {
                // we absolutely need to release the old lock and acquire the new one
                // or the CPU will stall when we return
                mMutex10.Release(Key11);
                mMutex11.Acquire(Key11, -1);
            }

        }

        public void EndFrame()
        {
            mMutex11.Release(Key11);
        }
    }
}
