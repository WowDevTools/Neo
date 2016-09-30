using System;
using System.Runtime.InteropServices;
using Neo.Graphics;
using Neo.IO.Files.Models;
using Neo.Storage;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using DataType = Neo.Graphics.DataType;

namespace Neo.Scene.Models.M2
{
    class M2SingleRenderer : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        struct PerModelPassBuffer
        {
            public Matrix4 uvAnimMatrix1;
            public Matrix4 uvAnimMatrix2;
            public Matrix4 uvAnimMatrix3;
            public Matrix4 uvAnimMatrix4;
            public Vector4 modelPassParams;
            public Vector4 animatedColor;
            public Vector4 transparency;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PerDrawCallBuffer
        {
            public Matrix4 instanceMat;
            public Color4 colorMod;
        }

        private static Mesh gMesh;
        private static Sampler gSamplerWrapU;
        private static Sampler gSamplerWrapV;
        private static Sampler gSamplerWrapBoth;
        private static Sampler gSamplerClampBoth;

        private static readonly BlendState[] BlendStates = new BlendState[7];

        private static ShaderProgram gCustomProgram;

        private static ShaderProgram gNoBlendProgram;
        private static ShaderProgram gBlendProgram;
        private static ShaderProgram gBlendTestProgram;
        private static ShaderProgram g2PassProgram;
        private static ShaderProgram g3PassProgram;

        private static RasterState gNoCullState;
        private static RasterState gCullState;

        private static DepthState gDepthWriteState;
        private static DepthState gDepthNoWriteState;

        private M2File mModel;
        private IM2Animator mAnimator;
        private Matrix4[] mAnimationMatrices;
        private UniformBuffer mAnimBuffer;

        private static UniformBuffer gPerDrawCallBuffer;
        private static UniformBuffer gPerPassBuffer;

        public M2SingleRenderer(M2File model)
        {
            mModel = model;
            if (model.NeedsPerInstanceAnimation)
            {
                mAnimationMatrices = new Matrix4[model.GetNumberOfBones()];
                mAnimator = ModelFactory.Instance.CreateAnimator(model);

	            if (mAnimator.SetAnimation(AnimationType.Stand) == false)
	            {
		            mAnimator.SetAnimationByIndex(0);
	            }
            }
        }

        ~M2SingleRenderer()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (mAnimBuffer != null)
            {
                var ab = mAnimBuffer;
                WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
                {
	                if (ab != null)
	                {
		                ab.Dispose();
	                }
                });

                mAnimBuffer = null;
            }

