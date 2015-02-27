using System;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using WoWEditor6.Graphics;
using WoWEditor6.IO.Files.Models;

namespace WoWEditor6.Scene.Models.M2
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

        public virtual void Dispose()
        {
            var ib = mInstanceBuffer;
            WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
            {
                if (ib != null)
                    ib.Dispose();
            });
        }

        public static void BeginDraw()
        {
            gMesh.BeginDraw();

            if( IO.FileManager.Instance.Version == IO.FileDataVersion.Lichking )
            {
                gMesh.InitLayout(gNoBlendProgram);
            }
            else
            {
                gMesh.InitLayout(gCustomProgram);
            }
            
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
                float[] transparencyFloats = new float[4] { 1, 1, 1, 1 };
                for (var i = 0; i < pass.OpCount; ++i)
                {
                    transparencyFloats[i] = renderer.Animator.GetAlphaValue(pass.AlphaAnimIndex + i);
                }

                Matrix _uvAnimMatrix1 = Matrix.Identity;
                Matrix _uvAnimMatrix2 = Matrix.Identity;
                Matrix _uvAnimMatrix3 = Matrix.Identity;
                Matrix _uvAnimMatrix4 = Matrix.Identity;

                renderer.Animator.GetUvAnimMatrix(pass.TexAnimIndex + 0, out _uvAnimMatrix1);
                if (pass.OpCount >= 2) renderer.Animator.GetUvAnimMatrix(pass.TexAnimIndex + 1, out _uvAnimMatrix2);
                if (pass.OpCount >= 3) renderer.Animator.GetUvAnimMatrix(pass.TexAnimIndex + 2, out _uvAnimMatrix3);
                if (pass.OpCount >= 4) renderer.Animator.GetUvAnimMatrix(pass.TexAnimIndex + 3, out _uvAnimMatrix4);

                var color = renderer.Animator.GetColorValue(pass.ColorAnimIndex);

                gPerPassBuffer.UpdateData(new PerModelPassBuffer
                {
                    uvAnimMatrix1 = _uvAnimMatrix1,
                    uvAnimMatrix2 = _uvAnimMatrix2,
                    uvAnimMatrix3 = _uvAnimMatrix3,
                    uvAnimMatrix4 = _uvAnimMatrix4,
                    transparency = new Vector4(transparencyFloats[0], transparencyFloats[1], transparencyFloats[2], transparencyFloats[3]),
                    modelPassParams = new Vector4(unlit, unfogged, alphakey, 0.0f),
                    animatedColor = color
                });

                gMesh.StartVertex = 0;
                gMesh.StartIndex = pass.StartIndex;
                gMesh.IndexCount = pass.IndexCount;

                for (var i = 0; i < pass.OpCount && i < 4; ++i)
                {
                    Graphics.Texture.SamplerFlagType SamplerType = Model.TextureInfos[pass.TextureIndices[i]].SamplerFlags;

                    if (SamplerType == Graphics.Texture.SamplerFlagType.WrapBoth) gMesh.Program.SetPixelSampler(i, gSamplerWrapBoth);
                    else if (SamplerType == Graphics.Texture.SamplerFlagType.WrapU) gMesh.Program.SetPixelSampler(i, gSamplerWrapU);
                    else if (SamplerType == Graphics.Texture.SamplerFlagType.WrapV) gMesh.Program.SetPixelSampler(i, gSamplerWrapV);
                    else if (SamplerType == Graphics.Texture.SamplerFlagType.ClampBoth) gMesh.Program.SetPixelSampler(i, gSamplerClampBoth);

                    gMesh.Program.SetPixelTexture(i, pass.Textures[i]);
                }

                gMesh.Draw(mInstanceCount);
            }
        }

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
