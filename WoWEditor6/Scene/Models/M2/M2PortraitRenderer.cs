using System;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using WoWEditor6.Graphics;
using WoWEditor6.IO;
using WoWEditor6.IO.Files.Models;
using WoWEditor6.Storage;

namespace WoWEditor6.Scene.Models.M2
{
    class M2PortraitRenderer : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        struct PerModelPassBuffer
        {
            public Matrix uvAnimMatrix;
            public Vector4 modelPassParams;
            public Vector4 ColorValue;
        }

        private static Mesh Mesh { get; set; }
        private static Sampler Sampler { get; set; }

        private static readonly BlendState[] BlendStates = new BlendState[7];
        private static ShaderProgram gNoBlendProgram;
        private static ShaderProgram gBlendProgram;
        private static ShaderProgram gBlendMaskProgram;
        private static ShaderProgram g2PassProgram;
        private static ShaderProgram g3PassProgram;
        private static RasterState gNoCullState;
        private static RasterState gCullState;

        public IM2Animator Animator { get; private set; }
        public M2File Model { get; private set; }

        public TextureInfo[] Textures { get; private set; }

        private readonly Matrix[] mAnimationMatrices;

        private ConstantBuffer mAnimBuffer;
        private ConstantBuffer mPerPassBuffer;

        public M2PortraitRenderer(M2File model)
        {
            Model = model;
            Textures = model.TextureInfos.ToArray();

            mAnimationMatrices = new Matrix[model.GetNumberOfBones()];
            Animator = ModelFactory.Instance.CreateAnimator(model);
            Animator.SetAnimation(AnimationType.Stand);
            Animator.Update(null);
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
            Animator.Update(null);

            Mesh.BeginDraw();
            Mesh.Program.SetPixelSampler(0, Sampler);

            Mesh.UpdateIndexBuffer(renderer.IndexBuffer);
            Mesh.UpdateVertexBuffer(renderer.VertexBuffer);

            if (Animator.GetBones(mAnimationMatrices))
                mAnimBuffer.UpdateData(mAnimationMatrices);

            Mesh.Program.SetVertexConstantBuffer(1, mAnimBuffer);
            Mesh.Program.SetVertexConstantBuffer(2, mPerPassBuffer);
            Mesh.Program.SetPixelConstantBuffer(0, mPerPassBuffer);

            foreach (var pass in Model.Passes)
            {
                var cullingDisabled = (pass.RenderFlag & 0x04) != 0;
                Mesh.UpdateRasterizerState(cullingDisabled ? gNoCullState : gCullState);
                Mesh.UpdateBlendState(BlendStates[pass.BlendMode]);

                var oldProgram = Mesh.Program;
                ShaderProgram newProgram;
                switch (pass.BlendMode)
                {
                    case 0:
                        newProgram = gNoBlendProgram;
                        break;
                    case 1:
                        newProgram = gBlendMaskProgram;
                        break;
                    default:
                        switch (pass.TextureIndices.Count)
                        {
                            case 2:
                                newProgram = g2PassProgram;
                                break;

                            case 3:
                                newProgram = g3PassProgram;
                                break;

                            default:
                                newProgram = gBlendProgram;
                                break;
                        }
                        break;
                }

                if (newProgram != oldProgram)
                {
                    Mesh.Program = newProgram;
                    Mesh.Program.Bind();
                }

                var unlit = ((pass.RenderFlag & 0x01) != 0) ? 0.0f : 1.0f;
                var unfogged = ((pass.RenderFlag & 0x02) != 0) ? 0.0f : 1.0f;

                Matrix uvAnimMat;
                Animator.GetUvAnimMatrix(pass.TexAnimIndex, out uvAnimMat);
                var color = Animator.GetColorValue(pass.ColorAnimIndex);
                var alpha = Animator.GetAlphaValue(pass.AlphaAnimIndex);
                color.W *= alpha;

                //Log.Debug(color);

                mPerPassBuffer.UpdateData(new PerModelPassBuffer
                {
                    uvAnimMatrix = uvAnimMat,
                    modelPassParams = new Vector4(unlit, unfogged, 0.0f, 0.0f),
                    ColorValue = color
                });

                Mesh.StartVertex = 0;
                Mesh.StartIndex = pass.StartIndex;
                Mesh.IndexCount = pass.IndexCount;
                for(var i = 0; i < pass.TextureIndices.Count; ++i)
                    Mesh.Program.SetPixelTexture(i, Textures[pass.TextureIndices[i]].Texture);

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

            gCullState.CullCounterClock = FileManager.Instance.Version >= FileDataVersion.Lichking;
        }

        public static void Initialize(GxContext context)
        {
            Mesh = new Mesh(context)
            {
                Stride = SizeCache<M2Vertex>.Size,
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

            gBlendMaskProgram = new ShaderProgram(context);
            gBlendMaskProgram.SetPixelShader(Resources.Shaders.M2PixelPortraitBlendAlpha);
            gBlendMaskProgram.SetVertexShader(Resources.Shaders.M2VertexPortrait);

            g2PassProgram = new ShaderProgram(context);
            g2PassProgram.SetVertexShader(Resources.Shaders.M2VertexPortrait);
            g2PassProgram.SetPixelShader(Resources.Shaders.M2PixelPortrait2Pass);

            g3PassProgram = new ShaderProgram(context);
            g3PassProgram.SetVertexShader(Resources.Shaders.M2VertexPortrait);
            g3PassProgram.SetPixelShader(Resources.Shaders.M2PixelPortrait3Pass);

            gNoCullState = new RasterState(context) {CullEnabled = false};
            gCullState = new RasterState(context) {CullEnabled = true, CullCounterClock = true};
        }
    }
}
