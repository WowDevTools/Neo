using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	/// <summary>
	/// The <see cref="Buffer"/> class acts as a wrapper around an OpenGL data buffer
	/// with arbitrary values, and serves to simplyfy management of these buffers.
	///
	/// When this object is disposed, the memory occupied by the data is marked as
	/// released on the GPU.
	/// </summary>
    public abstract class Buffer : IDisposable
    {
	    /// <summary>
		/// The native handle on the GPU which refers to the buffer data held by this object.
		/// </summary>
	    public int glBufferID
	    {
		    get;
		    private set;
	    }

	    /// <summary>
		/// The type of buffer this is. By default, the buffer is assumed
		/// to be an <see cref="BufferTarget.ArrayBuffer"/>.
		/// </summary>
	    public BufferTarget BufferType
	    {
		    get;
		    private set;
	    }

	    /// <summary>
		/// The intended use for the buffer. This is used to hint the OpenGL driver as to how
		/// to pack and manipulate the buffer, improving performance.
		/// </summary>
	    public BufferUsageHint BufferUsage
	    {
		    get;
		    private set;
	    }

	    /// <summary>
		/// Creates a new instance of the <see cref="Buffer"/> class, generating a buffer object
		/// on the GPU in the process.
		/// </summary>
		/// <param name="bufferType">The type of buffer to be created.</param>
		/// <param name="bufferUsageHint">
		/// The hinted usage of the buffer.
		/// Note that this is not a binding promise, merely a hint to the underlying driver.
		/// </param>
        protected Buffer(BufferTarget bufferType, BufferUsageHint bufferUsageHint)
        {
	        this.glBufferID = GL.GenBuffer();
	        this.BufferType = bufferType;
	        this.BufferUsage = bufferUsageHint;
        }

	    ~Buffer()
        {
            Dispose(false);
        }

	    /// <summary>
		/// Buffers an array of data into graphics memory. If the buffer already contains a
		/// set of data, the old data is deleted and replaced with the new data.
		/// </summary>
		/// <param name="data">An array of data values.</param>
		/// <typeparam name="T">The type of the data to buffer.</typeparam>
	    public void BufferData<T>(T[] data) where T : struct
	    {
		    if (data == null)
		    {
			    throw new ArgumentNullException(nameof(data), $"The \"{nameof(data)}\" argument was passed with a null value.");
		    }

		    if (data.Length <= 0)
		    {
			    throw new ArgumentException($"The {nameof(data)} array was empty.", nameof(data));
		    }

		    Bind();

		    int dataSize = 0;
		    foreach (T dataElement in data)
		    {
			    dataSize += Marshal.SizeOf(dataElement);
		    }

		    GL.BufferData(this.BufferType, (IntPtr)dataSize, data, this.BufferUsage);
	    }

	    /// <summary>
	    /// Buffers a structure of data into graphics memory. If the buffer already contains a
	    /// set of data, the old data is deleted and replaced with the new data.
	    /// </summary>
	    /// <param name="data">A structure containing the data..</param>
	    /// <typeparam name="T">The type of the data to buffer.</typeparam>
	    public void BufferData<T>(T data) where T : struct
	    {
		    Bind();

		    GL.BufferData(this.BufferType, (IntPtr)Marshal.SizeOf(data), new []{data}, this.BufferUsage);
	    }

	    /// <summary>
	    /// Binds this buffer object, setting it as the current object to be operated on in the
	    /// current thread context.
	    /// </summary>
	    public void Bind()
	    {
		    GL.BindBuffer(this.BufferType, this.glBufferID);
	    }

	    private void Dispose(bool disposing)
	    {
		    if (this.glBufferID > 0)
		    {
			    GL.DeleteBuffer(this.glBufferID);
		    }
	    }

	    public virtual void Dispose()
	    {
		    Dispose(true);
		    GC.SuppressFinalize(this);
	    }
    }
}
