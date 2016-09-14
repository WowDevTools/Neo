using System;
using System.Collections.Generic;
using System.IO;
using SharpDX;

namespace Neo.IO.Files.Models
{
    class TextureInfo
    {
        public int TextureType { get; set; }
        public Graphics.Texture.SamplerFlagType SamplerFlags { get; set; }
        public Graphics.Texture Texture { get; set; }
    } 

    abstract class M2File
    {
        protected M2SubMeshInfo[] mSubMeshes = new M2SubMeshInfo[0];

        public M2Vertex[] Vertices { get; protected set; }
        public List<M2RenderPass> Passes { get; private set; }
        public ushort[] Indices { get; protected set; }

        public BoundingBox BoundingBox { get; protected set; }
        public bool HasBlendPass { get; protected set; }
        public bool HasOpaquePass { get; protected set; }

        public TextureInfo[] TextureInfos { get; protected set; }

        public string ModelRoot { get; private set; }

        public bool NeedsPerInstanceAnimation { get; protected set; }

        public float BoundingRadius { get; protected set; }

        public short[] AnimationLookup { get; protected set; }

        public ushort[] AnimationIds { get; protected set; }

        public abstract string ModelName { get; }
        public string FileName { get; protected set; }

        protected M2File(string path)
        {
            FileName = path ?? "";
            ModelRoot = Path.GetDirectoryName(path) ?? "";
            TextureInfos = new TextureInfo[0];
            Vertices = new M2Vertex[0];
            Passes = new List<M2RenderPass>();
            Indices = new ushort[0];
        }

        public abstract int GetNumberOfBones();

        public abstract bool Load();

        public bool Intersect(ref Ray ray, out float distance)
        {
            distance = float.MaxValue;

            var orig = ray.Position;
            var dir = ray.Direction;
            Vector3 e1, e2, p, T, q;

            var hasHit = false;

            foreach (var submesh in mSubMeshes)
            {
                //if (ray.Intersects(ref submesh.BoundingSphere) == false)
                //    continue;

                for (var i = submesh.StartIndex; i < submesh.StartIndex + submesh.NumIndices; i += 3)
                {
                    var i0 = Indices[i];
                    var i1 = Indices[i + 1];
                    var i2 = Indices[i + 2];
                    Vector3.Subtract(ref Vertices[i1].position, ref Vertices[i0].position, out e1);
                    Vector3.Subtract(ref Vertices[i2].position, ref Vertices[i0].position, out e2);

                    Vector3.Cross(ref dir, ref e2, out p);
                    float det;
                    Vector3.Dot(ref e1, ref p, out det);

                    if (Math.Abs(det) < 1e-4)
                        continue;

                    var invDet = 1.0f / det;
                    Vector3.Subtract(ref orig, ref Vertices[i0].position, out T);
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
    }
}
