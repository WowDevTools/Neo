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

        public BoundingSphere BoundingSphere { get { return mBoundingSphere; } }

        public SkySphere(float radius, int rings, int sectors)
        {
            mRadius = radius;
            mBoundingSphere = new BoundingSphere(Vector3.Zero, radius);
            mMesh = new Mesh();
            mMesh.AddElement("POSITION", 0, 3);
            mMesh.AddElement("TEXCOORD", 0, 2);
            mMesh.BlendState.BlendEnabled = false;
            mMesh.DepthState.DepthEnabled = true;
            mMesh.Stride = IO.SizeCache<SphereVertex>.Size;

            InitVertices(radius, rings, sectors);

            mMesh.VertexBuffer.BufferData(mVertices);

            mMatrixBuffer = new UniformBuffer();
            mMatrixBuffer.BufferData(Matrix4.Identity);

            mMesh.Program = ShaderCache.GetShaderProgram(NeoShader.Sky);
        }

        public void Render()
        {
            if (mSkyTexture == null)
            {
	            return;
            }

	        mMesh.Program.SetVertexUniformBuffer(1, mMatrixBuffer);
            mMesh.Program.SetFragmentTexture(0, mSkyTexture);
            mMesh.BeginDraw();
            mMesh.Draw();
        }

        public void UpdatePosition(Vector3 position)
        {
            mBoundingSphere = new BoundingSphere(position, mRadius);
            mMatrixBuffer.BufferData(new Vector4(position, 1.0f));
        }

        public void UpdateSkyTexture(Graphics.Texture tex)
        {
            mSkyTexture = tex;
        }

        private void InitVertices(float radius, int rings, int sectors)
        {
            // ReSharper disable InconsistentNaming
            var R = 1.0f / (rings - 1);
            var S = 1.0f / (sectors - 1);

            mVertices = new SphereVertex[rings * sectors];
            var counter = 0;

            for(var r = 0; r < rings; ++r)
            {
                for(var s = 0; s < sectors; ++s)
                {
                    var z = (float) Math.Sin(-(Math.PI / 2.0f) + Math.PI * r * R);
                    var x = (float) Math.Cos((2 * Math.PI * s * S)) * (float) Math.Sin(Math.PI * r * R);
                    var y = (float) Math.Sin((2 * Math.PI * s * S)) * (float) Math.Sin(Math.PI * r * R);

                    mVertices[counter++] = new SphereVertex
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

            mMesh.IndexCount = indices.Length;
	        mMesh.IndexBuffer.IndexFormat = DrawElementsType.UnsignedInt;
	        mMesh.IndexBuffer.BufferData(indices);
        }
    }
}
