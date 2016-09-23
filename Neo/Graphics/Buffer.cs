using System;
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
        //private BufferDescription mDescription;
        //private GxContext mContext;
        //public SharpDX.Direct3D11.Buffer BufferID { get; private set; }

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
	    /// Binds this buffer object, setting it as the current object to be operated on in the
	    /// scope of the OpenGL pipeline.
	    /// </summary>
	    public void Bind()
	    {
		    GL.BindBuffer(BufferType, this.glBufferID);
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

	    [Obsolete]
        public void UpdateData<T>(T data) where T : struct
        {
            //Resize(IO.SizeCache<T>.Size, data);
        }

	    [Obsolete]
        public void UpdateData<T>(T[] data) where T : struct
        {
            //Resize(data.Length * IO.SizeCache<T>.Size, data);
        }

	    /*
        private void Resize<T>(int length, T value) where T : struct
        {
            if (length > mDescription.SizeInBytes)
            {
                mDescription.SizeInBytes = length;
                if (BufferID != null)
                    BufferID.Dispose();

                using (var strm = new DataStream(length, true, true))
                {
                    strm.Write(value);
                    strm.Position = 0;
                    BufferID = new SharpDX.Direct3D11.Buffer(mContext.Device, strm, mDescription);
                }
            }
            else
                mContext.Context.UpdateSubresource(ref value, BufferID);
        }

        private void Resize<T>(int length, T[] data) where T : struct
        {
            if (length > mDescription.SizeInBytes)
            {
                mDescription.SizeInBytes = length;
                if (BufferID != null)
                    BufferID.Dispose();

                if (data != null)
                {
                    using (var strm = new DataStream(length, true, true))
                    {
                        strm.WriteRange(data);
                        strm.Position = 0;
                        BufferID = new SharpDX.Direct3D11.Buffer(mContext.Device, strm, mDescription);
                    }
                }
                else
                    BufferID = new SharpDX.Direct3D11.Buffer(mContext.Device, mDescription);
            }
            else
                mContext.Context.UpdateSubresource(data, BufferID);
        }
		*/
    }
}
