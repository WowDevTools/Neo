using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using WoWEditor6.UI;
using Device = SharpDX.Direct3D11.Device;

namespace WoWEditor6.Graphics
{
    class GxContext
    {
        private readonly Factory1 mFactory;
        private readonly MainWindow mWindow;
        private SwapChainDescription mSwapChainDesc;
        private SwapChain mSwapChain;
        private readonly Adapter1 mAdapter;
        private readonly Output mOutput;
        private RenderTargetView mRenderTarget;
        private DepthStencilView mDepthBuffer;
        private Texture2D mDepthTexture;
        private bool mHasMultisample;

        public Device Device { get; private set; }
        public DeviceContext Context { get; private set; }

        public GxContext(MainWindow window)
        {
            mWindow = window;
            mFactory = new Factory1();
            if (mFactory.Adapters1.Length == 0)
                throw new InvalidOperationException(
                    "Sorry, but DirectX returned that there is no graphics card installed on your system. Please check if all your drivers are up to date!");

            mAdapter = mFactory.GetAdapter1(0);
            if (mAdapter.Outputs.Length == 0)
                throw new InvalidOperationException(
                    "Sorry, but DirectX returned that there is no output (monitor) assigned to the graphics card: \"" +
                    mAdapter.Description1.Description
                    +
                    "\". Please check if your drivers are OK and if your graphics card and monitor show up in the device manager.");

            mOutput = mAdapter.Outputs[0];
        }

        public void BeginFrame()
        {
            Context.ClearRenderTargetView(mRenderTarget, SharpDX.Color4.Black);
        }

        public void EndFrame()
        {
            mSwapChain.Present(0, PresentFlags.None);
        }

        public void InitContext()
        {
            mSwapChainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                Flags = SwapChainFlags.None,
                IsWindowed = true,
                ModeDescription = new ModeDescription(mWindow.ClientSize.Width, mWindow.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                OutputHandle = mWindow.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            mOutput.GetClosestMatchingMode(null, mSwapChainDesc.ModeDescription, out mSwapChainDesc.ModeDescription);
            mSwapChainDesc.ModeDescription.Height = mWindow.ClientSize.Height;
            mSwapChainDesc.ModeDescription.Width = mWindow.ClientSize.Width;

#if DEBUG
            Device = new Device(mAdapter, DeviceCreationFlags.Debug);
#else
            mDevice = new Device(mAdapter);
#endif

            Context = Device.ImmediateContext;

            BuildMultisample();

            mSwapChain = new SwapChain(mFactory, Device, mSwapChainDesc);

            InitRenderTarget();
            InitDepthBuffer();

            Context.OutputMerger.SetRenderTargets(mDepthBuffer, mRenderTarget);
            Context.Rasterizer.SetViewport(new ViewportF(0, 0, mWindow.ClientSize.Width, mWindow.ClientSize.Height,
                0.0f, 1.0f));

            Texture.InitDefaultTexture(this);
        }

        private void BuildMultisample()
        {
            var maxCount = 1;
            var maxQuality = 0;
            for (var i = 0; i <= Device.MultisampleCountMaximum; ++i)
            {
                var quality = Device.CheckMultisampleQualityLevels(Format.R8G8B8A8_UNorm, i);
                if (quality <= 0) continue;

                maxCount = i;
                maxQuality = quality - 1;
            }

            mSwapChainDesc.SampleDescription = new SampleDescription(maxCount, maxQuality);
            mHasMultisample = maxQuality > 0 && maxCount > 1;
        }

        private void InitRenderTarget()
        {
            using (var tex = mSwapChain.GetBackBuffer<Texture2D>(0))
            {
                var rtv = new RenderTargetViewDescription()
                {
                    Dimension = mHasMultisample ? RenderTargetViewDimension.Texture2DMultisampled : RenderTargetViewDimension.Texture2D,
                    Format = Format.R8G8B8A8_UNorm,
                };

                if (mHasMultisample)
                    rtv.Texture2DMS = new RenderTargetViewDescription.Texture2DMultisampledResource();
                else
                    rtv.Texture2D = new RenderTargetViewDescription.Texture2DResource {MipSlice = 0};

                mRenderTarget = new RenderTargetView(Device, tex, rtv);
            }
        }

        private void InitDepthBuffer()
        {
            var texDesc = new Texture2DDescription()
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.D24_UNorm_S8_UInt,
                Height = mWindow.ClientSize.Height,
                Width = mWindow.ClientSize.Width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = mSwapChainDesc.SampleDescription,
                Usage = ResourceUsage.Default
            };

            mDepthTexture = new Texture2D(Device, texDesc);

            var dsvd = new DepthStencilViewDescription()
            {
                Dimension =
                    mHasMultisample
                        ? DepthStencilViewDimension.Texture2DMultisampled
                        : DepthStencilViewDimension.Texture2D,
                Format = Format.D24_UNorm_S8_UInt
            };

            if (mHasMultisample)
                dsvd.Texture2DMS = new DepthStencilViewDescription.Texture2DMultisampledResource();
            else
                dsvd.Texture2D = new DepthStencilViewDescription.Texture2DResource {MipSlice = 0};

            mDepthBuffer = new DepthStencilView(Device, mDepthTexture, dsvd);
        }
    }
}
