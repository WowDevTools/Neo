using SharpDX.Direct3D11;

namespace WoWEditor6.Graphics
{
    class BlendState
    {
        private SharpDX.Direct3D11.BlendState mState;
        private BlendStateDescription mDescription;
        private bool mChanged;
        private readonly GxContext mContext;

        public bool BlendEnabled
        {
            get { return mDescription.RenderTarget[0].IsBlendEnabled; }
            set { mDescription.RenderTarget[0].IsBlendEnabled = value; mChanged = true; }
        }

        public SharpDX.Direct3D11.BlendState Native
        {
            get
            {
                if (mChanged != true) return mState;

                mState?.Dispose();
                mState = new SharpDX.Direct3D11.BlendState(mContext.Device, mDescription);
                mChanged = false;

                return mState;
            }
        }

        public BlendState(GxContext context)
        {
            mContext = context;
            mDescription = new BlendStateDescription
            {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false
            };

            mDescription.RenderTarget[0] = new RenderTargetBlendDescription
            {
                IsBlendEnabled = true,
                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                BlendOperation = BlendOperation.Add,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.Zero,
                AlphaBlendOperation = BlendOperation.Add,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };

            mChanged = true;
        }
    }
}
