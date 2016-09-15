using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Neo.Graphics;
using OpenTK;
using Warcraft.Core;

namespace Neo.Scene.Models
{
    class BoundingBoxInstance : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        struct BoundingVertex
        {
            public Vector3 position;
            public Vector3 texCoord;
        }

        private Box mBox;
        private VertexBuffer mVertexBuffer;

        public BoundingBoxInstance(Box box)
        {
            mBox = box;
            BuildVertices();
        }

        public BoundingBoxInstance(Vector3[] corners)
        {
            BuildVertices(corners);
        }

        public void Dispose()
        {
            if (mVertexBuffer != null)
                mVertexBuffer.Dispose();
            mVertexBuffer = null;
        }

        public void OnFrame(Mesh mesh)
        {
            mesh.UpdateVertexBuffer(mVertexBuffer);
            mesh.Draw();
        }

        public void UpdateBoundingBox(IReadOnlyList<Vector3> positions)
        {
            BuildVertices(positions);
        }

        private void BuildVertices()
        {
            var minimum = mBox.BottomCorner;
            var maximum = mBox.TopCorner;

            #region Vertex Building
            var vertices = new[]
            {
                new BoundingVertex
                {
                    position = minimum,
                    texCoord = new Vector3(0, 0, 0)
                },
                new BoundingVertex
                {
                    position = new Vector3(maximum.X, minimum.Y, minimum.Z),
                    texCoord = new Vector3(1, 0, 0)
                },
                new BoundingVertex
                {
                    position = new Vector3(maximum.X, maximum.Y, minimum.Z),
                    texCoord = new Vector3(1, 1, 0)
                },
                new BoundingVertex
                {
                    position = new Vector3(minimum.X, maximum.Y, minimum.Z),
                    texCoord = new Vector3(0, 1, 0)
                },
                new BoundingVertex
                {
                    position = new Vector3(minimum.X, minimum.Y, maximum.Z),
                    texCoord = new Vector3(0, 0, 1)
                },
                new BoundingVertex
                {
                    position = new Vector3(maximum.X, minimum.Y, maximum.Z),
                    texCoord = new Vector3(1, 0, 1)
                },
                new BoundingVertex
                {
                    position = maximum,
                    texCoord = new Vector3(1, 1, 1)
                },
                new BoundingVertex
                {
                    position = new Vector3(minimum.X, maximum.Y, maximum.Z),
                    texCoord = new Vector3(0, 1, 1)
                }
            };
            #endregion

            mVertexBuffer = new VertexBuffer(WorldFrame.Instance.GraphicsContext);
            mVertexBuffer.UpdateData(vertices);
        }

        private void BuildVertices(IReadOnlyList<Vector3> positions)
        {
            var vertices = new[]
            {
                new BoundingVertex {position = positions[0], texCoord = new Vector3(0, 0, 0)},
                new BoundingVertex {position = positions[1], texCoord = new Vector3(1, 0, 0)},
                new BoundingVertex {position = positions[2], texCoord = new Vector3(1, 1, 0)},
                new BoundingVertex {position = positions[3], texCoord = new Vector3(0, 1, 0)},
                new BoundingVertex {position = positions[4], texCoord = new Vector3(0, 0, 1)},
                new BoundingVertex {position = positions[5], texCoord = new Vector3(1, 0, 1)},
                new BoundingVertex {position = positions[6], texCoord = new Vector3(1, 1, 1)},
                new BoundingVertex {position = positions[7], texCoord = new Vector3(0, 1, 1)}
            };

            if (mVertexBuffer == null)
                mVertexBuffer = new VertexBuffer(WorldFrame.Instance.GraphicsContext);

            mVertexBuffer.UpdateData(vertices);
        }
    }

    class BoundingBoxDrawManager
    {
        private static IndexBuffer gIndexBuffer;
        private static Mesh gMesh;
        private readonly List<BoundingBoxInstance> mBoundingBoxes = new List<BoundingBoxInstance>();

        public void OnFrame()
        {
            gMesh.BeginDraw();

            foreach (var bbox in mBoundingBoxes)
            {
                bbox.OnFrame(gMesh);
            }
        }

        public BoundingBoxInstance AddDrawableBox(Box box)
        {
            var bbox = new BoundingBoxInstance(box);
            mBoundingBoxes.Add(bbox);
            return bbox;
        }

        public BoundingBoxInstance AddDrawableBox(Vector3[] corners)
        {
            var bbox = new BoundingBoxInstance(corners);
            mBoundingBoxes.Add(bbox);
            return bbox;
        }

        public void RemoveDrawableBox(BoundingBoxInstance instance)
        {
            mBoundingBoxes.Remove(instance);
            instance.Dispose();
        }

        public static void Initialize()
        {
            var indices = new ushort[]
            {
                0, 1, 2,
                0, 2, 3,
                0, 1, 5,
                0, 5, 4,
                3, 0, 4,
                3, 4, 7,
                2, 3, 7,
                2, 7, 6,
                1, 2, 6,
                1, 6, 5,
                4, 5, 6,
                4, 6, 7
            };

            gIndexBuffer = new IndexBuffer(WorldFrame.Instance.GraphicsContext);
            gIndexBuffer.UpdateData(indices);
            gIndexBuffer.IndexFormat = Format.R16_UInt;

            var program = new ShaderProgram(WorldFrame.Instance.GraphicsContext);
            program.SetVertexShader(Resources.Shaders.BoundingBoxVertex);
            program.SetPixelShader(Resources.Shaders.BoundingBoxPixel);

            gMesh = new Mesh(WorldFrame.Instance.GraphicsContext)
            {
                IndexBuffer = gIndexBuffer,
                IndexCount = 36,
                Stride = 24,
                BlendState = {BlendEnabled = true},
                DepthState = {DepthEnabled = true}
            };

            gMesh.AddElement("POSITION", 0, 3);
            gMesh.AddElement("TEXCOORD", 0, 3);

            gMesh.Program = program;
            gMesh.InitLayout(program);
        }
    }
}
