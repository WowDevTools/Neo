using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	/// <summary>
	/// The <see cref="VertexBuffer"/> class is a specialized data container class for
	/// uniform constant values which are passed to the shader programs on the GPU.
	///
	/// When this object is disposed, the memory occupied by the data is marked as
	/// released on the GPU.
	/// </summary>
	public class UniformBuffer : Buffer
    {
	    /// <summary>
		/// Initializes a new instance of the <see cref="UniformBuffer"/> class.
		/// </summary>
        public UniformBuffer()
            : base(BufferTarget.UniformBuffer, BufferUsageHint.StaticDraw)
        {

        }
    }
}
