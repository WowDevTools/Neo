using SharpDX.DXGI;

namespace Neo.Graphics
{
	public class IndexBuffer : Buffer
    {
        public IndexBuffer(GxContext context) :
            base(context, SharpDX.Direct3D11.BindFlags.IndexBuffer)
        {
            IndexFormat = Format.R16_UInt;
        }

        public Format IndexFormat { get; set; }
    }
}
