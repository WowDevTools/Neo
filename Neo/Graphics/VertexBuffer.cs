
namespace Neo.Graphics
{
	public class VertexBuffer : Buffer
    {
        public VertexBuffer(GxContext context) :
            base(context, SharpDX.Direct3D11.BindFlags.VertexBuffer)
        {

        }
    }
}
