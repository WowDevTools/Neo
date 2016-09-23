using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace Neo.Graphics
{
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

	    public BeginMode Topology
	    {
		    get;
		    set;
	    } = BeginMode.Triangles;

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
            var ctx = mContext.Context;
            if (VertexBuffer != null)
                ctx.InputAssembler.SetVertexBuffers(0, new[] {VertexBuffer.BufferID}, new[] {Stride}, new[] {0});

            if (IndexBuffer != null)
                ctx.InputAssembler.SetIndexBuffer(IndexBuffer.BufferID, IndexBuffer.IndexFormat, 0);

            if (DepthState != null)
                ctx.OutputMerger.DepthStencilState = DepthState.Native;

            if (BlendState != null)
                ctx.OutputMerger.BlendState = BlendState.Native;

            ctx.InputAssembler.PrimitiveTopology = mTopology;
            mProgram.Bind();
        }

        public void Draw()
        {
            mContext.Context.DrawIndexed(IndexCount, StartIndex, StartVertex);
        }

        public void Draw(int numInstances)
        {
            mContext.Context.DrawIndexedInstanced(IndexCount, numInstances, StartIndex, StartVertex, 0);
        }

        public void DrawNonIndexed()
        {
            mContext.Context.Draw(IndexCount, StartVertex);
        }

        public void UpdateInstanceBuffer(VertexBuffer buffer)
        {
            if (InstanceStride == 0 || buffer == null)
                return;

            mContext.Context.InputAssembler.SetVertexBuffers(1,
                new VertexBufferBinding(buffer.BufferID, InstanceStride, 0));
        }

        public void UpdateIndexBuffer(IndexBuffer ib)
        {
            mContext.Context.InputAssembler.SetIndexBuffer(ib.BufferID, ib.IndexFormat, 0);
        }

        public void UpdateVertexBuffer(VertexBuffer vb)
        {
            mContext.Context.InputAssembler.SetVertexBuffers(0, new[] { vb.BufferID }, new[] { Stride }, new[] { 0 });
        }

        public void UpdateBlendState(BlendState state)
        {
			mContext.Context.OutputMerger.BlendState = state.Native;
            BlendState = state;
        }

        public void UpdateRasterizerState(RasterState state)
        {
			mContext.Context.Rasterizer.State = state.Native;

            RasterizerState = state;
        }

        public void UpdateDepthState(DepthState state)
        {
			mContext.Context.OutputMerger.DepthStencilState = state.Native;
            DepthState = state;
        }

        public void AddElement(VertexElement element) { mElements.Add(element); }

        public void AddElement(string semantic, int index, int components, DataType type = DataType.Float, bool normalized = false, int slot = 0, bool instanceData = false)
        {
            AddElement(new VertexElement(semantic, index, components, type, normalized, slot, instanceData));
        }

        public void InitLayout(ShaderProgram program)
        {
            if (mLayout != null) return;
            mLayout = new InputLayout(mContext.Device, program.VertexShaderCode.Data, mElements.Select(e => e.Element).ToArray());
        }
    }
}
