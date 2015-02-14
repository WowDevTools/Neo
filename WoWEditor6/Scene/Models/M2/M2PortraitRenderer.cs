using System;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using WoWEditor6.Graphics;
using WoWEditor6.IO.Files.Models;

namespace WoWEditor6.Scene.Models.M2
{
    class M2PortraitRenderer : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        struct PerModelPassBuffer
        {
            public Matrix uvAnimMatrix;
            public Vector4 modelPassParams;
        }

        private static Mesh Mesh { get; set; }
        private static Sampler Sampler { get; set; }

        private static readonly BlendState[] BlendStates = new BlendState[7];
        private static ShaderProgram gNoBlendProgram;
        private static ShaderProgram gBlendProgram;
        private static RasterState gNoCullState;
        private static RasterState gCullState;

        private readonly IM2Animator mAnimator;
        public M2File Model { get; private set; }

        public TextureInfo[] Textures { get; private set; }

        private readonly Matrix[] mAnimationMatrices = new Matrix[256];

        private ConstantBuffer mAnimBuffer;
        private ConstantBuffer mPerPassBuffer;

        public M2PortraitRenderer(M2File model)
        {
            Model = model;
            Textures = model.TextureInfos.ToArray();
            mAnimator = ModelFactory.Instance.CreateAnimator(model);
            mAnimator.SetAnimationByIndex(0);
            mAnimator.Update();
        }

        public virtual void Dispose()
        {
            if (mAnimBuffer != null)
                mAnimBuffer.Dispose();

            if (mPerPassBuffer != null)
                mPerPassBuffer.Dispose();
        }

        public void OnFrame(M2Renderer renderer)
        {
            mAnimator.Update();

            Mesh.BeginDraw();
            Mesh.Program.SetPixelSampler(0, Sampler);

            Mesh.UpdateIndexBuffer(renderer.IndexBuffer);
            Mesh.UpdateVertexBuffer(renderer.VertexBuffer);

            if (mAnimator.GetBones(mAnimationMatrices))
                mAnimBuffer.UpdateData(mAnimationMatrices);

            Mesh.Program.SetVertexConstantBuffer(2, mAnimBuffer);
            Mesh.Program.SetVertexConstantBuffer(3, mPerPassBuffer);

            foreach (var pass in Model.Passes)
            {
                var cullingDisabled = (pass.RenderFlag & 0x04) != 0;
                Mesh.UpdateRasterizerState(cullingDisabled ? gNoCullState : gCullState);
                Mesh.UpdateBlendState(BlendStates[pass.BlendMode]);

                var oldProgram = Mesh.Program;
                Mesh.Program = (pass.BlendMode > 0 ? gBlendProgram : gNoBlendProgram);
                if (Mesh.Program != oldProgram)
                    Mesh.Program.Bind();

                var unlit = ((pass.RenderFlag & 0x01) != 0) ? 0.0f : 1.0f;
                var unfogged = ((pass.RenderFlag & 0x02) != 0) ? 0.0f : 1.0f;

                Matrix uvAnimMat;
                mAnimator.GetUvAnimMatrix(pass.TexAnimIndex, out uvAnimMat);

                mPerPassBuffer.UpdateData(new PerModelPassBuffer()
                {
                    uvAnimMatrix = uvAnimMat,
                    modelPassParams = new Vector4(unlit, unfogged, 0.0f, 0.0f)
                });

                Mesh.StartVertex = 0;
                Mesh.StartIndex = pass.StartIndex;
                Mesh.IndexCount = pass.IndexCount;
                Mesh.Program.SetPixelTexture(0, pass.Textures.First());
                Mesh.Draw();
            }
        }

        public void OnSyncLoad()
        {
            var ctx = WorldFrame.Instance.GraphicsContext;
            mAnimBuffer = new ConstantBuffer(ctx);
            mAnimBuffer.UpdateData(mAnimationMatrices);

            mPerPassBuffer = new ConstantBuffer(ctx);
            mPerPassBuffer.UpdateData(new PerModelPassBuffer()
            {
                uvAnimMatrix = Matrix.Identity,
                modelPassParams = Vector4.Zero
            });
        }

