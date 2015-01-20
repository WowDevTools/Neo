using System;
using System.Runtime.InteropServices;
using SharpDX;
using WoWEditor6.Graphics;

namespace WoWEditor6.Scene.Terrain
{
    class SkySphere
    {
        [StructLayout(LayoutKind.Sequential)]
        struct SphereVertex
        {
            public Vector3 Position;
            public Vector2 TexCoord;
        }

        private SphereVertex[] mVertices;
        private readonly ConstantBuffer mMatrixBuffer;
        private Graphics.Texture mSkyTexture;
        private readonly Sampler mSampler;
        private readonly Mesh mMesh;

        public SkySphere(float radius, int rings, int sectors, GxContext context)
        {
            mSampler = new Sampler(context);
            mMesh = new Mesh(context);
            mMesh.AddElement("POSITION", 0, 3);
            mMesh.AddElement("TEXCOORD", 0, 2);
            mMesh.BlendState.BlendEnabled = false;
            mMesh.DepthState.DepthEnabled = true;
            mMesh.Stride = IO.SizeCache<SphereVertex>.Size;

            InitVertices(radius, rings, sectors);

            mMesh.VertexBuffer.UpdateData(mVertices);

            mMatrixBuffer = new ConstantBuffer(context);
            mMatrixBuffer.UpdateData(Matrix.Identity);

            var program = new ShaderProgram(context);
            program.SetVertexShader(Resources.Shaders.SkyVertex, "main");
            program.SetPixelShader(Resources.Shaders.SkyPixel, "main");

            mMesh.Program = program;
        }

        public void Render()
        {
            if (mSkyTexture == null)
                return;

            mMesh.Program.SetVertexConstantBuffer(1, mMatrixBuffer);
            mMesh.Program.SetPixelTexture(0, mSkyTexture);
            mMesh.Program.SetPixelSampler(0, mSampler);
            mMesh.BeginDraw();
            mMesh.Draw();
        }

        public void UpdatePosition(Vector3 position)
        {
            mMatrixBuffer.UpdateData(new Vector4(position, 1.0f));
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
            mMesh.IndexBuffer.IndexFormat = SharpDX.DXGI.Format.R32_UInt;
            mMesh.IndexBuffer.UpdateData(indices);
        }
    }
}
