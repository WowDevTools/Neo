using System.Collections.Generic;
using System.Linq;
using SharpDX.Direct3D11;

namespace WoWEditor6.Graphics
{
    class Mesh
    {
        private readonly List<VertexElement> mElements = new List<VertexElement>();
        private ShaderProgram mProgram;
        private InputLayout mLayout;
        private readonly GxContext mContext;

        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        public int Stride { get; set; }
        public int IndexCount { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int StartIndex { get; set; }
        public int StartVertex { get; set; }
        public DepthState DepthState { get; set; }
        public RasterState RasterizerState { get; set; }
        public BlendState BlendState { get; set; }

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

            ctx.InputAssembler.SetIndexBuffer(IndexBuffer.Native, IndexBuffer.IndexFormat, 0);
            ctx.InputAssembler.InputLayout = mLayout;
            ctx.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            ctx.OutputMerger.DepthStencilState = DepthState.State;
            ctx.Rasterizer.State = RasterizerState.Native;
            ctx.OutputMerger.BlendState = BlendState.Native;
            mProgram.Bind();
        }

        public void Draw()
        {
            mContext.Context.DrawIndexed(IndexCount, StartIndex, StartVertex);
        }

        public void UpdateVertexBuffer(VertexBuffer vb)
        {
            mContext.Context.InputAssembler.SetVertexBuffers(0, new[] {vb.Native}, new[] {Stride}, new[] {0});
        }

        public ShaderProgram Program { get { return mProgram; } set { UpdateProgram(value); } }

        public void AddElement(VertexElement element) => mElements.Add(element);

        public void AddElement(string semantic, int index, int components, DataType type = DataType.Float, bool normalized = false)
        {
            AddElement(new VertexElement(semantic, index, components, type, normalized));
        }

        private void UpdateProgram(ShaderProgram program)
        {
            if (program == mProgram)
                return;

            mLayout = InputLayoutCache.GetLayout(mContext, mElements.Select(e => e.Element).ToArray(), this, program);
            mProgram = program;
        }
    }
}
