using System;
using SharpDX;
using SharpDX.Direct3D11;

namespace WoWEditor6.Graphics
{
    class RenderTarget : IDisposable
    {
        private GxContext mContext;
        private Texture2D mDepthTexture;
        private DepthStencilView mDepthView;

        private DepthStencilView mOldDepthView;
        private RenderTargetView mOldRenderTarget;

        public RenderTargetView Native { get; private set; }
        public Texture2D Texture { get; private set; }

        public RenderTarget(GxContext context)
        {
            mContext = context;
        }

        ~RenderTarget()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (mDepthView != null)
            {
                mDepthView.Dispose();
                mDepthView = null;
            }

            if (mOldDepthView != null)
            {
                mOldDepthView.Dispose();
                mOldDepthView = null;
            }

            if (mOldRenderTarget != null)
            {
                mOldRenderTarget.Dispose();
                mOldRenderTarget = null;
            }

            if (mDepthTexture != null)
            {
                mDepthTexture.Dispose();
                mDepthTexture = null;
            }

            if (Native != null)
            {
                Native.Dispose();
                Native = null;
            }

            if (Texture != null)
            {
                Texture.Dispose();
                Texture = null;
            }

            mContext = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Clear()
        {
            if (Native == null)
                return;

            mContext.Context.ClearRenderTargetView(Native, Color4.Black);
            mContext.Context.ClearDepthStencilView(mDepthView,
                DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
        }

        public void Apply()
        {
            if (Native == null || mDepthView == null)
                throw new InvalidOperationException("Cannot bind render target before its initialized");

            mOldRenderTarget = mContext.Context.OutputMerger.GetRenderTargets(1, out mOldDepthView)[0];
            mContext.Context.OutputMerger.SetRenderTargets(mDepthView, Native);
        }

        public void Remove()
        {
            if (mOldDepthView != null && mOldRenderTarget != null)
                mContext.Context.OutputMerger.SetRenderTargets(mOldDepthView, mOldRenderTarget);

            if (mOldDepthView != null) mOldDepthView.Dispose();
            if (mOldRenderTarget != null) mOldRenderTarget.Dispose();
        }

        // ReSharper disable once FunctionComplexityOverflow
        public void Resize(int width, int height, bool bgra)
        {
            if (Texture != null)
                Texture.Dispose();

            if (Native != null)
                Native.Dispose();

            var texDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = bgra ? SharpDX.DXGI.Format.B8G8R8A8_UNorm : SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                Height = height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = mContext.Multisampling,
                Usage = ResourceUsage.Default,
                Width = width
            };

            Texture = new Texture2D(mContext.Device, texDesc);

            var rtvd = new RenderTargetViewDescription
            {
                Dimension = RenderTargetViewDimension.Texture2DMultisampled,
                Format = texDesc.Format,

                Texture2DMS = new RenderTargetViewDescription.Texture2DMultisampledResource()
            };

            Native = new RenderTargetView(mContext.Device, Texture, rtvd);

            if (mDepthTexture != null)
                mDepthTexture.Dispose();

            if (mDepthView != null)
                mDepthView.Dispose();

            texDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
                Height = height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = mContext.Multisampling,
                Usage = ResourceUsage.Default,
                Width = width
            };

            mDepthTexture = new Texture2D(mContext.Device, texDesc);

            var dsvd = new DepthStencilViewDescription
            {
                Dimension = DepthStencilViewDimension.Texture2DMultisampled,
                Flags = DepthStencilViewFlags.None,
                Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
                Texture2DMS = new DepthStencilViewDescription.Texture2DMultisampledResource()
            };

            mDepthView = new DepthStencilView(mContext.Device, mDepthTexture, dsvd);
        }
    }
}
