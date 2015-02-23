using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpDX;

namespace WoWEditor6.IO.Files.Models.WoD
{
    class M2File : Models.M2File
    {
        private string mModelName;
        private readonly string mFileName;

        private M2Header mHeader;
        private Graphics.Texture[] mTextures = new Graphics.Texture[0];
        private bool mRemapBlend;
        private ushort[] mBlendMap = new ushort[0];
        private M2SkinFile mSkin;

        public M2AnimationBone[] Bones { get; private set; }
        public M2UVAnimation[] UvAnimations { get; private set; }
        public M2TexColorAnimation[] ColorAnimations { get; private set; }
        public M2AlphaAnimation[] Transparencies { get; private set; }

        public uint[] GlobalSequences { get; private set; }
        public AnimationEntry[] Animations { get; private set; }

        public string FileName { get { return mFileName; } }

        public M2File(string fileName) : base(fileName)
        {
            Bones = new M2AnimationBone[0];
            UvAnimations = new M2UVAnimation[0];
            ColorAnimations = new M2TexColorAnimation[0];
            Transparencies = new M2AlphaAnimation[0];
            GlobalSequences = new uint[0];
            Animations = new AnimationEntry[0];
            AnimationLookup = new short[0];
            mModelName = string.Empty;
            mFileName = fileName;
        }

        public override bool Load()
        {
            using (var strm = FileManager.Instance.Provider.OpenFile(mFileName))
            {
                var reader = new BinaryReader(strm);
                mHeader = reader.Read<M2Header>();

                BoundingRadius = mHeader.VertexRadius;

                if((mHeader.GlobalFlags & 0x08) != 0)
                {
                    mRemapBlend = true;
                    var nBlendMaps = reader.Read<int>();
                    var ofsBlendMaps = reader.Read<int>();
                    strm.Position = ofsBlendMaps;
                    mBlendMap = reader.ReadArray<ushort>(nBlendMaps);
                }

                BoundingBox = new BoundingBox(mHeader.BoundingBoxMin, mHeader.BoundingBoxMax);
                BoundingSphere = new BoundingSphere(Vector3.Zero, mHeader.BoundingRadius);
                strm.Position = mHeader.OfsName;
                if (mHeader.LenName > 0)
                    mModelName = Encoding.ASCII.GetString(reader.ReadBytes(mHeader.LenName - 1));

                GlobalSequences = ReadArrayOf<uint>(reader, mHeader.OfsGlobalSequences, mHeader.NGlobalSequences);
                Vertices = ReadArrayOf<M2Vertex>(reader, mHeader.OfsVertices, mHeader.NVertices);
                var textures = ReadArrayOf<M2Texture>(reader, mHeader.OfsTextures, mHeader.NTextures);
                mTextures = new Graphics.Texture[textures.Length];
                TextureInfos = new TextureInfo[textures.Length];
                for(var i = 0; i < textures.Length; ++i)
                {
                    var tex = textures[i];
                    if (tex.type == 0 && tex.lenName > 0)
                    {
                        var texName =
                            Encoding.ASCII.GetString(ReadArrayOf<byte>(reader, tex.ofsName, tex.lenName - 1)).Trim();
                        mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture(texName);
                    }
                    else
                        mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture("default_texture");

                    TextureInfos[i] = new TextureInfo
                    {
                        Texture = mTextures[i],
                        TextureType = tex.type
                    };
                }

                LoadSkins(reader);
                LoadAnimations(reader);
            }

            return true;
        }

