using System;
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
	    /// The size of one index stored in this buffer.
	    /// </summary>
	    /// <exception cref="ArgumentOutOfRangeException">
	    /// Throws if the <see cref="IndexFormat"/> is set to an invalid format.
	    /// </exception>
	    public int IndexFormatSize
	    {
		    get
		    {
			    switch (IndexFormat)
			    {
				    case DrawElementsType.UnsignedByte:
				    {
					    return 1;
				    }
				    case DrawElementsType.UnsignedShort:
				    {
					    return 2;
				    }
				    case DrawElementsType.UnsignedInt:
				    {
					    return 4;
				    }
				    default:
				    {
					    throw new ArgumentOutOfRangeException();
				    }
			    }
		    }
	    }

	    /// <summary>
		/// Initializes a new instance of the <see cref="IndexBuffer"/> class.
		/// </summary>
		/// <param name="indexFormat">The data type of the indices which will be stored in this buffer.</param>
	    public IndexBuffer(DrawElementsType indexFormat) :
            base(BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw)
	    {
		    IndexFormat = indexFormat;
	    }

	    /// <summary>
	    /// Initializes a new instance of the <see cref="IndexBuffer"/> class.
	    /// </summary>
	    /// <param name="indexFormat">The data type of the indices which will be stored in this buffer.</param>
	    public IndexBuffer() :
		    base(BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw)
	    {
		    IndexFormat = DrawElementsType.UnsignedShort;
	    }
    }
}
