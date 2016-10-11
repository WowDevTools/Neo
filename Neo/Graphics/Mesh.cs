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
            VertexBuffer = new VertexBuffer();
            IndexBuffer = new IndexBuffer();

            DepthState = new DepthState();
	        RasterizerState = new RasterState();
            BlendState = new BlendState();
        }

        public void BeginDraw()
        {
	        if (VertexBuffer != null)
	        {
		        VertexBuffer.Bind();
	        }

	        if (IndexBuffer != null)
	        {
		        IndexBuffer.Bind();
	        }

	        if (DepthState != null)
	        {
		        DepthState.Activate();
	        }

	        if (BlendState != null)
	        {
		        BlendState.Activate();
	        }

            Program.Bind();
        }

        public void Draw()
        {
	        GL.DrawElementsInstanced(
		        Topology,
		        IndexCount,
		        IndexBuffer.IndexFormat,
		        new IntPtr(StartIndex * IndexBuffer.IndexFormatSize),
		        1);
        }

        public void Draw(int numInstances)
        {
	        GL.DrawElementsInstanced(
		        Topology,
		        IndexCount,
		        IndexBuffer.IndexFormat,
		        new IntPtr(StartIndex * IndexBuffer.IndexFormatSize),
		        numInstances);
        }

        public void DrawNonIndexed()
        {
	        GL.DrawRangeElements(
		        Topology,
		        StartIndex,
		        StartIndex + IndexCount,
		        IndexCount,
		        IndexBuffer.IndexFormat,
		        new IntPtr(StartIndex * IndexBuffer.IndexFormatSize));
        }

        public void UpdateInstanceBuffer(VertexBuffer buffer)
        {
	        if (InstanceStride == 0 || buffer == null)
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
            BlendState = state;
        }

        public void UpdateRasterizerState(RasterState state)
        {
            RasterizerState = state;
        }

        public void UpdateDepthState(DepthState state)
        {
            DepthState = state;
        }

        public void AddElement(VertexElement element) { mElements.Add(element); }

        public void AddElement(string semantic, int index, int components, DataType type = DataType.Float, bool normalized = false, int slot = 0, bool instanceData = false)
        {
            AddElement(new VertexElement(semantic, index, components, type, normalized, slot, instanceData));
        }
    }
}
