using SharpDX.Direct3D11;

namespace WoWEditor6.Graphics
{
    class Sampler
    {
        private SamplerState mState;
        private SamplerStateDescription mDescription;
        private readonly GxContext mContext;
        private bool mChanged;

        public TextureAddressMode AddressMode
        {
            set
            {
                mDescription.AddressU = mDescription.AddressV = mDescription.AddressW = value;
                mChanged = true;
            }
            get { return mDescription.AddressU; }
        }

        public Filter Filter
        {
            set { mDescription.Filter = value; mChanged = true; }
            get { return mDescription.Filter; }
        }

        public Sampler(GxContext context)
        {
            mContext = context;
            mDescription = new SamplerStateDescription
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                BorderColor = SharpDX.Color4.Black,
                ComparisonFunction = Comparison.Always,
                Filter = Filter.MinMagMipLinear,
                MaximumAnisotropy = 0,
                MaximumLod = float.MaxValue,
                MinimumLod = float.MinValue,
                MipLodBias = 0.0f
            };

            mChanged = true;
        }

        public SamplerState Native
        {
            get
            {
                if (!mChanged) return mState;
                mState?.Dispose();
                mState = new SamplerState(mContext.Device, mDescription);
                mChanged = false;

                return mState;
            }
        }

    }
}