            mModel = null;
            mAnimator = null;
            mAnimationMatrices = null;
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
            gMesh.Program.SetVertexConstantBuffer(2, gPerDrawCallBuffer);
            gMesh.Program.SetVertexConstantBuffer(3, gPerPassBuffer);
            gMesh.Program.SetPixelConstantBuffer(1, gPerPassBuffer);
        }

        public void OnFrame(M2Renderer renderer, M2RenderInstance instance)
        {
            var animator = mAnimator ?? renderer.Animator;
	        if (mAnimator != null)
	        {
		        UpdateAnimations(instance);
	        }

            gMesh.UpdateIndexBuffer(renderer.IndexBuffer);
            gMesh.UpdateVertexBuffer(renderer.VertexBuffer);

            gPerDrawCallBuffer.BufferData(new PerDrawCallBuffer
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
	                {
		                continue;
	                }
                }

                // TODO: Since this isn't choosing among static programs anymore, cache a different way (comparison func?)
                var ctx = WorldFrame.Instance.GraphicsContext;
                gCustomProgram.SetVertexShader(ctx.M2Shaders.GetVertexShader_Single(pass.VertexShaderType));
                gCustomProgram.SetPixelShader(ctx.M2Shaders.GetPixelShader(pass.PixelShaderType));

                gMesh.Program = gCustomProgram;
                gCustomProgram.Bind();

                var depthState = gDepthNoWriteState;
	            if (pass.BlendMode == 0 || pass.BlendMode == 1)
	            {
		            depthState = gDepthWriteState;
	            }

                gMesh.UpdateDepthState(depthState);

                var cullingDisabled = (pass.RenderFlag & 0x04) != 0;
                gMesh.UpdateRasterizerState(cullingDisabled ? gNoCullState : gCullState);
                gMesh.UpdateBlendState(BlendStates[pass.BlendMode]);

                var unlit = ((pass.RenderFlag & 0x01) != 0) ? 0.0f : 1.0f;
                var unfogged = ((pass.RenderFlag & 0x02) != 0) ? 0.0f : 1.0f;
                var alphakey = (pass.BlendMode == 1 ) ? 1.0f : 0.0f;

                // These are per texture
                var alphaValues = new float[] { 1, 1, 1, 1 };
	            for (var i = 0; i < pass.OpCount; ++i)
	            {
		            alphaValues[i] = animator.GetAlphaValue(pass.AlphaAnimIndex + i);
	            }

                var uvAnimMatrix1 = Matrix4.Identity;
                var uvAnimMatrix2 = Matrix4.Identity;
                var uvAnimMatrix3 = Matrix4.Identity;
                var uvAnimMatrix4 = Matrix4.Identity;

                animator.GetUvAnimMatrix(pass.TexAnimIndex + 0, out uvAnimMatrix1);
                if (pass.OpCount >= 2) animator.GetUvAnimMatrix(pass.TexAnimIndex + 1, out uvAnimMatrix2);
                if (pass.OpCount >= 3) animator.GetUvAnimMatrix(pass.TexAnimIndex + 2, out uvAnimMatrix3);
                if (pass.OpCount >= 4) animator.GetUvAnimMatrix(pass.TexAnimIndex + 3, out uvAnimMatrix4);

                gPerPassBuffer.BufferData(new PerModelPassBuffer
                {
                    uvAnimMatrix1 = uvAnimMatrix1,
                    uvAnimMatrix2 = uvAnimMatrix2,
                    uvAnimMatrix3 = uvAnimMatrix3,
                    uvAnimMatrix4 = uvAnimMatrix4,
                    transparency = new Vector4(alphaValues[0], alphaValues[1], alphaValues[2], alphaValues[3]),
                    modelPassParams = new Vector4(unlit, unfogged, alphakey, 0.0f),
                    animatedColor = animator.GetColorValue(pass.ColorAnimIndex)
                });

                for (var i = 0; i < pass.OpCount && i < 4; ++i)
                {
                    switch (mModel.TextureInfos[pass.TextureIndices[i]].SamplerFlags)
                    {
                        case Graphics.SamplerFlagType.WrapBoth:
	                    {
		                    gMesh.Program.SetPixelSampler(i, gSamplerWrapBoth);
		                    break;
	                    }
                        case Graphics.SamplerFlagType.WrapU:
	                    {
		                    gMesh.Program.SetPixelSampler(i, gSamplerWrapU);
		                    break;
	                    }
                        case Graphics.SamplerFlagType.WrapV:
	                    {
		                    gMesh.Program.SetPixelSampler(i, gSamplerWrapV);
		                    break;
	                    }
                        case Graphics.SamplerFlagType.ClampBoth:
	                    {
		                    gMesh.Program.SetPixelSampler(i, gSamplerClampBoth);
		                    break;
	                    }
                    }

                    gMesh.Program.SetPixelTexture(i, pass.Textures[i]);
                }

                gMesh.StartVertex = 0;
                gMesh.StartIndex = pass.StartIndex;
                gMesh.IndexCount = pass.IndexCount;
                gMesh.Draw();
            }
        }

        // TODO: Get rid of this
        public void OnFrame_Old(M2Renderer renderer, M2RenderInstance instance)
        {
            var animator = mAnimator ?? renderer.Animator;
	        if (mAnimator != null)
	        {
		        UpdateAnimations(instance);
	        }

            gMesh.UpdateIndexBuffer(renderer.IndexBuffer);
            gMesh.UpdateVertexBuffer(renderer.VertexBuffer);

            gPerDrawCallBuffer.BufferData(new PerDrawCallBuffer
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
	                {
		                continue;
	                }
                }

                var oldProgram = gMesh.Program;
                ShaderProgram newProgram;
                switch (pass.BlendMode)
                {
                    case 0:
	                {
		                newProgram = gNoBlendProgram;
		                break;
	                }
                    case 1:
					{
						newProgram = gBlendTestProgram;
						break;
					}
                    default:
					{
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
                }

                if (newProgram != oldProgram)
                {
                    gMesh.Program = newProgram;
                    gMesh.Program.Bind();
                }

                var depthState = gDepthNoWriteState;
	            if (pass.BlendMode == 0 || pass.BlendMode == 1)
	            {
		            depthState = gDepthWriteState;
	            }

                gMesh.UpdateDepthState(depthState);

                var cullingDisabled = (pass.RenderFlag & 0x04) != 0;
                gMesh.UpdateRasterizerState(cullingDisabled ? gNoCullState : gCullState);
                gMesh.UpdateBlendState(BlendStates[pass.BlendMode]);

                var unlit = ((pass.RenderFlag & 0x01) != 0) ? 0.0f : 1.0f;
                var unfogged = ((pass.RenderFlag & 0x02) != 0) ? 0.0f : 1.0f;

                Matrix4 uvAnimMat;
                animator.GetUvAnimMatrix(pass.TexAnimIndex, out uvAnimMat);
                var color = animator.GetColorValue(pass.ColorAnimIndex);
                var alpha = animator.GetAlphaValue(pass.AlphaAnimIndex);
                color.W *= alpha;

                gPerPassBuffer.BufferData(new PerModelPassBuffer
                {
                    uvAnimMatrix1 = uvAnimMat,
                    modelPassParams = new Vector4(unlit, unfogged, 0.0f, 0.0f),
                    animatedColor = color
                });

	            for (var i = 0; i < pass.Textures.Count && i < 3; ++i)
	            {
		            gMesh.Program.SetPixelTexture(i, pass.Textures[i]);
	            }

                gMesh.StartVertex = 0;
                gMesh.StartIndex = pass.StartIndex;
                gMesh.IndexCount = pass.IndexCount;
                gMesh.Draw();
            }
        }

        private void UpdateAnimations(M2RenderInstance instance)
        {
            var camera = WorldFrame.Instance.ActiveCamera;
            mAnimator.Update(new BillboardParameters
            {
                Forward = camera.Forward,
                Right = camera.Right,
                Up = camera.Up,
                InverseRotation = instance.InverseRotation
            });

	        if (mAnimator.GetBones(mAnimationMatrices))
	        {
		        mAnimBuffer.BufferData(mAnimationMatrices);
	        }
        }

        public void OnSyncLoad()
        {
            if (mAnimator != null)
            {
                mAnimBuffer = new UniformBuffer();
                mAnimBuffer.BufferData(mAnimationMatrices);
            }
        }

        public static void Initialize(GxContext context)
        {
            gDepthWriteState = new DepthState
            {
                DepthEnabled = true,
                DepthWriteEnabled = true
            };

            gDepthNoWriteState = new DepthState
            {
                DepthEnabled = true,
                DepthWriteEnabled = false
            };

            gMesh = new Mesh
            {
                Stride = IO.SizeCache<M2Vertex>.Size,
                DepthState = gDepthNoWriteState
            };

            gMesh.IndexBuffer.Dispose();
            gMesh.VertexBuffer.Dispose();

            gMesh.IndexBuffer = null;
            gMesh.VertexBuffer = null;

            gMesh.AddElement("POSITION", 0, 3);
            gMesh.AddElement("BLENDWEIGHT", 0, 4, DataType.Byte, true);
            gMesh.AddElement("BLENDINDEX", 0, 4, DataType.Byte);
            gMesh.AddElement("NORMAL", 0, 3);
            gMesh.AddElement("TEXCOORD", 0, 2);
            gMesh.AddElement("TEXCOORD", 1, 2);

            // all combinations are set in this one each time
            gCustomProgram = new ShaderProgram(context);
            gCustomProgram.SetVertexShader(Resources.Shaders.M2VertexSingle_VS_Diffuse_T1);
            gCustomProgram.SetPixelShader(Resources.Shaders.M2Fragment_PS_Combiners_Mod);

            gMesh.Program = gCustomProgram;

            gNoBlendProgram = new ShaderProgram(context);
            gNoBlendProgram.SetPixelShader(Resources.Shaders.M2FragmentOld);
            gNoBlendProgram.SetVertexShader(Resources.Shaders.M2VertexSingleOld);

            gBlendProgram = new ShaderProgram(context);
            gBlendProgram.SetPixelShader(Resources.Shaders.M2FragmentBlendOld);
            gBlendProgram.SetVertexShader(Resources.Shaders.M2VertexSingleOld);

            gBlendTestProgram = new ShaderProgram(context);
            gBlendTestProgram.SetPixelShader(Resources.Shaders.M2FragmentBlendAlphaOld);
            gBlendTestProgram.SetVertexShader(Resources.Shaders.M2VertexSingleOld);

            g2PassProgram = new ShaderProgram(context);
            g2PassProgram.SetPixelShader(Resources.Shaders.M2Fragment2PassOld);
            g2PassProgram.SetVertexShader(Resources.Shaders.M2VertexSingleOld);

            g3PassProgram = new ShaderProgram(context);
            g3PassProgram.SetPixelShader(Resources.Shaders.M2Fragment3PassOld);
            g3PassProgram.SetVertexShader(Resources.Shaders.M2VertexSingleOld);

            gPerDrawCallBuffer = new UniformBuffer();
            gPerDrawCallBuffer.BufferData(new PerDrawCallBuffer
            {
                instanceMat = Matrix4.Identity
            });

            gPerPassBuffer = new UniformBuffer();

            gPerPassBuffer.BufferData(new PerModelPassBuffer
            {
                uvAnimMatrix1 = Matrix4.Identity,
                uvAnimMatrix2 = Matrix4.Identity,
                uvAnimMatrix3 = Matrix4.Identity,
                uvAnimMatrix4 = Matrix4.Identity,
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
	        {
		        BlendStates[i] = new BlendState();
	        }

	        BlendStates[0] = new BlendState
	        {
		        BlendEnabled = false
	        };

	        BlendStates[1] = new BlendState
	        {
		        BlendEnabled = true,
		        SourceBlend = BlendingFactorSrc.One,
		        DestinationBlend = BlendingFactorDest.Zero,
		        SourceAlphaBlend = BlendingFactorSrc.One,
		        DestinationAlphaBlend = BlendingFactorDest.Zero
	        };

	        BlendStates[2] = new BlendState
	        {
		        BlendEnabled = true,
		        SourceBlend = BlendingFactorSrc.SrcAlpha,
		        DestinationBlend = BlendingFactorDest.OneMinusSrcAlpha,
		        SourceAlphaBlend = BlendingFactorSrc.SrcAlpha,
		        DestinationAlphaBlend = BlendingFactorDest.OneMinusSrcAlpha
	        };

	        BlendStates[3] = new BlendState
	        {
		        BlendEnabled = true,
		        // INVESTIGATE: Workaround for enum value not present
		        SourceBlend = (BlendingFactorSrc) BlendingFactorDest.SrcColor,
		        // INVESTIGATE: Workaround for enum value not present
		        DestinationBlend = (BlendingFactorDest) BlendingFactorSrc.DstColor,
		        SourceAlphaBlend = BlendingFactorSrc.SrcAlpha,
		        DestinationAlphaBlend = BlendingFactorDest.DstAlpha
	        };

	        BlendStates[4] = new BlendState(
	        {
		        BlendEnabled = true,
		        SourceBlend = BlendingFactorSrc.SrcAlpha,
		        DestinationBlend = BlendingFactorDest.One,
		        SourceAlphaBlend = BlendingFactorSrc.SrcAlpha,
		        DestinationAlphaBlend = BlendingFactorDest.One
	        };

	        BlendStates[5] = new BlendState
	        {
		        BlendEnabled = true,
		        SourceBlend = BlendingFactorSrc.SrcAlpha,
		        DestinationBlend = BlendingFactorDest.OneMinusSrcAlpha,
		        SourceAlphaBlend = BlendingFactorSrc.SrcAlpha,
		        DestinationAlphaBlend = BlendingFactorDest.OneMinusSrcAlpha
	        };

	        BlendStates[6] = new BlendState
	        {
		        BlendEnabled = true,
		        SourceBlend = BlendingFactorSrc.DstColor,
		        DestinationBlend = BlendingFactorDest.SrcColor,
		        SourceAlphaBlend = BlendingFactorSrc.DstAlpha,
		        DestinationAlphaBlend = BlendingFactorDest.SrcAlpha
	        };

	        gNoCullState = new RasterState
            {
	            BackfaceCullingEnabled = false
            };
            gCullState = new RasterState
            {
	            BackfaceCullingEnabled = true
            };
        }
    }
}
