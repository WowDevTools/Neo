using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	public class IndexBuffer : Buffer
    {
	    public DrawElementsType IndexFormat { get; set; }

	    public IndexBuffer() :
            base(BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw)
        {
	        IndexFormat = DrawElementsType.UnsignedInt;
        }
    }
}
