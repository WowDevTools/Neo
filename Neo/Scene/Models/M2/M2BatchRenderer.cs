using System;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using Neo.Graphics;
using Neo.IO.Files.Models;

namespace Neo.Scene.Models.M2
{
    class M2BatchRenderer : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        struct PerInstanceBuffer
        {
            public Matrix matInstance;
            public Color4 colorMod;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PerModelPassBuffer
        {
            public Matrix uvAnimMatrix1;
            public Matrix uvAnimMatrix2;
            public Matrix uvAnimMatrix3;
            public Matrix uvAnimMatrix4;
            public Vector4 modelPassParams;
            public Vector4 animatedColor;
            public Vector4 transparency;
        }

        private static Mesh gMesh;
        private static Sampler gSamplerWrapU;
        private static Sampler gSamplerWrapV;
        private static Sampler gSamplerWrapBoth;
        private static Sampler gSamplerClampBoth;

        private static readonly BlendState[] BlendStates = new BlendState[2];

        private static ShaderProgram gCustomProgram;

        private static ShaderProgram gMaskBlendProgram;
        private static ShaderProgram gNoBlendProgram;

        private static RasterState gNoCullState;
        private static RasterState gCullState;

        public M2File Model { get; private set; }

        private VertexBuffer mInstanceBuffer;

        private int mInstanceCount;

        private PerInstanceBuffer[] mActiveInstances = new PerInstanceBuffer[0];
        private static ConstantBuffer gPerPassBuffer;

        public M2BatchRenderer(M2File model)
        {
            Model = model;
        }

        ~M2BatchRenderer()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (mInstanceBuffer != null)
            {
                var ib = mInstanceBuffer;
                WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
                {
                    if (ib != null)
                        ib.Dispose();
                });

                mInstanceBuffer = null;
            }

            Model = null;
            mActiveInstances = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static void BeginDraw()
        {
            gMesh.BeginDraw();

            // TODO: Get rid of this switch
            if (IO.FileManager.Instance.Version == IO.FileDataVersion.Lichking)
                gMesh.InitLayout(gNoBlendProgram);
            else
                gMesh.InitLayout(gCustomProgram);
            
            gMesh.Program.SetPixelSampler(0, gSamplerWrapBoth);
            gMesh.Program.SetPixelSampler(1, gSamplerWrapBoth);
            gMesh.Program.SetPixelSampler(2, gSamplerWrapBoth);
            gMesh.Program.SetPixelSampler(3, gSamplerWrapBoth);
            gMesh.Program.SetVertexConstantBuffer(2, gPerPassBuffer);
            gMesh.Program.SetPixelConstantBuffer(1, gPerPassBuffer);
        }

