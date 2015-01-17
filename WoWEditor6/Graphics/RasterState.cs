using SharpDX.Direct3D11;

namespace WoWEditor6.Graphics
{
    class RasterState
    {
        private RasterizerState mState;
        private RasterizerStateDescription mDescription;
        private bool mChanged;
        private readonly GxContext mContext;

        public RasterizerState Native
        {
            get
            {
                if (!mChanged) return mState;

                mState?.Dispose();
                mState = new RasterizerState(mContext.Device, mDescription);
                mChanged = false;

                return mState;
            }
        }
        public RasterState(GxContext context)
        {
            mContext = context;
            mDescription = new RasterizerStateDescription
            {
                CullMode = CullMode.None,
                DepthBias = 0,
                DepthBiasClamp = 0.0f,
                FillMode = FillMode.Solid,
                IsAntialiasedLineEnabled = false,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = false,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0.0f
            };

            mChanged = true;
        }
    }
}
