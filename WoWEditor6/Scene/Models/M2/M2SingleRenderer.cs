using System;
using System.Runtime.InteropServices;
using SharpDX;
using WoWEditor6.Graphics;
using WoWEditor6.IO.Files.Models;
using WoWEditor6.Storage;

namespace WoWEditor6.Scene.Models.M2
{
    class M2SingleRenderer : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        struct PerModelPassBuffer
        {
            public Matrix uvAnimMatrix;
            public Vector4 modelPassParams;
            public Vector4 animatedColor;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PerDrawCallBuffer
        {
            public Matrix instanceMat;
            public Color4 colorMod;
        }

        private static Mesh gMesh;
        private static Sampler gSampler;

        private static readonly BlendState[] BlendStates = new BlendState[7];
        private static ShaderProgram gNoBlendProgram;
        private static ShaderProgram gBlendProgram;
        private static ShaderProgram gBlendTestProgram;
        private static ShaderProgram g2PassProgram;
        private static ShaderProgram g3PassProgram;

        private static RasterState gNoCullState;
        private static RasterState gCullState;

        private static DepthState gDepthWriteState;
        private static DepthState gDepthNoWriteState;

        private readonly M2File mModel;
        private readonly IM2Animator mAnimator;
        private readonly Matrix[] mAnimationMatrices;
        private ConstantBuffer mAnimBuffer;

        private static ConstantBuffer gPerDrawCallBuffer;
        private static ConstantBuffer gPerPassBuffer;

        public M2SingleRenderer(M2File model)
        {
            mModel = model;
            if (model.NeedsPerInstanceAnimation)
            {
                mAnimationMatrices = new Matrix[model.GetNumberOfBones()];
                mAnimator = ModelFactory.Instance.CreateAnimator(model);
                if(mAnimator.SetAnimation(AnimationType.Stand) == false)
                    mAnimator.SetAnimationByIndex(0);
            }
        }

        public virtual void Dispose()
        {
            var ab = mAnimBuffer;
            WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
            {
                if (ab != null)
                    ab.Dispose();
            });
        }

        public static void BeginDraw()
        {
            gMesh.BeginDraw();
            gMesh.Program.SetPixelSampler(0, gSampler);
            gMesh.Program.SetVertexConstantBuffer(2, gPerDrawCallBuffer);
            gMesh.Program.SetVertexConstantBuffer(3, gPerPassBuffer);
            gMesh.Program.SetPixelConstantBuffer(1, gPerPassBuffer);
        }

        public void OnFrame(M2Renderer renderer, M2RenderInstance instance)
        {
            var animator = renderer.Animator;
            if (mAnimator != null)
            {
                // If we have our own animator, use that. Otherwise use the global one.
                animator = mAnimator;

                var camera = WorldFrame.Instance.ActiveCamera;
                mAnimator.Update(new BillboardParameters
                {
                    Forward = camera.Forward,
                    Right = camera.Right,
                    Up = camera.Up,
                    InverseRotation = instance.InverseRotation
                });

                if (mAnimator.GetBones(mAnimationMatrices))
                    mAnimBuffer.UpdateData(mAnimationMatrices);
            }

            gMesh.UpdateIndexBuffer(renderer.IndexBuffer);
            gMesh.UpdateVertexBuffer(renderer.VertexBuffer);

            gPerDrawCallBuffer.UpdateData(new PerDrawCallBuffer
            {
                instanceMat = instance.InstanceMatrix,
                colorMod = instance.HighlightColor
            });

            gMesh.Program.SetVertexConstantBuffer(1, mAnimBuffer ?? renderer.AnimBuffer);

            foreach (var pass in mModel.Passes)
            {
                if (!mModel.NeedsPerInstanceAnimation)
                {
                    // Prevent double rendering since this model pass
                    // was already processed by the batch renderer
                    if (pass.BlendMode == 0 || pass.BlendMode == 1)
                        continue;
                }

                var oldProgram = gMesh.Program;
                ShaderProgram newProgram;
                switch (pass.BlendMode)
                {
                    case 0:
                        newProgram = gNoBlendProgram;
                        break;
                    case 1:
                        newProgram = gBlendTestProgram;
                        break;
                    default:
                        switch (pass.Textures.Count)
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
                    gMesh.Program = newProgram;
                    gMesh.Program.Bind();
                }

                var depthState = gDepthNoWriteState;
                if (pass.BlendMode == 0 || pass.BlendMode == 1)
                    depthState = gDepthWriteState;

                gMesh.UpdateDepthState(depthState);

                var cullingDisabled = (pass.RenderFlag & 0x04) != 0;
                gMesh.UpdateRasterizerState(cullingDisabled ? gNoCullState : gCullState);
                gMesh.UpdateBlendState(BlendStates[pass.BlendMode]);

                var unlit = ((pass.RenderFlag & 0x01) != 0) ? 0.0f : 1.0f;
                var unfogged = ((pass.RenderFlag & 0x02) != 0) ? 0.0f : 1.0f;

                Matrix uvAnimMat;
                animator.GetUvAnimMatrix(pass.TexAnimIndex, out uvAnimMat);
                var color = animator.GetColorValue(pass.ColorAnimIndex);
                var alpha = animator.GetAlphaValue(pass.AlphaAnimIndex);
                color.W *= alpha;

                gPerPassBuffer.UpdateData(new PerModelPassBuffer()
                {
                    uvAnimMatrix = uvAnimMat,
                    modelPassParams = new Vector4(unlit, unfogged, 0.0f, 0.0f),
                    animatedColor = color
                });

                gMesh.StartVertex = 0;
                gMesh.StartIndex = pass.StartIndex;
                gMesh.IndexCount = pass.IndexCount;
                for(var i = 0; i < pass.Textures.Count && i < 3; ++i)
                    gMesh.Program.SetPixelTexture(i, pass.Textures[i]);

                gMesh.Draw();
            }
        }

