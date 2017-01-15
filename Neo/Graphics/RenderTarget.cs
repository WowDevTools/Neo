using System;
using OpenTK.Graphics;

namespace Neo.Graphics
{
	internal class RenderTarget : IDisposable
    {
        private Texture2D mDepthTexture;
        private DepthStencilView mDepthView;

        private DepthStencilView mOldDepthView;
        private RenderTargetView mOldRenderTarget;

        public RenderTargetView Native { get; private set; }
        public Texture2D Texture { get; private set; }

        private Color4 backgroundcolor = Color4.Black;
        private Func<System.Drawing.Color, Color4> ConvertColor = c => new Color4(c.R / 255f, c.G / 255f, c.B / 255f, 1);

        public RenderTarget()
        {
	        this.backgroundcolor = this.ConvertColor(Properties.Settings.Default.AssetRenderBackgroundColor);

            Properties.Settings.Default.SettingChanging += SettingsChanging;
        }

        ~RenderTarget()
        {
            Dispose(false);
        }

        private void SettingsChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            if (e.SettingName == "AssetRenderBackgroundColor")
            {
	            this.backgroundcolor = this.ConvertColor((System.Drawing.Color)e.NewValue);
            }
        }

        private void Dispose(bool disposing)
        {
            if (this.mDepthView != null)
            {
	            this.mDepthView.Dispose();
	            this.mDepthView = null;
            }

            if (this.mOldDepthView != null)
            {
	            this.mOldDepthView.Dispose();
	            this.mOldDepthView = null;
            }

            if (this.mOldRenderTarget != null)
            {
	            this.mOldRenderTarget.Dispose();
	            this.mOldRenderTarget = null;
            }

            if (this.mDepthTexture != null)
            {
	            this.mDepthTexture.Dispose();
	            this.mDepthTexture = null;
            }

            if (this.Native != null)
            {
	            this.Native.Dispose();
	            this.Native = null;
            }

            if (this.Texture != null)
            {
	            this.Texture.Dispose();
	            this.Texture = null;
            }

        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Clear()
        {
	        if (this.Native == null)
	        {
		        return;
	        }

	        // TODO: Figure out the purpose of this
            //mContext.Context.ClearRenderTargetView(Native, backgroundcolor);
            //mContext.Context.ClearDepthStencilView(mDepthView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
        }

        public void Apply()
        {
	        if (this.Native == null || this.mDepthView == null)
	        {
		        throw new InvalidOperationException("Cannot bind render target before its initialized");
	        }

	        // TODO: Figure out the purpose of this
	        //mOldRenderTarget = mContext.Context.OutputMerger.GetRenderTargets(1, out mOldDepthView)[0];
            //mContext.Context.OutputMerger.SetRenderTargets(mDepthView, Native);
        }

        public void Remove()
        {
	        if (this.mOldDepthView != null && this.mOldRenderTarget != null)
	        {
		        // TODO: Figure out the purpose of this
		        //mContext.Context.OutputMerger.SetRenderTargets(mOldDepthView, mOldRenderTarget);
	        }

            if (this.mOldDepthView != null)
            {
	            this.mOldDepthView.Dispose();
            }
	        if (this.mOldRenderTarget != null)
	        {
		        this.mOldRenderTarget.Dispose();
	        }
        }

        // ReSharper disable once FunctionComplexityOverflow
        public void Resize(int width, int height, bool bgra)
        {
            if (this.Texture != null)
            {
	            this.Texture.Dispose();
            }

	        if (this.Native != null)
	        {
		        this.Native.Dispose();
	        }

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

	        this.Texture = new Texture2D(mContext.Device, texDesc);

            var rtvd = new RenderTargetViewDescription
            {
                Dimension = RenderTargetViewDimension.Texture2DMultisampled,
                Format = texDesc.Format,

                Texture2DMS = new RenderTargetViewDescription.Texture2DMultisampledResource()
            };

	        this.Native = new RenderTargetView(mContext.Device, this.Texture, rtvd);

            if (this.mDepthTexture != null)
            {
	            this.mDepthTexture.Dispose();
            }

	        if (this.mDepthView != null)
	        {
		        this.mDepthView.Dispose();
	        }

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

	        this.mDepthTexture = new Texture2D(mContext.Device, texDesc);

            var dsvd = new DepthStencilViewDescription
            {
                Dimension = DepthStencilViewDimension.Texture2DMultisampled,
                Flags = DepthStencilViewFlags.None,
                Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
                Texture2DMS = new DepthStencilViewDescription.Texture2DMultisampledResource()
            };

	        this.mDepthView = new DepthStencilView(mContext.Device, this.mDepthTexture, dsvd);
        }
    }
}
