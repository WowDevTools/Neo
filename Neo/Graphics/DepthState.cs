using System;
using SharpDX.Direct3D11;

namespace Neo.Graphics
{
    class DepthState : IDisposable
    {
        private DepthStencilState mState;
        private DepthStencilStateDescription mDescription;
        private bool mChanged;
        private GxContext mContext;

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

        public DepthStencilState Native
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

        ~DepthState()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (mState != null)
            {
                mState.Dispose();
                mState = null;
            }

            mContext = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
