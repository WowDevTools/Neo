using System;
using System.Runtime.InteropServices;
using Neo.Graphics;
using Neo.Resources;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SlimTK;

namespace Neo.Scene.Terrain
{
	internal class SkySphere
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct SphereVertex
        {
            public Vector3 Position;
            public Vector2 TexCoord;
        }

        private SphereVertex[] mVertices;
        private readonly UniformBuffer mMatrixBuffer;
        private Graphics.Texture mSkyTexture;
        private readonly Mesh mMesh;
        private BoundingSphere mBoundingSphere;
        private readonly float mRadius;

        public BoundingSphere BoundingSphere { get { return this.mBoundingSphere; } }

        public SkySphere(float radius, int rings, int sectors)
        {
	        this.mRadius = radius;
	        this.mBoundingSphere = new BoundingSphere(Vector3.Zero, radius);
	        this.mMesh = new Mesh();
	        this.mMesh.AddElement("POSITION", 0, 3);
	        this.mMesh.AddElement("TEXCOORD", 0, 2);
	        this.mMesh.BlendState.BlendEnabled = false;
	        this.mMesh.DepthState.DepthEnabled = true;
	        this.mMesh.Stride = IO.SizeCache<SphereVertex>.Size;

            InitVertices(radius, rings, sectors);

	        this.mMesh.VertexBuffer.BufferData(this.mVertices);

	        this.mMatrixBuffer = new UniformBuffer();
	        this.mMatrixBuffer.BufferData(Matrix4.Identity);

	        this.mMesh.Program = ShaderCache.GetShaderProgram(NeoShader.Sky);
        }

        public void Render()
        {
            if (this.mSkyTexture == null)
            {
	            return;
            }

	        this.mMesh.Program.SetVertexUniformBuffer(1, this.mMatrixBuffer);
	        this.mMesh.Program.SetFragmentTexture(0, this.mSkyTexture);
	        this.mMesh.BeginDraw();
	        this.mMesh.Draw();
        }

        public void UpdatePosition(Vector3 position)
        {
	        this.mBoundingSphere = new BoundingSphere(position, this.mRadius);
	        this.mMatrixBuffer.BufferData(new Vector4(position, 1.0f));
        }

        public void UpdateSkyTexture(Graphics.Texture tex)
        {
	        this.mSkyTexture = tex;
        }

        private void InitVertices(float radius, int rings, int sectors)
        {
            // ReSharper disable InconsistentNaming
            var R = 1.0f / (rings - 1);
            var S = 1.0f / (sectors - 1);

	        this.mVertices = new SphereVertex[rings * sectors];
            var counter = 0;

            for(var r = 0; r < rings; ++r)
            {
                for(var s = 0; s < sectors; ++s)
                {
                    var z = (float) Math.Sin(-(Math.PI / 2.0f) + Math.PI * r * R);
                    var x = (float) Math.Cos((2 * Math.PI * s * S)) * (float) Math.Sin(Math.PI * r * R);
                    var y = (float) Math.Sin((2 * Math.PI * s * S)) * (float) Math.Sin(Math.PI * r * R);

	                this.mVertices[counter++] = new SphereVertex
                    {
                        Position = new Vector3(x * radius, y * radius, z * radius),
                        TexCoord = new Vector2(s * S, r * R)
                    };
                }
            }

            var indices = new uint[rings * sectors * 6];
            counter = 0;
            for (uint r = 0; r < rings - 1; ++r)
            {
                for (uint s = 0; s < sectors - 1; ++s)
                {
                    indices[counter++] = (uint)(r * sectors + s);
                    indices[counter++] = (uint) (r * sectors + s + 1);
                    indices[counter++] = (uint) ((r + 1) * sectors + s + 1);
                    indices[counter++] = (uint)(r * sectors + s);
                    indices[counter++] = (uint)((r + 1) * sectors + s + 1);
                    indices[counter++] = (uint)((r + 1) * sectors + s);
                }
            }

	        this.mMesh.IndexCount = indices.Length;
	        this.mMesh.IndexBuffer.IndexFormat = DrawElementsType.UnsignedInt;
	        this.mMesh.IndexBuffer.BufferData(indices);
        }
    }
}
