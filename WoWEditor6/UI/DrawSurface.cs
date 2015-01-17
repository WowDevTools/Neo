using System;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D10;
using SharpDX.DXGI;
using WoWEditor6.Graphics;
using Device1 = SharpDX.Direct3D10.Device1;
using Factory = SharpDX.Direct2D1.Factory;

namespace WoWEditor6.UI
{
    class DrawSurface
    {
        private RenderTarget mRenderTarget;
        private readonly Factory mFactory = new Factory();
        private Device1 mTmpDevice;
        private Texture2D mTmpTexture;
        private SharpDX.Direct3D11.Texture2D mRealTexture;
        private readonly GxContext mDevice;
        private KeyedMutex mMutex10, mMutex11;

        private readonly SharpDX.DirectWrite.Factory mDwFactory =
            new SharpDX.DirectWrite.Factory(SharpDX.DirectWrite.FactoryType.Isolated);

        private const long Key11 = 0xFF110000;

        public SharpDX.Direct3D11.ShaderResourceView NativeView { get; private set; }

        public DrawSurface(GxContext context)
        {
            mDevice = context;
        }

        public void GraphicsInit()
        {
            mTmpDevice = new Device1(mDevice.Adapter, DeviceCreationFlags.BgraSupport,
                SharpDX.Direct3D10.FeatureLevel.Level_10_1);
        }

        public void OnResize(int width, int height)
        {
            mRealTexture?.Dispose();
            mTmpTexture?.Dispose();

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
                mTmpTexture = mTmpDevice.OpenSharedResource<Texture2D>(resource.SharedHandle);

            NativeView?.Dispose();
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

            mRenderTarget?.Dispose();
            using (var surface = mTmpTexture.QueryInterface<Surface>())
                mRenderTarget = new RenderTarget(mFactory, surface, new RenderTargetProperties()
                {
                    DpiX = 0.0f,
                    DpiY = 0.0f,
                    MinLevel = SharpDX.Direct2D1.FeatureLevel.Level_DEFAULT,
                    PixelFormat = new PixelFormat() { AlphaMode = AlphaMode.Premultiplied, Format = Format.Unknown },
                    Type = RenderTargetType.Hardware,
                    Usage = RenderTargetUsage.None
                });

            mMutex10?.Dispose();
            mMutex11?.Dispose();

            mMutex10 = mTmpTexture.QueryInterface<KeyedMutex>();
            mMutex11 = mRealTexture.QueryInterface<KeyedMutex>();

            Brushes.Initialize(mRenderTarget);
            Fonts.Initialize(mDwFactory);

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
                mRenderTarget.BeginDraw();
                mRenderTarget.Clear(new Color4(0, 0, 0, 0));
                mRenderTarget.DrawText("TEST", Fonts.Cache["Segoe UI", 18.0f, SharpDX.DirectWrite.FontWeight.Bold], new RectangleF(30, 30, 300, 50),
                    Brushes.Solid[System.Drawing.Color.Aqua]);
                renderAction?.Invoke(mRenderTarget);
                mRenderTarget.EndDraw();
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
