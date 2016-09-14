using System.Collections.Generic;
using System.Linq;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace Neo.Graphics
{
    class Mesh
    {
        private readonly List<VertexElement> mElements = new List<VertexElement>();
        private ShaderProgram mProgram;
        private InputLayout mLayout;
        private PrimitiveTopology mTopology = PrimitiveTopology.TriangleList;
        private readonly GxContext mContext;

        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        public int Stride { get; set; }
        public int InstanceStride { get; set; }
        public int IndexCount { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int StartIndex { get; set; }
        public int StartVertex { get; set; }
        public DepthState DepthState { get; set; }
        public RasterState RasterizerState { get; set; }
        public BlendState BlendState { get; set; }
        public ShaderProgram Program { get { return mProgram; } set { UpdateProgram(value); } }
        public PrimitiveTopology Topology { get { return mTopology; } set { UpdateTopology(value); } }
        public InputLayout Layout { get { return mLayout; } set { UpdateLayout(value); } }

        public Mesh(GxContext context)
        {
            mContext = context;
            VertexBuffer = new VertexBuffer(context);
            IndexBuffer = new IndexBuffer(context);
            DepthState = new DepthState(context);
            RasterizerState = new RasterState(context);
            BlendState = new BlendState(context);
        }

        public void BeginDraw()
        {
            var ctx = mContext.Context;
            if (VertexBuffer != null)
                ctx.InputAssembler.SetVertexBuffers(0, new[] {VertexBuffer.Native}, new[] {Stride}, new[] {0});

            if (IndexBuffer != null)
                ctx.InputAssembler.SetIndexBuffer(IndexBuffer.Native, IndexBuffer.IndexFormat, 0);

            if (DepthState != null)
                ctx.OutputMerger.DepthStencilState = DepthState.Native;

            if (RasterizerState != null)
                ctx.Rasterizer.State = RasterizerState.Native;

            if (BlendState != null)
                ctx.OutputMerger.BlendState = BlendState.Native;

            ctx.InputAssembler.InputLayout = mLayout;
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
                new VertexBufferBinding(buffer.Native, InstanceStride, 0));
        }

        public void UpdateIndexBuffer(IndexBuffer ib)
        {
            mContext.Context.InputAssembler.SetIndexBuffer(ib.Native, ib.IndexFormat, 0);
        }

        public void UpdateVertexBuffer(VertexBuffer vb)
        {
            mContext.Context.InputAssembler.SetVertexBuffers(0, new[] { vb.Native }, new[] { Stride }, new[] { 0 });
        }

        public void UpdateBlendState(BlendState state)
        {
            if (mContext.Context.OutputMerger.BlendState != state.Native)
                mContext.Context.OutputMerger.BlendState = state.Native;

            BlendState = state;
        }

        public void UpdateRasterizerState(RasterState state)
        {
            if (mContext.Context.Rasterizer.State != state.Native)
                mContext.Context.Rasterizer.State = state.Native;

            RasterizerState = state;
        }

        public void UpdateDepthState(DepthState state)
        {
            if (mContext.Context.OutputMerger.DepthStencilState != state.Native)
                mContext.Context.OutputMerger.DepthStencilState = state.Native;

            DepthState = state;
        }

        public void AddElement(VertexElement element) { mElements.Add(element); }

        public void AddElement(string semantic, int index, int components, DataType type = DataType.Float, bool normalized = false, int slot = 0, bool instanceData = false)
        {
            AddElement(new VertexElement(semantic, index, components, type, normalized, slot, instanceData));
        }

        private void UpdateLayout(InputLayout Layout)
        {
            mLayout = Layout;

            if (mContext.Context.InputAssembler.InputLayout != Layout)
                mContext.Context.InputAssembler.InputLayout = Layout;
        }

        public void InitLayout(ShaderProgram program)
        {
            if (mLayout != null) return;
            mLayout = new InputLayout(mContext.Device, program.VertexShaderCode.Data, mElements.Select(e => e.Element).ToArray());
        }

        private void UpdateProgram(ShaderProgram program)
        {
            if (program == mProgram)
                return;

            //mLayout = InputLayoutCache.GetLayout(mContext, mElements.Select(e => e.Element).ToArray(), this, program);
            mProgram = program;
        }

        private void UpdateTopology(PrimitiveTopology topology)
        {
            mTopology = topology;

            if (mContext.Context.InputAssembler.PrimitiveTopology != topology)
                mContext.Context.InputAssembler.PrimitiveTopology = topology;
        }
    }
}
