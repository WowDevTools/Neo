using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	public class IndexBuffer : Buffer
    {
        public IndexBuffer(GxContext context) :
            base(context, SharpDX.Direct3D11.BindFlags.IndexBuffer)
        {
	        IndexFormat = DrawElementsType.UnsignedInt;
        }

        public DrawElementsType IndexFormat { get; set; }
    }
}
