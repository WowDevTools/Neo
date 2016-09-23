using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	public class VertexBuffer : Buffer
    {
        public VertexBuffer() :
            base(BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw)
        {

        }
    }
}
