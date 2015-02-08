using SharpDX.Direct3D11;

namespace WoWEditor6.Graphics
{
    class DepthState
    {
        private DepthStencilState mState;
        private DepthStencilStateDescription mDescription;
        private bool mChanged;
        private readonly GxContext mContext;

        public bool DepthEnabled
        {
            get { return mDescription.IsDepthEnabled; }
            set
            {
                mDescription.IsDepthEnabled = value;
                mDescription.IsStencilEnabled = value;
                mChanged = true;
            }
        }

        public bool DepthWriteEnabled
        {
            get { return mDescription.DepthWriteMask == DepthWriteMask.All; }
            set
            {
                mDescription.DepthWriteMask = value ? DepthWriteMask.All : DepthWriteMask.Zero;
                mChanged = true;
            }
        }

        public DepthState(GxContext context)
        {
            mContext = context;

            mDescription = new DepthStencilStateDescription
            {
                BackFace = new DepthStencilOperationDescription
                {
                    Comparison = Comparison.Always,
                    DepthFailOperation = StencilOperation.Decrement,
                    FailOperation = StencilOperation.Keep,
                    PassOperation = StencilOperation.Keep
                },

                FrontFace = new DepthStencilOperationDescription
                {
                    Comparison = Comparison.Always,
                    PassOperation = StencilOperation.Keep,
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Increment
                },

                DepthComparison = Comparison.LessEqual,
                DepthWriteMask = DepthWriteMask.All,
                IsDepthEnabled = true,
                IsStencilEnabled = true,
                StencilReadMask = 0xFF,
                StencilWriteMask = 0xFF
            };

            mChanged = true;
        }

        public DepthStencilState State
        {
            get
            {
                if (!mChanged) return mState;
                if (mState != null)
                    mState.Dispose();
                mState = new DepthStencilState(mContext.Device, mDescription);
                mChanged = false;
                return mState;
            }
        }
    }
}