        public static void Initialize(GxContext context)
        {
            Mesh = new Mesh(context)
            {
                Stride = IO.SizeCache<M2Vertex>.Size,
                DepthState = { DepthEnabled = true }
            };

            Mesh.BlendState.Dispose();
            Mesh.IndexBuffer.Dispose();
            Mesh.VertexBuffer.Dispose();

            Mesh.AddElement("POSITION", 0, 3);
            Mesh.AddElement("BLENDWEIGHT", 0, 4, DataType.Byte, true);
            Mesh.AddElement("BLENDINDEX", 0, 4, DataType.Byte);
            Mesh.AddElement("NORMAL", 0, 3);
            Mesh.AddElement("TEXCOORD", 0, 2);
            Mesh.AddElement("TEXCOORD", 1, 2);

            var program = new ShaderProgram(context);
            program.SetVertexShader(Resources.Shaders.M2VertexPortrait);
            program.SetPixelShader(Resources.Shaders.M2PixelPortrait);

            Mesh.Program = program;

            Sampler = new Sampler(context)
            {
                AddressMode = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                Filter = SharpDX.Direct3D11.Filter.MinMagMipLinear
            };

            for (var i = 0; i < BlendStates.Length; ++i)
                BlendStates[i] = new BlendState(context);

            BlendStates[0] = new BlendState(context)
            {
                BlendEnabled = false
            };

            BlendStates[1] = new BlendState(context)
            {
                BlendEnabled = true,
                SourceBlend = SharpDX.Direct3D11.BlendOption.One,
                DestinationBlend = SharpDX.Direct3D11.BlendOption.Zero,
                SourceAlphaBlend = SharpDX.Direct3D11.BlendOption.One,
                DestinationAlphaBlend = SharpDX.Direct3D11.BlendOption.Zero
            };

            BlendStates[2] = new BlendState(context)
            {
                BlendEnabled = true,
                SourceBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha,
                DestinationBlend = SharpDX.Direct3D11.BlendOption.InverseSourceAlpha,
                SourceAlphaBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha,
                DestinationAlphaBlend = SharpDX.Direct3D11.BlendOption.InverseSourceAlpha
            };

            BlendStates[3] = new BlendState(context)
            {
                BlendEnabled = true,
                SourceBlend = SharpDX.Direct3D11.BlendOption.SourceColor,
                DestinationBlend = SharpDX.Direct3D11.BlendOption.DestinationColor,
                SourceAlphaBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha,
                DestinationAlphaBlend = SharpDX.Direct3D11.BlendOption.DestinationAlpha
            };

            BlendStates[4] = new BlendState(context)
            {
                BlendEnabled = true,
                SourceBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha,
                DestinationBlend = SharpDX.Direct3D11.BlendOption.One,
                SourceAlphaBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha,
                DestinationAlphaBlend = SharpDX.Direct3D11.BlendOption.One
            };

            BlendStates[5] = new BlendState(context)
            {
                BlendEnabled = true,
                SourceBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha,
                DestinationBlend = SharpDX.Direct3D11.BlendOption.InverseSourceAlpha,
                SourceAlphaBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha,
                DestinationAlphaBlend = SharpDX.Direct3D11.BlendOption.InverseSourceAlpha
            };

            BlendStates[6] = new BlendState(context)
            {
                BlendEnabled = true,
                SourceBlend = SharpDX.Direct3D11.BlendOption.DestinationColor,
                DestinationBlend = SharpDX.Direct3D11.BlendOption.SourceColor,
                SourceAlphaBlend = SharpDX.Direct3D11.BlendOption.DestinationAlpha,
                DestinationAlphaBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha
            };

            gNoBlendProgram = program;

            gBlendProgram = new ShaderProgram(context);
            gBlendProgram.SetPixelShader(Resources.Shaders.M2PixelPortraitBlend);
            gBlendProgram.SetVertexShader(Resources.Shaders.M2VertexPortrait);

            gNoCullState = new RasterState(context) {CullEnabled = false};
            gCullState = new RasterState(context) {CullEnabled = true};
        }
    }
}
