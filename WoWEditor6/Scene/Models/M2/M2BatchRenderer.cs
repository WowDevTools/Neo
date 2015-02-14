using System;
using System.Collections.Generic;
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
            public Matrix uvAnimMatrix;
            public Vector4 modelPassParams;
        }

        public static Mesh Mesh { get; private set; }
        public static Sampler Sampler { get; private set; }

        private static BlendState BlendState;

        private static ShaderProgram gNoBlendProgram;
        private static RasterState gNoCullState;
        private static RasterState gCullState;

        public M2File Model { get; private set; }

        private VertexBuffer mInstanceBuffer;
        private readonly object mInstanceBufferLock = new object();

        private int mInstanceCount;

        private PerInstanceBuffer[] mActiveInstances = new PerInstanceBuffer[0];
        private ConstantBuffer mPerPassBuffer;

        public M2BatchRenderer(M2File model)
        {
            Model = model;
        }

        public virtual void Dispose()
        {
            var ib = mInstanceBuffer;
            var pb = mPerPassBuffer;

            WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
            {
                if (ib != null)
                    ib.Dispose();
                if (pb != null)
                    pb.Dispose();
            });
        }

        public void OnFrame(M2Renderer renderer)
        {
            UpdateVisibleInstances(renderer);
            if (mInstanceCount == 0)
                return;

            Mesh.UpdateIndexBuffer(renderer.IndexBuffer);
            Mesh.UpdateVertexBuffer(renderer.VertexBuffer);
            Mesh.UpdateInstanceBuffer(mInstanceBuffer);

            Mesh.UpdateBlendState(BlendState);
            Mesh.Program.SetVertexConstantBuffer(2, renderer.AnimBuffer);
            Mesh.Program.SetVertexConstantBuffer(3, mPerPassBuffer);

            foreach (var pass in Model.Passes)
            {
                // This renderer is only for opaque pass
                if (pass.BlendMode != 0)
                    continue;

                var cullingDisabled = (pass.RenderFlag & 0x04) != 0;
                Mesh.UpdateRasterizerState(cullingDisabled ? gNoCullState : gCullState);

                var unlit = ((pass.RenderFlag & 0x01) != 0) ? 0.0f : 1.0f;
                var unfogged = ((pass.RenderFlag & 0x02) != 0) ? 0.0f : 1.0f;

                Matrix uvAnimMat;
                renderer.Animator.GetUvAnimMatrix(pass.TexAnimIndex, out uvAnimMat);

                mPerPassBuffer.UpdateData(new PerModelPassBuffer()
                {
                    uvAnimMatrix = uvAnimMat,
                    modelPassParams = new Vector4(unlit, unfogged, 0.0f, 0.0f)
                });

                Mesh.StartVertex = 0;
                Mesh.StartIndex = pass.StartIndex;
                Mesh.IndexCount = pass.IndexCount;
                Mesh.Program.SetPixelTexture(0, pass.Textures.First());
                Mesh.Draw(mInstanceCount);
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
                InstanceStride = IO.SizeCache<PerInstanceBuffer>.Size,
                DepthState = {DepthEnabled = true}
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

            Mesh.AddElement("TEXCOORD", 2, 4, DataType.Float, false, 1, true);
            Mesh.AddElement("TEXCOORD", 3, 4, DataType.Float, false, 1, true);
            Mesh.AddElement("TEXCOORD", 4, 4, DataType.Float, false, 1, true);
            Mesh.AddElement("TEXCOORD", 5, 4, DataType.Float, false, 1, true);
            Mesh.AddElement("COLOR", 0, 4, DataType.Float, false, 1, true);

            gNoBlendProgram = new ShaderProgram(context);
            gNoBlendProgram.SetVertexShader(Resources.Shaders.M2VertexInstanced);
            gNoBlendProgram.SetPixelShader(Resources.Shaders.M2Pixel);

            Mesh.Program = gNoBlendProgram;

            Sampler = new Sampler(context)
            {
                AddressMode = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                Filter = SharpDX.Direct3D11.Filter.MinMagMipLinear
            };

            BlendState = new BlendState(context)
            {
                BlendEnabled = false
            };

            gNoCullState = new RasterState(context) { CullEnabled = false };
            gCullState = new RasterState(context) { CullEnabled = true };
        }
    }
}