        public void OnFrame(M2Renderer renderer)
        {
            UpdateVisibleInstances(renderer);
            if (mInstanceCount == 0)
                return;

            gMesh.UpdateIndexBuffer(renderer.IndexBuffer);
            gMesh.UpdateVertexBuffer(renderer.VertexBuffer);
            gMesh.UpdateInstanceBuffer(mInstanceBuffer);
            gMesh.Program.SetVertexConstantBuffer(1, renderer.AnimBuffer);

            foreach (var pass in Model.Passes)
            {
                // This renderer is only for opaque pass
                if (pass.BlendMode != 0 && pass.BlendMode != 1)
                    continue;

                // TODO: Since this isn't choosing among static programs anymore, cache a different way e.g. (comparison func)
                var ctx = WorldFrame.Instance.GraphicsContext;
                gCustomProgram.SetVertexShader(ctx.M2Shaders.GetVertexShader_Instanced(pass.VertexShaderType));
                gCustomProgram.SetPixelShader(ctx.M2Shaders.GetPixelShader(pass.PixelShaderType));

                gMesh.Program = gCustomProgram;
                gCustomProgram.Bind();
     
                var cullingDisabled = (pass.RenderFlag & 0x04) != 0;
                gMesh.UpdateRasterizerState(cullingDisabled ? gNoCullState : gCullState);
                gMesh.UpdateBlendState(BlendStates[pass.BlendMode]);

                var unlit = ((pass.RenderFlag & 0x01) != 0) ? 0.0f : 1.0f;
                var unfogged = ((pass.RenderFlag & 0x02) != 0) ? 0.0f : 1.0f;
                var alphakey = (pass.BlendMode == 1) ? 1.0f : 0.0f;

                // These are per texture
                var alphaValues = new float[] { 1, 1, 1, 1 };
                for (var i = 0; i < pass.OpCount; ++i)
                    alphaValues[i] = renderer.Animator.GetAlphaValue(pass.AlphaAnimIndex + i);

                var uvAnimMatrix1 = Matrix.Identity;
                var uvAnimMatrix2 = Matrix.Identity;
                var uvAnimMatrix3 = Matrix.Identity;
                var uvAnimMatrix4 = Matrix.Identity;

                renderer.Animator.GetUvAnimMatrix(pass.TexAnimIndex + 0, out uvAnimMatrix1);
                if (pass.OpCount >= 2) renderer.Animator.GetUvAnimMatrix(pass.TexAnimIndex + 1, out uvAnimMatrix2);
                if (pass.OpCount >= 3) renderer.Animator.GetUvAnimMatrix(pass.TexAnimIndex + 2, out uvAnimMatrix3);
                if (pass.OpCount >= 4) renderer.Animator.GetUvAnimMatrix(pass.TexAnimIndex + 3, out uvAnimMatrix4);

                gPerPassBuffer.UpdateData(new PerModelPassBuffer
                {
                    uvAnimMatrix1 = uvAnimMatrix1,
                    uvAnimMatrix2 = uvAnimMatrix2,
                    uvAnimMatrix3 = uvAnimMatrix3,
                    uvAnimMatrix4 = uvAnimMatrix4,
                    transparency = new Vector4(alphaValues[0], alphaValues[1], alphaValues[2], alphaValues[3]),
                    modelPassParams = new Vector4(unlit, unfogged, alphakey, 0.0f),
                    animatedColor = renderer.Animator.GetColorValue(pass.ColorAnimIndex)
                });

                for (var i = 0; i < pass.OpCount && i < 4; ++i)
                {
                    switch (Model.TextureInfos[pass.TextureIndices[i]].SamplerFlags)
                    {
                        case Graphics.Texture.SamplerFlagType.WrapBoth:
                            gMesh.Program.SetPixelSampler(i, gSamplerWrapBoth);
                            break;
                        case Graphics.Texture.SamplerFlagType.WrapU:
                            gMesh.Program.SetPixelSampler(i, gSamplerWrapU);
                            break;
                        case Graphics.Texture.SamplerFlagType.WrapV:
                            gMesh.Program.SetPixelSampler(i, gSamplerWrapV);
                            break;
                        case Graphics.Texture.SamplerFlagType.ClampBoth:
                            gMesh.Program.SetPixelSampler(i, gSamplerClampBoth);
                            break;
                    }

                    gMesh.Program.SetPixelTexture(i, pass.Textures[i]);
                }

                gMesh.StartVertex = 0;
                gMesh.StartIndex = pass.StartIndex;
                gMesh.IndexCount = pass.IndexCount;
                gMesh.Draw(mInstanceCount);
            }
        }

        // TODO: Get rid of this
        public void OnFrame_Old(M2Renderer renderer)
        {
            UpdateVisibleInstances(renderer);
            if (mInstanceCount == 0)
                return;

            gMesh.UpdateIndexBuffer(renderer.IndexBuffer);
            gMesh.UpdateVertexBuffer(renderer.VertexBuffer);
            gMesh.UpdateInstanceBuffer(mInstanceBuffer);
            gMesh.Program.SetVertexConstantBuffer(1, renderer.AnimBuffer);

            foreach (var pass in Model.Passes)
            {
                // This renderer is only for opaque pass
                if (pass.BlendMode != 0 && pass.BlendMode != 1)
                    continue;

                var program = pass.BlendMode == 0 ? gNoBlendProgram : gMaskBlendProgram;
                if (program != gMesh.Program)
                {
                    gMesh.Program = program;
                    program.Bind();
                }

                var cullingDisabled = (pass.RenderFlag & 0x04) != 0;
                gMesh.UpdateRasterizerState(cullingDisabled ? gNoCullState : gCullState);
                gMesh.UpdateBlendState(BlendStates[pass.BlendMode]);

                var unlit = ((pass.RenderFlag & 0x01) != 0) ? 0.0f : 1.0f;
                var unfogged = ((pass.RenderFlag & 0x02) != 0) ? 0.0f : 1.0f;

                Matrix uvAnimMat;
                renderer.Animator.GetUvAnimMatrix(pass.TexAnimIndex, out uvAnimMat);
                var color = renderer.Animator.GetColorValue(pass.ColorAnimIndex);
                color.W *= renderer.Animator.GetAlphaValue(pass.AlphaAnimIndex);

                gPerPassBuffer.UpdateData(new PerModelPassBuffer
                {
                    uvAnimMatrix1 = uvAnimMat,
                    modelPassParams = new Vector4(unlit, unfogged, 0.0f, 0.0f),
                    animatedColor = color
                });

                gMesh.StartVertex = 0;
                gMesh.StartIndex = pass.StartIndex;
                gMesh.IndexCount = pass.IndexCount;
                gMesh.Program.SetPixelTexture(0, pass.Textures.First());
                gMesh.Draw(mInstanceCount);
            }
        }

