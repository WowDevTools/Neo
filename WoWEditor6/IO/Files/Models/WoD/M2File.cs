using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpDX;

namespace WoWEditor6.IO.Files.Models.WoD
{
    class M2File
    {
        private string mModelName;
        private readonly string mRootPath;
        private readonly string mFileName;

        private BoundingBox mBoundingBox;

        private M2Header mHeader;
        private M2Vertex[] mVertices = new M2Vertex[0];
        private Graphics.Texture[] mTextures = new Graphics.Texture[0];
        private AnimationEntry[] mAnimations = new AnimationEntry[0];
        private short[] mUVAnimLookup = new short[0];
        private bool mRemapBlend;
        private ushort[] mBlendMap = new ushort[0];
        private M2SkinFile mSkin;
        private short[] mAnimLookup = new short[0];
        private readonly List<M2RenderPass> mPasses = new List<M2RenderPass>();

        public uint[] GlobalSequences { get; private set; } = new uint[0];

        public M2File(string fileName)
        {
            mFileName = fileName;
            mRootPath = Path.GetDirectoryName(mFileName);
        }

        public bool Load()
        {
            using (var strm = FileManager.Instance.Provider.OpenFile(mFileName))
            {
                var reader = new BinaryReader(strm);
                mHeader = reader.Read<M2Header>();

                if((mHeader.GlobalFlags & 0x08) != 0)
                {
                    mRemapBlend = true;
                    var nBlendMaps = reader.Read<int>();
                    var ofsBlendMaps = reader.Read<int>();
                    strm.Position = ofsBlendMaps;
                    mBlendMap = reader.ReadArray<ushort>(nBlendMaps);
                }

                mBoundingBox = new BoundingBox(mHeader.BoundingBoxMin, mHeader.BoundingBoxMax);
                strm.Position = mHeader.OfsName;
                mModelName = Encoding.ASCII.GetString(reader.ReadBytes(mHeader.LenName)).Trim();

                GlobalSequences = ReadArrayOf<uint>(reader, mHeader.OfsGlobalSequences, mHeader.NGlobalSequences);
                mVertices = ReadArrayOf<M2Vertex>(reader, mHeader.OfsVertices, mHeader.NVertices);
                var textures = ReadArrayOf<M2Texture>(reader, mHeader.OfsTextures, mHeader.NTextures);
                mTextures = new Graphics.Texture[textures.Length];
                for(var i = 0; i < textures.Length; ++i)
                {
                    var tex = textures[i];
                    if (tex.type == 0)
                    {
                        var texName =
                            Encoding.ASCII.GetString(ReadArrayOf<byte>(reader, tex.ofsName, tex.lenName)).Trim();
                        mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture(texName);
                    }
                    else
                        mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture("default_texture");
                }

                LoadSkins(reader);
            }

            return true;
        }

        private void LoadSkins(BinaryReader reader)
        {
            mSkin = new M2SkinFile(mRootPath, mModelName, 0);
            if (mSkin.Load() == false)
                throw new InvalidOperationException("Unable to load skin file");

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
                switch (texUnit.op_count)
                {
                    case 2:
                        textures.Add(mTextures[texLookup[texUnit.texture]]);
                        textures.Add(mTextures[texLookup[texUnit.texture + 1]]);
                        break;
                    case 3:
                        textures.Add(mTextures[texLookup[texUnit.texture]]);
                        textures.Add(mTextures[texLookup[texUnit.texture + 1]]);
                        textures.Add(mTextures[texLookup[texUnit.texture + 2]]);
                        break;
                    default:
                        textures.Add(mTextures[texLookup[texUnit.texture]]);
                        break;
                }

                mPasses.Add(new M2RenderPass
                {
                    Textures = textures,
                    AlphaAnimIndex = transLookup[texUnit.transparency],
                    ColorAnimIndex = texUnit.colorIndex,
                    IndexCount = mesh.nTriangles,
                    RenderFlags = renderFlags[texUnit.renderFlags],
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
            mAnimLookup = ReadArrayOf<short>(reader, mHeader.OfsAnimLookup, mHeader.NAnimLookup);
            mAnimations = ReadArrayOf<AnimationEntry>(reader, mHeader.OfsAnimations, mHeader.NAnimations);
        }

        private void SortPasses()
        {
            mPasses.Sort((e1, e2) =>
            {
                if (e1.BlendMode == 0 && e2.BlendMode != 0)
                    return -1;

                if (e1.BlendMode != 0 && e2.BlendMode == 0)
                    return 1;

                if (e1.BlendMode == e2.BlendMode && e1.BlendMode == 0)
                    return e1.TexUnitNumber.CompareTo(e2.TexUnitNumber);

                if (e1.BlendMode == 2 && e2.BlendMode != 2)
                    return 1;

                if (e2.BlendMode == 2 && e1.BlendMode != 2)
                    return -1;

                var is1Additive = e1.BlendMode == 1 || e1.BlendMode == 6 || e1.BlendMode == 3;
                var is2Additive = e2.BlendMode == 1 || e2.BlendMode == 6 || e2.BlendMode == 3;

                if (is1Additive && !is2Additive)
                    return 1;

                if (is2Additive && !is1Additive)
                    return -1;

                return e1.TexUnitNumber.CompareTo(e2.TexUnitNumber);
            });
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
