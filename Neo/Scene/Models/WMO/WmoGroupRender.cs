using System;
using System.Collections.Generic;
using Neo.Graphics;
using Warcraft.Core;
using OpenTK;

namespace Neo.Scene.Models.WMO
{
    class WmoGroupRender : IDisposable
    {
        class WmoRenderBatch
        {
            public IO.Files.Models.WmoMaterial Material;
            public IO.Files.Models.WmoBatch Batch;
        }

        public static Mesh Mesh { get; private set; }

        private static BlendState gNoBlendState;
        private static BlendState gAlphaBlendState;
        private static ShaderProgram gBlendProgram;
        private static ShaderProgram gNoBlendProgram;
        private static ShaderProgram gIndoorBlendProgram;
        private static ShaderProgram gIndoorNoBlendProgram;
        private static RasterState gNoCullState;
        private static RasterState gCullState;

        public static Sampler Sampler { get; private set; }
        public static ConstantBuffer InstanceBuffer { get; private set; }

        private List<WmoRenderBatch> mBatches = new List<WmoRenderBatch>();

        private bool mLoaded;

        public int BaseIndex { get; set; }
        public int BaseVertex { get; set; }

        public IO.Files.Models.WmoGroup Data { get; private set; }

        public Box BoundingBox { get { return Data.BoundingBox; } }

        public WmoGroupRender(IO.Files.Models.WmoGroup group, WmoRootRender root)
        {
            Data = group;
            foreach(var batch in Data.Batches)
            {
                mBatches.Add(new WmoRenderBatch
                {
                    Batch = batch,
                    Material = root.Data.GetMaterial(batch.MaterialId)
                });
            }
        }

        ~WmoGroupRender()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (mBatches != null)
            {
                mBatches.Clear();
                mBatches = null;
            }

            Data = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void OnFrame()
        {
            if (mLoaded == false)
                return;

            if (Data.DisableRendering)
                return;

            Mesh.StartVertex = BaseVertex;

            foreach(var batch in mBatches)
            {
                SetupBatch(batch);
                Mesh.StartIndex = BaseIndex + batch.Batch.StartIndex;
                Mesh.IndexCount = batch.Batch.NumIndices;

                Mesh.Draw();
            }
        }

        public void SyncLoad()
        {
            mLoaded = true;
        }

        public bool Intersects(IntersectionParams parameters, ref Ray ray, out float distance)
        {
            distance = float.MaxValue;
            var hasHit = false;

            var orig = ray.Position;
            var dir = ray.Direction;
            Vector3 e1, e2, p, T, q;

            foreach (var batch in mBatches)
            {
                for (int i = batch.Batch.StartIndex, j = 0; j < batch.Batch.NumIndices; i += 3, j += 3)
                {
                    var i0 = Data.Indices[i];
                    var i1 = Data.Indices[i + 1];
                    var i2 = Data.Indices[i + 2];
                    Vector3.Subtract(ref Data.Vertices[i1].Position, ref Data.Vertices[i0].Position, out e1);
                    Vector3.Subtract(ref Data.Vertices[i2].Position, ref Data.Vertices[i0].Position, out e2);

                    Vector3.Cross(ref dir, ref e2, out p);
                    float det;
                    Vector3.Dot(ref e1, ref p, out det);

                    if (Math.Abs(det) < 1e-4)
                        continue;

                    var invDet = 1.0f / det;
                    Vector3.Subtract(ref orig, ref Data.Vertices[i0].Position, out T);
                    float u;
                    Vector3.Dot(ref T, ref p, out u);
                    u *= invDet;

                    if (u < 0 || u > 1)
                        continue;

                    Vector3.Cross(ref T, ref e1, out q);
                    float v;
                    Vector3.Dot(ref dir, ref q, out v);
                    v *= invDet;
                    if (v < 0 || (u + v) > 1)
                        continue;

                    float t;
                    Vector3.Dot(ref e2, ref q, out t);
                    t *= invDet;

                    if (t < 1e-4) continue;

                    hasHit = true;
                    if (t < distance)
                        distance = t;
                }
            }

            return hasHit;
        }

        private void SetupBatch(WmoRenderBatch batch)
        {
            var cullingDisabled = (batch.Material.MaterialFlags & 0x04) != 0;
            Mesh.UpdateRasterizerState(cullingDisabled ? gNoCullState : gCullState);
            Mesh.UpdateBlendState((batch.Batch.BlendMode != 0) ? gAlphaBlendState : gNoBlendState);
            ShaderProgram newProgram;
            if(Data.IsIndoor)
                newProgram = (batch.Batch.BlendMode != 0) ? gIndoorBlendProgram : gIndoorNoBlendProgram;
            else
                newProgram = (batch.Batch.BlendMode != 0) ? gBlendProgram : gNoBlendProgram;

            if(newProgram != Mesh.Program)
            {
                Mesh.Program = newProgram;
                Mesh.Program.Bind();
            }
            Mesh.Program.SetPixelTextures(0, batch.Material.Textures);
        }

        public static void Initialize(GxContext context)
        {
            gNoBlendState = new BlendState(context) { BlendEnabled = false };
            gAlphaBlendState = new BlendState(context) { BlendEnabled = true };

            gNoCullState = new RasterState(context) {CullEnabled = false};
            gCullState = new RasterState(context) {CullEnabled = true};

            Sampler = new Sampler(context);

            InstanceBuffer = new ConstantBuffer(context);
            InstanceBuffer.UpdateData(Matrix4.Identity); // preallocate space so the underlying buffer wont change anymore

            Mesh = new Mesh(context)
            {
                DepthState = { DepthEnabled = true },
                Stride = IO.SizeCache<IO.Files.Models.WmoVertex>.Size
            };

            Mesh.AddElement("POSITION", 0, 3);
            Mesh.AddElement("NORMAL", 0, 3);
            Mesh.AddElement("TEXCOORD", 0, 2);
            Mesh.AddElement("COLOR", 0, 4, DataType.Byte, true);

            gNoBlendProgram = new ShaderProgram(context);
            gNoBlendProgram.SetVertexShader(Resources.Shaders.WmoVertex);
            gNoBlendProgram.SetPixelShader(Resources.Shaders.WmoPixel);

            gBlendProgram = new ShaderProgram(context);
            gBlendProgram.SetVertexShader(Resources.Shaders.WmoVertex);
            gBlendProgram.SetPixelShader(Resources.Shaders.WmoPixelBlend);

            gIndoorNoBlendProgram = new ShaderProgram(context);
            gIndoorNoBlendProgram.SetVertexShader(Resources.Shaders.WmoVertex);
            gIndoorNoBlendProgram.SetPixelShader(Resources.Shaders.WmoPixelIndoor);

            gIndoorBlendProgram = new ShaderProgram(context);
            gIndoorBlendProgram.SetVertexShader(Resources.Shaders.WmoVertex);
            gIndoorBlendProgram.SetPixelShader(Resources.Shaders.WmoPixelBlendIndoor);

            Mesh.Program = gNoBlendProgram;

            Mesh.InitLayout(gNoBlendProgram);
        }
    }
}