        private void UpdateVisibleInstances(M2Renderer renderer)
        {
            lock (renderer.VisibleInstances)
            {
                if (mActiveInstances.Length < renderer.VisibleInstances.Count)
                    mActiveInstances = new PerInstanceBuffer[renderer.VisibleInstances.Count];

                for (var i = 0; i < renderer.VisibleInstances.Count; ++i)
                {
                    mActiveInstances[i].matInstance = renderer.VisibleInstances[i].InstanceMatrix;
                    mActiveInstances[i].colorMod = renderer.VisibleInstances[i].HighlightColor;
                }

                mInstanceCount = renderer.VisibleInstances.Count;
                if (mInstanceCount == 0)
                    return;
            }

            mInstanceBuffer.UpdateData(mActiveInstances);
        }

        public void OnSyncLoad()
        {
            var ctx = WorldFrame.Instance.GraphicsContext;
            mInstanceBuffer = new VertexBuffer(ctx);
        }

        public static void Initialize(GxContext context)
        {
            gMesh = new Mesh(context)
            {
                Stride = IO.SizeCache<M2Vertex>.Size,
                InstanceStride = IO.SizeCache<PerInstanceBuffer>.Size,
                DepthState = {
                    DepthEnabled = true,
                    DepthWriteEnabled = true
                }
            };

            gMesh.BlendState.Dispose();
            gMesh.IndexBuffer.Dispose();
            gMesh.VertexBuffer.Dispose();

            gMesh.BlendState = null;
            gMesh.IndexBuffer = null;
            gMesh.VertexBuffer = null;

            gMesh.AddElement("POSITION", 0, 3);
            gMesh.AddElement("BLENDWEIGHT", 0, 4, DataType.Byte, true);
            gMesh.AddElement("BLENDINDEX", 0, 4, DataType.Byte);
            gMesh.AddElement("NORMAL", 0, 3);
            gMesh.AddElement("TEXCOORD", 0, 2);
            gMesh.AddElement("TEXCOORD", 1, 2);

            gMesh.AddElement("TEXCOORD", 2, 4, DataType.Float, false, 1, true);
            gMesh.AddElement("TEXCOORD", 3, 4, DataType.Float, false, 1, true);
            gMesh.AddElement("TEXCOORD", 4, 4, DataType.Float, false, 1, true);
            gMesh.AddElement("TEXCOORD", 5, 4, DataType.Float, false, 1, true);
            gMesh.AddElement("COLOR", 0, 4, DataType.Float, false, 1, true);

            // all combinations are set in this one each time
            gCustomProgram = new ShaderProgram(context);
            gCustomProgram.SetVertexShader(Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1);
            gCustomProgram.SetPixelShader(Resources.Shaders.M2Pixel_PS_Combiners_Opaque);

            gMesh.Program = gCustomProgram;

            // Old versions for temporary WOTLK compatibility.. can we figure out how to map these to the actual types??
            gNoBlendProgram = new ShaderProgram(context);
            gNoBlendProgram.SetVertexShader(Resources.Shaders.M2VertexInstancedOld);
            gNoBlendProgram.SetPixelShader(Resources.Shaders.M2PixelOld);

            gMaskBlendProgram = new ShaderProgram(context);
            gMaskBlendProgram.SetVertexShader(Resources.Shaders.M2VertexInstancedOld);
            gMaskBlendProgram.SetPixelShader(Resources.Shaders.M2PixelBlendAlphaOld);
            
            gPerPassBuffer = new ConstantBuffer(context);

            gPerPassBuffer.UpdateData(new PerModelPassBuffer()
            {
                uvAnimMatrix1 = Matrix.Identity,
                uvAnimMatrix2 = Matrix.Identity,
                uvAnimMatrix3 = Matrix.Identity,
                uvAnimMatrix4 = Matrix.Identity,
                modelPassParams = Vector4.Zero
            });

            gSamplerWrapU = new Sampler(context)
            {
                AddressU = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                AddressV = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                AddressW = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                Filter = SharpDX.Direct3D11.Filter.Anisotropic,
                MaximumAnisotropy = 16
            };

            gSamplerWrapV = new Sampler(context)
            {
                AddressU = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                AddressV = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                AddressW = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                Filter = SharpDX.Direct3D11.Filter.Anisotropic,
                MaximumAnisotropy = 16
            };

            gSamplerWrapBoth = new Sampler(context)
            {
                AddressU = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                AddressV = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                AddressW = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                Filter = SharpDX.Direct3D11.Filter.Anisotropic,
                MaximumAnisotropy = 16
            };

            gSamplerClampBoth = new Sampler(context)
            {
                AddressU = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                AddressV = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                AddressW = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                Filter = SharpDX.Direct3D11.Filter.Anisotropic,
                MaximumAnisotropy = 16
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

            gNoCullState = new RasterState(context) { CullEnabled = false };
            gCullState = new RasterState(context) { CullEnabled = true };
        }
    }
}
