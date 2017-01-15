using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using SlimTK;

namespace Neo.IO.Files.Models
{
	public class TextureInfo
    {
        public TextureType TextureType { get; set; }
        public Graphics.SamplerFlagType SamplerFlags { get; set; }
        public Graphics.Texture Texture { get; set; }
    }

    public class CreatureDisplayInfo
    {
        public int TextureVariation { get; set; }
        public int SkinId { get; set; }
        public int FaceId { get; set; }
        public int HairStyleId { get; set; }
        public int HairColorId { get; set; }
        public int FacialHairId { get; set; }

        public List<Tuple<string, string, string>> TextureVariationFiles { get; set; }
        public int[] SkinOptions { get; set; }
        public int[] FaceOptions { get; set; }
        public int[] HairStyleOptions { get; set; }
        public int[] HairColorOptions { get; set; }
        public int[] FacialHairOptions { get; set; }
    }

    public abstract class M2File
    {
        protected M2SubMeshInfo[] mSubMeshes = new M2SubMeshInfo[0];

        public M2Vertex[] Vertices { get; protected set; }
        public List<M2RenderPass> Passes { get; private set; }
        public ushort[] Indices { get; protected set; }

        public BoundingBox BoundingBox { get; protected set; }
        public bool HasBlendPass { get; protected set; }
        public bool HasOpaquePass { get; protected set; }

        public TextureInfo[] TextureInfos { get; protected set; }
        public CreatureDisplayInfo DisplayOptions { get; set; }

        public string ModelRoot { get; private set; }

        public bool NeedsPerInstanceAnimation { get; protected set; }

        public float BoundingRadius { get; protected set; }

        public short[] AnimationLookup { get; protected set; }

        public ushort[] AnimationIds { get; protected set; }

        public abstract string ModelName { get; }
        public string FileName { get; protected set; }

        protected M2File(string path)
        {
	        this.FileName = path ?? "";
	        this.ModelRoot = Path.GetDirectoryName(path) ?? "";
	        this.TextureInfos = new TextureInfo[0];
	        this.Vertices = new M2Vertex[0];
	        this.Passes = new List<M2RenderPass>();
	        this.Indices = new ushort[0];
	        this.DisplayOptions = new CreatureDisplayInfo();
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

            foreach (var submesh in this.mSubMeshes)
            {
                //if (ray.Intersects(ref submesh.BoundingSphere) == false)
                //    continue;

                for (var i = submesh.StartIndex; i < submesh.StartIndex + submesh.NumIndices; i += 3)
                {
                    var i0 = this.Indices[i];
                    var i1 = this.Indices[i + 1];
                    var i2 = this.Indices[i + 2];
                    Vector3.Subtract(ref this.Vertices[i1].position, ref this.Vertices[i0].position, out e1);
                    Vector3.Subtract(ref this.Vertices[i2].position, ref this.Vertices[i0].position, out e2);

                    Vector3.Cross(ref dir, ref e2, out p);
                    float det;
                    Vector3.Dot(ref e1, ref p, out det);

<<<<<<< HEAD
	                if (Math.Abs(det) < 1e-4)
	                {
		                continue;
	                }
=======
                    if (Math.Abs(det) < 1e-4)
                    {
	                    continue;
                    }
>>>>>>> 0cbb631de4b61218f0a1263b2ba9bc29366cbb1e

	                var invDet = 1.0f / det;
                    Vector3.Subtract(ref orig, ref this.Vertices[i0].position, out T);
                    float u;
                    Vector3.Dot(ref T, ref p, out u);
                    u *= invDet;

<<<<<<< HEAD
	                if (u < 0 || u > 1)
	                {
		                continue;
	                }
=======
                    if (u < 0 || u > 1)
                    {
	                    continue;
                    }
>>>>>>> 0cbb631de4b61218f0a1263b2ba9bc29366cbb1e

	                Vector3.Cross(ref T, ref e1, out q);
                    float v;
                    Vector3.Dot(ref dir, ref q, out v);
                    v *= invDet;
<<<<<<< HEAD
	                if (v < 0 || (u + v) > 1)
	                {
		                continue;
	                }
=======
                    if (v < 0 || (u + v) > 1)
                    {
	                    continue;
                    }
>>>>>>> 0cbb631de4b61218f0a1263b2ba9bc29366cbb1e

	                float t;
                    Vector3.Dot(ref e2, ref q, out t);
                    t *= invDet;

<<<<<<< HEAD
	                if (t < 1e-4)
	                {
		                continue;
	                }

                    hasHit = true;
	                if (t < distance)
	                {
		                distance = t;
	                }
=======
                    if (t < 1e-4)
                    {
	                    continue;
                    }

	                hasHit = true;
                    if (t < distance)
                    {
	                    distance = t;
                    }
>>>>>>> 0cbb631de4b61218f0a1263b2ba9bc29366cbb1e
                }
            }

            return hasHit;
        }

    }

    public enum TextureType : uint
    {
        None = 0,
        Skin = 1,
        ObjectSkin = 2,
        WeaponBlade = 3,
        WeaponHandle = 4,
        Environment = 5,
        CharacterHair = 6,
        CharacterFacialHair = 7,
        SkinExtra = 8,
        UiSkin = 9,
        TaurenMane = 10,
        MonsterSkin1 = 11,
        MonsterSkin2 = 12,
        MonsterSkin3 = 13,
        ItemIcon = 14,
        GuildBackgroundColor = 15,
        GuildEmblemColor = 16,
        GuildBorderColor = 17,
        GuildEmblem = 18
    }

}