        private void LoadSkins(BinaryReader reader)
        {
            mSkin = new M2SkinFile(ModelRoot, mModelName, 0);
            if (mSkin.Load() == false)
                throw new InvalidOperationException("Unable to load skin file");

            Indices = mSkin.Indices;

            var texLookup = ReadArrayOf<ushort>(reader, mHeader.OfsTexLookup, mHeader.NTexLookup);
            var renderFlags = ReadArrayOf<uint>(reader, mHeader.OfsRenderFlags, mHeader.NRenderFlags);
            var uvAnimLookup = ReadArrayOf<short>(reader, mHeader.OfsUvAnimLookup, mHeader.NUvAnimLookup);
            var transLookup = ReadArrayOf<short>(reader, mHeader.OfsTransLookup, mHeader.NTransLookup);

            foreach(var texUnit in mSkin.TexUnits)
            {
                var mesh = mSkin.SubMeshes[texUnit.submeshIndex];

                int uvIndex;
                if (texUnit.textureAnim >= uvAnimLookup.Length || uvAnimLookup[texUnit.textureAnim] < 0)
                    uvIndex = -1;
                else
                    uvIndex = uvAnimLookup[texUnit.textureAnim];

                var startTriangle = (int) mesh.startTriangle;
                if ((mesh.unk1 & 1) != 0)
                    startTriangle += ushort.MaxValue + 1;

                var textures = new List<Graphics.Texture>();
                var texIndices = new List<int>();
                switch (texUnit.op_count)
                {
                    case 2:
                        textures.Add(mTextures[texLookup[texUnit.texture]]);
                        textures.Add(mTextures[texLookup[texUnit.texture + 1]]);
                        texIndices.Add(texLookup[texUnit.texture]);
                        texIndices.Add(texLookup[texUnit.texture + 1]);
                        break;
                    case 3:
                        textures.Add(mTextures[texLookup[texUnit.texture]]);
                        textures.Add(mTextures[texLookup[texUnit.texture + 1]]);
                        textures.Add(mTextures[texLookup[texUnit.texture + 2]]);
                        texIndices.Add(texLookup[texUnit.texture]);
                        texIndices.Add(texLookup[texUnit.texture + 1]);
                        texIndices.Add(texLookup[texUnit.texture + 2]);
                        break;
                    default:
                        textures.Add(mTextures[texLookup[texUnit.texture]]);
                        texIndices.Add(texLookup[texUnit.texture]);
                        break;
                }

                var flags = renderFlags[texUnit.renderFlags];
                var blendMode = flags >> 16;
                var flag = flags & 0xFFFF;

                if (mRemapBlend && texUnit.shaderId < mBlendMap.Length)
                    blendMode = mBlendMap[texUnit.shaderId];

                blendMode %= 7;

                if (blendMode != 0 && blendMode != 1)
                    HasBlendPass = true;
                else
                    HasOpaquePass = true;

                Passes.Add(new M2RenderPass
                {
                    TextureIndices = texIndices,
                    Textures = textures,
                    AlphaAnimIndex = transLookup[texUnit.transparency],
                    ColorAnimIndex = texUnit.colorIndex,
                    IndexCount = mesh.nTriangles,
                    RenderFlag = flag,
                    BlendMode = blendMode,
                    StartIndex = startTriangle,
                    TexAnimIndex = uvIndex,
                    TexUnitNumber = texUnit.texUnitNumber
                });
            }

            SortPasses();
        }

        private void LoadAnimations(BinaryReader reader)
        {
            var bones = ReadArrayOf<M2Bone>(reader, mHeader.OfsBones, mHeader.NBones);
            Bones = bones.Select(b => new M2AnimationBone(this, ref b, reader)).ToArray();

            if (Bones.Any(b => b.IsBillboarded))
                NeedsPerInstanceAnimation = true;

            AnimationLookup = ReadArrayOf<short>(reader, mHeader.OfsAnimLookup, mHeader.NAnimLookup);
            Animations = ReadArrayOf<AnimationEntry>(reader, mHeader.OfsAnimations, mHeader.NAnimations);

            var uvAnims = ReadArrayOf<M2TexAnim>(reader, mHeader.OfsUvAnimation, mHeader.NUvAnimation);
            UvAnimations = uvAnims.Select(uv => new M2UVAnimation(this, ref uv, reader)).ToArray();

            var colorAnims = ReadArrayOf<M2ColorAnim>(reader, mHeader.OfsSubmeshAnimations, mHeader.NSubmeshAnimations);
            ColorAnimations = colorAnims.Select(c => new M2TexColorAnimation(this, ref c, reader)).ToArray();

            var transparencies = ReadArrayOf<AnimationBlock>(reader, mHeader.OfsTransparencies, mHeader.NTransparencies);
            Transparencies = transparencies.Select(t => new M2AlphaAnimation(this, ref t, reader)).ToArray();
        }

        private void SortPasses()
        {
            Passes.Sort((e1, e2) =>
            {
                if (e1.BlendMode == 0 && e2.BlendMode != 0)
                    return -1;

                if (e1.BlendMode != 0 && e2.BlendMode == 0)
                    return 1;

                if (e1.BlendMode == e2.BlendMode && e1.BlendMode == 0)
                    return e1.TexUnitNumber.CompareTo(e2.TexUnitNumber);

                if (e1.BlendMode == 2 && e2.BlendMode != 2)
                    return -1;

                if (e2.BlendMode == 2 && e1.BlendMode != 2)
                    return 1;

                var is1Additive = e1.BlendMode == 1 || e1.BlendMode == 6 || e1.BlendMode == 3;
                var is2Additive = e2.BlendMode == 1 || e2.BlendMode == 6 || e2.BlendMode == 3;

                if (is1Additive && !is2Additive)
                    return -1;

                if (is2Additive && !is1Additive)
                    return 1;

                return e1.TexUnitNumber.CompareTo(e2.TexUnitNumber);
            });
        }

        public override int GetNumberOfBones()
        {
            return Bones.Length;
        }

        private static T[] ReadArrayOf<T>(BinaryReader reader, int offset, int count) where T : struct
        {
            if (count == 0)
                return new T[0];

            reader.BaseStream.Position = offset;
            return reader.ReadArray<T>(count);
        }
    }
}