        public void OnSyncLoad()
        {
            var ctx = WorldFrame.Instance.GraphicsContext;
            if (mAnimator != null)
            {
                mAnimBuffer = new ConstantBuffer(ctx);
                mAnimBuffer.UpdateData(mAnimationMatrices);
            }
        }

        public static void Initialize(GxContext context)
        {
            gDepthWriteState = new DepthState(context)
            {
                DepthEnabled = true,
                DepthWriteEnabled = true
            };

            gDepthNoWriteState = new DepthState(context)
            {
                DepthEnabled = true,
                DepthWriteEnabled = false
            };

            gMesh = new Mesh(context)
            {
                Stride = IO.SizeCache<M2Vertex>.Size,
                DepthState = gDepthNoWriteState
            };

            gMesh.BlendState.Dispose();
            gMesh.IndexBuffer.Dispose();
            gMesh.VertexBuffer.Dispose();

            gMesh.AddElement("POSITION", 0, 3);
            gMesh.AddElement("BLENDWEIGHT", 0, 4, DataType.Byte, true);
            gMesh.AddElement("BLENDINDEX", 0, 4, DataType.Byte);
            gMesh.AddElement("NORMAL", 0, 3);
            gMesh.AddElement("TEXCOORD", 0, 2);
            gMesh.AddElement("TEXCOORD", 1, 2);

            gNoBlendProgram = new ShaderProgram(context);
            gNoBlendProgram.SetPixelShader(Resources.Shaders.M2Pixel);
            gNoBlendProgram.SetVertexShader(Resources.Shaders.M2VertexSingle);

            gBlendProgram = new ShaderProgram(context);
            gBlendProgram.SetPixelShader(Resources.Shaders.M2PixelBlend);
            gBlendProgram.SetVertexShader(Resources.Shaders.M2VertexSingle);

            gBlendTestProgram = new ShaderProgram(context);
            gBlendTestProgram.SetPixelShader(Resources.Shaders.M2PixelBlendAlpha);
            gBlendTestProgram.SetVertexShader(Resources.Shaders.M2VertexSingle);

            g2PassProgram = new ShaderProgram(context);
            g2PassProgram.SetPixelShader(Resources.Shaders.M2Pixel2Pass);
            g2PassProgram.SetVertexShader(Resources.Shaders.M2VertexSingle);

            g3PassProgram = new ShaderProgram(context);
            g3PassProgram.SetPixelShader(Resources.Shaders.M2Pixel3Pass);
            g3PassProgram.SetVertexShader(Resources.Shaders.M2VertexSingle);

            gPerDrawCallBuffer = new ConstantBuffer(context);
            gPerDrawCallBuffer.UpdateData(new PerDrawCallBuffer()
            {
                instanceMat = Matrix.Identity
            });

            gPerPassBuffer = new ConstantBuffer(context);
            gPerPassBuffer.UpdateData(new PerModelPassBuffer()
            {
                uvAnimMatrix = Matrix.Identity,
                modelPassParams = Vector4.Zero
            });

            gMesh.Program = gBlendProgram;

            gSampler = new Sampler(context)
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

            gNoCullState = new RasterState(context) { CullEnabled = false };
            gCullState = new RasterState(context) { CullEnabled = true };
        }
    }
}
