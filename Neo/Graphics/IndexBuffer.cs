using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	/// <summary>
	/// The <see cref="VertexBuffer"/> class is a specialized data container class for
	/// element indices on the GPU.
	///
	/// When this object is disposed, the memory occupied by the data is marked as
	/// released on the GPU.
	/// </summary>
	public class IndexBuffer : Buffer
    {
	    /// <summary>
		/// The data type of the indices stored in this buffer.
		/// </summary>
	    public DrawElementsType IndexFormat { get; set; }

	    /// <summary>
		/// Initializes a new instance of the <see cref="IndexBuffer"/> class.
		/// </summary>
		/// <param name="indexFormat">The data type of the indices which will be stored in this buffer.</param>
	    public IndexBuffer(DrawElementsType indexFormat) :
            base(BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw)
	    {
		    IndexFormat = indexFormat;
	    }
    }
}
