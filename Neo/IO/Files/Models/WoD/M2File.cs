using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Neo.Scene.Models.M2;
using Neo.Storage;
using SlimTK;

namespace Neo.IO.Files.Models.WoD
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
        private string[] mDirectoryParts;

        public M2AnimationBone[] Bones { get; private set; }
        public M2UVAnimation[] UvAnimations { get; private set; }
        public M2TexColorAnimation[] ColorAnimations { get; private set; }
        public M2AlphaAnimation[] Transparencies { get; private set; }

        public uint[] GlobalSequences { get; private set; }
        public AnimationEntry[] Animations { get; private set; }

        public override string ModelName { get { return mModelName; } }

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
            mDirectoryParts = Path.GetDirectoryName(fileName).Split(Path.DirectorySeparatorChar);
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

                BoundingBox = new BoundingBox(mHeader.VertexBoxMin, mHeader.VertexBoxMax);

                LoadCreatureVariations();

                strm.Position = mHeader.OfsName;
                if (mHeader.LenName > 0)
                {
	                this.mModelName = Encoding.ASCII.GetString(reader.ReadBytes(this.mHeader.LenName - 1));
                }

	            GlobalSequences = ReadArrayOf<uint>(reader, mHeader.OfsGlobalSequences, mHeader.NGlobalSequences);
                Vertices = ReadArrayOf<M2Vertex>(reader, mHeader.OfsVertices, mHeader.NVertices);
                var textures = ReadArrayOf<M2Texture>(reader, mHeader.OfsTextures, mHeader.NTextures);
                mTextures = new Graphics.Texture[textures.Length];
                TextureInfos = new TextureInfo[textures.Length];
                for (var i = 0; i < textures.Length; ++i)
                {
                    var tex = textures[i];
                    if (tex.type == 0 && tex.lenName > 0)
                    {
                        var texName =
                            Encoding.ASCII.GetString(ReadArrayOf<byte>(reader, tex.ofsName, tex.lenName - 1)).Trim();
                        mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture(texName);
                    }
                    else
                    {
                        switch ((TextureType)tex.type)
                        {
                            case TextureType.MonsterSkin1:
                                if (DisplayOptions.TextureVariationFiles.Count > DisplayOptions.TextureVariation)
                                {
	                                if (!string.IsNullOrEmpty(this.DisplayOptions.TextureVariationFiles[this.DisplayOptions.TextureVariation].Item1))
	                                {
		                                this.mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture(this.DisplayOptions.TextureVariationFiles[this.DisplayOptions.TextureVariation].Item1);
	                                }
                                }
	                            break;
                            case TextureType.MonsterSkin2:
                                if (DisplayOptions.TextureVariationFiles.Count > DisplayOptions.TextureVariation)
                                {
	                                if (!string.IsNullOrEmpty(this.DisplayOptions.TextureVariationFiles[this.DisplayOptions.TextureVariation].Item2))
	                                {
		                                this.mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture(this.DisplayOptions.TextureVariationFiles[this.DisplayOptions.TextureVariation].Item2);
	                                }
                                }
	                            break;
                            case TextureType.MonsterSkin3:
                                if (DisplayOptions.TextureVariationFiles.Count > DisplayOptions.TextureVariation)
                                {
	                                if (!string.IsNullOrEmpty(this.DisplayOptions.TextureVariationFiles[this.DisplayOptions.TextureVariation].Item3))
	                                {
		                                this.mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture(this.DisplayOptions.TextureVariationFiles[this.DisplayOptions.TextureVariation].Item3);
	                                }
                                }
	                            break;
                            default:
                                mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture("default_texture");
                                break;
                        }
                    }

                    Graphics.SamplerFlagType samplerFlags;

                    if (tex.flags == 3)
                    {
	                    samplerFlags = Graphics.SamplerFlagType.WrapBoth;
                    }
                    else if (tex.flags == 2)
                    {
	                    samplerFlags = Graphics.SamplerFlagType.WrapV;
                    }
                    else if (tex.flags == 1)
                    {
	                    samplerFlags = Graphics.SamplerFlagType.WrapU;
                    }
                    else
                    {
	                    samplerFlags = Graphics.SamplerFlagType.ClampBoth;
                    }

	                TextureInfos[i] = new TextureInfo
                    {
                        Texture = mTextures[i],
                        TextureType = (TextureType)tex.type,
                        SamplerFlags = samplerFlags
                    };
                }

                LoadSkins(reader);
                LoadAnimations(reader);
            }

            return true;
        }

        private void LoadCreatureVariations()
        {
            Func<string, string> FormatPath = f =>
            {
                if (f == "")
                {
	                return string.Empty; //Ignore blank
                }
	            if (Path.GetExtension(f) != ".blp")
	            {
		            f += ".blp"; //Append filetype
	            }
	            return Path.Combine(Path.GetDirectoryName(mFileName), f); //Add full directory location
            };

            DisplayOptions.TextureVariationFiles = new List<Tuple<string, string, string>>();
            HashSet<Tuple<string, string, string>> variations = new HashSet<Tuple<string, string, string>>();

            //First check creatures/characters
            if (mDirectoryParts.Length > 0 && mDirectoryParts[0].ToLower() != "character" && mDirectoryParts[0].ToLower() != "creature")
            {
	            return;
            }

	        //Second check MDX exists
            string mdx = Path.ChangeExtension(mFileName, ".mdx").ToLower();
	        if (DbcStorage.CreatureModelData.GetAllRows<Wotlk.CreatureModelDataEntry>().All(x => x.ModelPath.ToLower() != mdx))
	        {
		        return;
	        }

	        var modelDisplay = from fd in Storage.DbcStorage.FileData.GetAllRows<WoD.FileDataIDEntry>()
                               join cmd in Storage.DbcStorage.CreatureModelData.GetAllRows<Wotlk.CreatureModelDataEntry>()
                               on Path.Combine(fd.FilePath, fd.FileName).ToLower() equals mFileName.ToLower()
                               join cdi in Storage.DbcStorage.CreatureDisplayInfo.GetAllRows<Wotlk.CreatureDisplayInfoEntry>()
                               on cmd.ID equals cdi.ModelId
                               where cmd.ModelPath.ToLower() == mdx
                               select cdi;

            if(!modelDisplay.Any())
            {
                variations.Add(new Tuple<string, string, string>("default_texture", "", ""));
            }
            else
            {
                foreach (var display in modelDisplay)
                {
                    variations.Add(new Tuple<string, string, string>(
                        FormatPath(display.TextureVariation1),
                        FormatPath(display.TextureVariation2),
                        FormatPath(display.TextureVariation3)));
                }
            }

            DisplayOptions.TextureVariationFiles.AddRange(variations);
        }

        private void LoadSkins(BinaryReader reader)
        {
            mSkin = new M2SkinFile(ModelRoot, mModelName, 0);
            if (mSkin.Load() == false)
            {
	            throw new InvalidOperationException("Unable to load skin file");
            }

	        Indices = mSkin.Indices;

            mSubMeshes = mSkin.SubMeshes.Select(sm => new M2SubMeshInfo
            {
                BoundingSphere =
                    new BoundingSphere(sm.centerBoundingBox, sm.radius),
                NumIndices = sm.nTriangles,
                StartIndex = sm.startTriangle + (((sm.unk1 & 1) != 0) ? (ushort.MaxValue + 1) : 0)
            }).ToArray();

            var texLookup = ReadArrayOf<ushort>(reader, mHeader.OfsTexLookup, mHeader.NTexLookup);
            var renderFlags = ReadArrayOf<uint>(reader, mHeader.OfsRenderFlags, mHeader.NRenderFlags);
            var uvAnimLookup = ReadArrayOf<short>(reader, mHeader.OfsUvAnimLookup, mHeader.NUvAnimLookup);

            foreach(var texUnit in mSkin.TexUnits)
            {
                var mesh = mSkin.SubMeshes[texUnit.submeshIndex];

                int uvIndex;
                if (texUnit.textureAnimIndex >= uvAnimLookup.Length || uvAnimLookup[texUnit.textureAnimIndex] < 0)
                {
	                uvIndex = -1;
                }
                else
                {
	                uvIndex = uvAnimLookup[texUnit.textureAnimIndex];
                }

	            var startTriangle = (int) mesh.startTriangle;
                if ((mesh.unk1 & 1) != 0)
                {
	                startTriangle += ushort.MaxValue + 1;
                }

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
                {
	                blendMode = this.mBlendMap[texUnit.shaderId];
                }

	            blendMode %= 7;

                if (blendMode != 0 && blendMode != 1)
                {
	                this.HasBlendPass = true;
                }
                else
                {
	                this.HasOpaquePass = true;
                }

	            Passes.Add(new M2RenderPass
                {
                    TextureIndices = texIndices,
                    Textures = textures,
                    AlphaAnimIndex = texUnit.transparencyIndex,
                    ColorAnimIndex = texUnit.colorIndex,
                    IndexCount = mesh.nTriangles,
                    RenderFlag = flag,
                    BlendMode = blendMode,
                    StartIndex = startTriangle,
                    TexAnimIndex = uvIndex,
                    TexUnitNumber = texUnit.texUnitNumber,
                    OpCount = texUnit.op_count,
                    VertexShaderType = M2ShadersClass.GetVertexShaderType(texUnit.shaderId, texUnit.op_count),
                    PixelShaderType = M2ShadersClass.GetPixelShaderType(texUnit.shaderId, texUnit.op_count),
                });
            }

            SortPasses();
        }

        private void LoadAnimations(BinaryReader reader)
        {
            var bones = ReadArrayOf<M2Bone>(reader, mHeader.OfsBones, mHeader.NBones);
            Bones = bones.Select(b => new M2AnimationBone(this, ref b, reader)).ToArray();

            if (Bones.Any(b => b.IsBillboarded))
            {
	            this.NeedsPerInstanceAnimation = true;
            }

	        AnimationLookup = ReadArrayOf<short>(reader, mHeader.OfsAnimLookup, mHeader.NAnimLookup);
            Animations = ReadArrayOf<AnimationEntry>(reader, mHeader.OfsAnimations, mHeader.NAnimations);

            AnimationIds = Animations.Select(x => x.animationID).ToArray();

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
                {
	                return -1;
                }

	            if (e1.BlendMode != 0 && e2.BlendMode == 0)
	            {
		            return 1;
	            }

	            if (e1.BlendMode == e2.BlendMode && e1.BlendMode == 0)
	            {
		            return e1.TexUnitNumber.CompareTo(e2.TexUnitNumber);
	            }

	            if (e1.BlendMode == 2 && e2.BlendMode != 2)
	            {
		            return -1;
	            }

	            if (e2.BlendMode == 2 && e1.BlendMode != 2)
	            {
		            return 1;
	            }

	            var is1Additive = e1.BlendMode == 1 || e1.BlendMode == 6 || e1.BlendMode == 3;
                var is2Additive = e2.BlendMode == 1 || e2.BlendMode == 6 || e2.BlendMode == 3;

                if (is1Additive && !is2Additive)
                {
	                return -1;
                }

	            if (is2Additive && !is1Additive)
	            {
		            return 1;
	            }

	            return e1.TexUnitNumber.CompareTo(e2.TexUnitNumber);
            });
        }

        public override int GetNumberOfBones()
        {
            return Bones.Length;
        }

	    [Obsolete("Requires the use of memcpy", true)]
	    private static T[] ReadArrayOf<T>(BinaryReader reader, int offset, int count) where T : struct
        {
            if (count == 0)
            {
	            return new T[0];
            }

	        reader.BaseStream.Position = offset;
            return reader.ReadArray<T>(count);
        }
    }
}
