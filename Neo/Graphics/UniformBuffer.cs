using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	public class UniformBuffer : Buffer
    {
        public UniformBuffer()
            : base(BufferTarget.UniformBuffer, BufferUsageHint.StaticDraw)
        {

        }
    }
}
