using System;
using System.Collections.Generic;
using Neo.Graphics;
using Neo.Resources;
using OpenTK;
using SlimTK;

namespace Neo.Scene.Models.WMO
{
    public class WmoGroupRender : IDisposable
    {
	    private class WmoRenderBatch
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
        public static UniformBuffer InstanceBuffer { get; private set; }

        private List<WmoRenderBatch> mBatches = new List<WmoRenderBatch>();

        private bool mLoaded;

        public int BaseIndex { get; set; }
        public int BaseVertex { get; set; }

        public IO.Files.Models.IWorldModelGroup Data { get; private set; }

        public BoundingBox BoundingBox { get { return this.Data.BoundingBox; } }

        public WmoGroupRender(IO.Files.Models.IWorldModelGroup group, WmoRootRender root)
        {
	        this.Data = group;
            foreach(var batch in this.Data.Batches)
            {
	            this.mBatches.Add(new WmoRenderBatch
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
            if (this.mBatches != null)
            {
	            this.mBatches.Clear();
	            this.mBatches = null;
            }

	        this.Data = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void OnFrame()
        {
            if (this.mLoaded == false)
            {
	            return;
            }

	        if (this.Data.DisableRendering)
	        {
		        return;
	        }

	        Mesh.StartVertex = this.BaseVertex;

            foreach(var batch in this.mBatches)
            {
                SetupBatch(batch);
                Mesh.StartIndex = this.BaseIndex + batch.Batch.StartIndex;
                Mesh.IndexCount = batch.Batch.NumIndices;

                Mesh.Draw();
            }
        }

        public void SyncLoad()
        {
	        this.mLoaded = true;
        }

        public bool Intersects(IntersectionParams parameters, ref Ray ray, out float distance)
        {
            distance = float.MaxValue;
            var hasHit = false;

            var orig = ray.Position;
            var dir = ray.Direction;
            Vector3 e1, e2, p, T, q;

            foreach (var batch in this.mBatches)
            {
                for (int i = batch.Batch.StartIndex, j = 0; j < batch.Batch.NumIndices; i += 3, j += 3)
                {
                    var i0 = this.Data.Indices[i];
                    var i1 = this.Data.Indices[i + 1];
                    var i2 = this.Data.Indices[i + 2];
                    Vector3.Subtract(ref this.Data.Vertices[i1].Position, ref this.Data.Vertices[i0].Position, out e1);
                    Vector3.Subtract(ref this.Data.Vertices[i2].Position, ref this.Data.Vertices[i0].Position, out e2);

                    Vector3.Cross(ref dir, ref e2, out p);
                    float det;
                    Vector3.Dot(ref e1, ref p, out det);

                    if (Math.Abs(det) < 1e-4)
                    {
	                    continue;
                    }

	                var invDet = 1.0f / det;
                    Vector3.Subtract(ref orig, ref this.Data.Vertices[i0].Position, out T);
                    float u;
                    Vector3.Dot(ref T, ref p, out u);
                    u *= invDet;

                    if (u < 0 || u > 1)
                    {
	                    continue;
                    }

	                Vector3.Cross(ref T, ref e1, out q);
                    float v;
                    Vector3.Dot(ref dir, ref q, out v);
                    v *= invDet;
                    if (v < 0 || (u + v) > 1)
                    {
	                    continue;
                    }

	                float t;
                    Vector3.Dot(ref e2, ref q, out t);
                    t *= invDet;

                    if (t < 1e-4)
                    {
	                    continue;
                    }

	                hasHit = true;
                    if (t < distance)
                    {
	                    distance = t;
                    }
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
            if(this.Data.IsIndoor)
            {
	            newProgram = (batch.Batch.BlendMode != 0) ? gIndoorBlendProgram : gIndoorNoBlendProgram;
            }
            else
            {
	            newProgram = (batch.Batch.BlendMode != 0) ? gBlendProgram : gNoBlendProgram;
            }

	        if(newProgram != Mesh.Program)
            {
                Mesh.Program = newProgram;
                Mesh.Program.Bind();
            }
            Mesh.Program.SetFragmentTextures(0, batch.Material.Textures);
        }

        public static void Initialize()
        {
            gNoBlendState = new BlendState
            {
	            BlendEnabled = false
            };

            gAlphaBlendState = new BlendState
            {
	            BlendEnabled = true
            };

            gNoCullState = new RasterState
            {
	            BackfaceCullingEnabled = false
            };

            gCullState = new RasterState
            {
	            BackfaceCullingEnabled = true
            };

	        Sampler = new Sampler();
            InstanceBuffer = new UniformBuffer();
	        // INVESTIGATE: Preallcation most likely not needed in OpenGL
            //InstanceBuffer.BufferData(Matrix4.Identity); // preallocate space so the underlying buffer wont change anymore

            Mesh = new Mesh
            {
                DepthState = { DepthEnabled = true },
                Stride = IO.SizeCache<IO.Files.Models.WmoVertex>.Size
            };

            Mesh.AddElement("POSITION", 0, 3);
            Mesh.AddElement("NORMAL", 0, 3);
            Mesh.AddElement("TEXCOORD", 0, 2);
            Mesh.AddElement("COLOR", 0, 4, DataType.Byte, true);

            gNoBlendProgram = new ShaderProgram();
            gNoBlendProgram.SetVertexShader(Shaders.WmoVertex);
	        gNoBlendProgram.SetFragmentShader(Shaders.WmoFragment);

            gBlendProgram = new ShaderProgram();
            gBlendProgram.SetVertexShader(Shaders.WmoVertex);
            gBlendProgram.SetFragmentShader(Shaders.WmoFragmentBlend);

            gIndoorNoBlendProgram = new ShaderProgram();
            gIndoorNoBlendProgram.SetVertexShader(Shaders.WmoVertex);
            gIndoorNoBlendProgram.SetFragmentShader(Shaders.WmoFragmentIndoor);

            gIndoorBlendProgram = new ShaderProgram();
            gIndoorBlendProgram.SetVertexShader(Shaders.WmoVertex);
            gIndoorBlendProgram.SetFragmentShader(Shaders.WmoFragmentBlendIndoor);

            Mesh.Program = gNoBlendProgram;

            Mesh.InitLayout(gNoBlendProgram);
        }
    }
}
