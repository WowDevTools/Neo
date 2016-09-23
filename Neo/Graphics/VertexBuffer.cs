using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	/// <summary>
	/// The <see cref="VertexBuffer"/> class is a specialized data container class for
	/// vertices on the GPU.
	///
	/// When this object is disposed, the memory occupied by the data is marked as
	/// released on the GPU.
	/// </summary>
	public class VertexBuffer : Buffer
    {
	    /// <summary>
		/// Initializes a new instanece of the <see cref="VertexBuffer"/> class.
		/// </summary>
        public VertexBuffer() :
            base(BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw)
        {

        }
    }
}
