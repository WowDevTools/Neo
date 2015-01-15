using SharpDX;
using SharpDX.Direct3D11;

namespace WoWEditor6.Graphics
{
    abstract class Buffer
    {
        private BufferDescription mDescription;
        private readonly GxContext mContext;

        public SharpDX.Direct3D11.Buffer Native { get; protected set; }

        protected Buffer(GxContext context, BindFlags binding)
        {
            mContext = context;

            mDescription = new BufferDescription
            {
                BindFlags = binding,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 0,
                StructureByteStride = 0,
                Usage = ResourceUsage.Default
            };
        }

        public void UpdateData<T>(T[] data) where T : struct
        {
            Resize(data.Length, data);
        }

        private void Resize<T>(int length, T[] data) where T : struct
        {
            if (length > mDescription.SizeInBytes)
            {
                mDescription.SizeInBytes = length;
                Native?.Dispose();
                if (data != null)
                {
                    using (var strm = new DataStream(length, true, true))
                    {
                        strm.WriteRange(data);
                        Native = new SharpDX.Direct3D11.Buffer(mContext.Device, strm, mDescription);
                    }
                }
                else
                    Native = new SharpDX.Direct3D11.Buffer(mContext.Device, mDescription);
            }
            else
                mContext.Context.UpdateSubresource(data, Native);
        }
    }
}
