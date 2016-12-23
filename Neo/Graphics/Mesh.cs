using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
	// TODO: Activate states before drawing
	// TODO: Bind and draw from buffers
    public class Mesh
    {
        private readonly List<VertexElement> mElements = new List<VertexElement>();

        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }

        public int Stride { get; set; }
        public int InstanceStride { get; set; }
        public int IndexCount { get; set; }

        public int StartIndex { get; set; }
        public int StartVertex { get; set; }

        public DepthState DepthState { get; set; }
        public RasterState RasterizerState { get; set; }
        public BlendState BlendState { get; set; }

        public ShaderProgram Program
        {
	        get;
	        set;
        }

	    public PrimitiveType Topology
	    {
		    get;
		    set;
	    } = PrimitiveType.Triangles;

	    public Mesh()
        {
	        this.VertexBuffer = new VertexBuffer();
	        this.IndexBuffer = new IndexBuffer();

	        this.DepthState = new DepthState();
	        this.RasterizerState = new RasterState();
	        this.BlendState = new BlendState();
        }

        public void BeginDraw()
        {
	        if (this.VertexBuffer != null)
	        {
		        this.VertexBuffer.Bind();
	        }

	        if (this.IndexBuffer != null)
	        {
		        this.IndexBuffer.Bind();
	        }

	        if (this.DepthState != null)
	        {
		        this.DepthState.Activate();
	        }

	        if (this.BlendState != null)
	        {
		        this.BlendState.Activate();
	        }

	        this.Program.Bind();
        }

        public void Draw()
        {
	        GL.DrawElementsInstanced(this.Topology, this.IndexCount, this.IndexBuffer.IndexFormat,
		        new IntPtr(this.StartIndex * this.IndexBuffer.IndexFormatSize),
		        1);
        }

        public void Draw(int numInstances)
        {
	        GL.DrawElementsInstanced(this.Topology, this.IndexCount, this.IndexBuffer.IndexFormat,
		        new IntPtr(this.StartIndex * this.IndexBuffer.IndexFormatSize),
		        numInstances);
        }

        public void DrawNonIndexed()
        {
	        GL.DrawRangeElements(this.Topology, this.StartIndex, this.StartIndex + this.IndexCount, this.IndexCount, this.IndexBuffer.IndexFormat,
		        new IntPtr(this.StartIndex * this.IndexBuffer.IndexFormatSize));
        }

        public void UpdateInstanceBuffer(VertexBuffer buffer)
        {
	        if (this.InstanceStride == 0 || buffer == null)
	        {
		        return;
	        }

	        // TODO: Figure out what this does
            //mContext.Context.InputAssembler.SetVertexBuffers(1, new VertexBufferBinding(buffer.BufferID, InstanceStride, 0));
        }

        public void UpdateIndexBuffer(IndexBuffer indexBuffer)
        {
	        this.IndexBuffer.Dispose();
	        this.IndexBuffer = indexBuffer;
        }

        public void UpdateVertexBuffer(VertexBuffer vertexBuffer)
        {
	        this.VertexBuffer.Dispose();
	        this.VertexBuffer = vertexBuffer;
        }

        public void UpdateBlendState(BlendState state)
        {
	        this.BlendState = state;
        }

        public void UpdateRasterizerState(RasterState state)
        {
	        this.RasterizerState = state;
        }

        public void UpdateDepthState(DepthState state)
        {
	        this.DepthState = state;
        }

        public void AddElement(VertexElement element) {
	        this.mElements.Add(element); }

        public void AddElement(string semantic, int index, int components, DataType type = DataType.Float, bool normalized = false, int slot = 0, bool instanceData = false)
        {
            AddElement(new VertexElement(semantic, index, components, type, normalized, slot, instanceData));
        }
    }
}
