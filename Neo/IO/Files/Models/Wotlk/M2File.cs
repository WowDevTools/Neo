using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Neo.Scene.Models.M2;
using Neo.Storage;
using OpenTK;
using SlimTK;

namespace Neo.IO.Files.Models.Wotlk
{
    public sealed class M2File : Models.M2File
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

        public override string ModelName { get { return this.mModelName; } }

        public M2File(string fileName) : base(fileName)
        {
	        this.Bones = new M2AnimationBone[0];
	        this.UvAnimations = new M2UVAnimation[0];
	        this.ColorAnimations = new M2TexColorAnimation[0];
	        this.Transparencies = new M2AlphaAnimation[0];
	        this.GlobalSequences = new uint[0];
	        this.Animations = new AnimationEntry[0];
	        this.AnimationLookup = new short[0];
	        this.mModelName = string.Empty;
	        this.mFileName = fileName;
	        this.mDirectoryParts = Path.GetDirectoryName(fileName).Split(Path.DirectorySeparatorChar);
        }

        public override bool Load()
        {
            using (var strm = FileManager.Instance.Provider.OpenFile(this.mFileName))
            {
                var reader = new BinaryReader(strm);
	            this.mHeader = reader.Read<M2Header>();

	            this.BoundingRadius = this.mHeader.VertexRadius;

                if ((this.mHeader.GlobalFlags & 0x08) != 0)
                {
	                this.mRemapBlend = true;
                    var nBlendMaps = reader.Read<int>();
                    var ofsBlendMaps = reader.Read<int>();
                    strm.Position = ofsBlendMaps;
	                this.mBlendMap = reader.ReadArray<ushort>(nBlendMaps);
                }

                strm.Position = this.mHeader.OfsName;
                if (this.mHeader.LenName > 0)
                {
	                this.mModelName = Encoding.ASCII.GetString(reader.ReadBytes(this.mHeader.LenName - 1));
                }

	            this.GlobalSequences = ReadArrayOf<uint>(reader, this.mHeader.OfsGlobalSequences, this.mHeader.NGlobalSequences);
	            this.Vertices = ReadArrayOf<M2Vertex>(reader, this.mHeader.OfsVertices, this.mHeader.NVertices);

                var minPos = new Vector3(float.MaxValue);
                var maxPos = new Vector3(float.MinValue);
                for (var i = 0; i < this.Vertices.Length; ++i)
                {
                    var p = this.Vertices[i].position;
                    p = new Vector3(p.X, -p.Y, p.Z);
	                this.Vertices[i].position = p;
	                this.Vertices[i].normal = new Vector3(this.Vertices[i].normal.X, -this.Vertices[i].normal.Y, this.Vertices[i].normal.Z);
                    if (p.X < minPos.X)
                    {
	                    minPos.X = p.X;
                    }
	                if (p.Y < minPos.Y)
	                {
		                minPos.Y = p.Y;
	                }
	                if (p.Z < minPos.Z)
	                {
		                minPos.Z = p.Z;
	                }
	                if (p.X > maxPos.X)
	                {
		                maxPos.X = p.X;
	                }
	                if (p.Y > maxPos.Y)
	                {
		                maxPos.Y = p.Y;
	                }
	                if (p.Z > maxPos.Z)
	                {
		                maxPos.Z = p.Z;
	                }
                }

	            this.BoundingBox = new BoundingBox(minPos, maxPos);

                LoadCreatureVariations();

                var textures = ReadArrayOf<M2Texture>(reader, this.mHeader.OfsTextures, this.mHeader.NTextures);
	            this.mTextures = new Graphics.Texture[textures.Length];
	            this.TextureInfos = new TextureInfo[textures.Length];
                for (var i = 0; i < textures.Length; ++i)
                {
                    var tex = textures[i];
                    if (tex.type == 0 && tex.lenName > 0)
                    {
                        var texName = Encoding.ASCII.GetString(ReadArrayOf<byte>(reader, tex.ofsName, tex.lenName - 1)).Trim();
	                    this.mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture(texName);
                    }
                    else
                    {
                        switch ((TextureType)tex.type)
                        {
                            //case TextureType.Skin:
                            //    string skinTexName = $"{mDirectoryParts[1]}{mDirectoryParts[2]}NakedTorsoSkin00_{DisplayOptions.SkinId.ToString("00")}.blp";
                            //    skinTexName = Path.Combine(Path.GetDirectoryName(mFileName), skinTexName);
                            //    mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture(skinTexName);
                            //    break;

                            ////case TextureType.ObjectSkin:
                            ////    break;

                            //case TextureType.CharacterHair:
                            //    string hairTexName = $"{mDirectoryParts[1]}NakedTorsoSkin00_{DisplayOptions.SkinId.ToString("00")}.blp";
                            //    skinTexName = Path.Combine(Path.GetDirectoryName(mFileName), hairTexName);
                            //    mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture(skinTexName);
                            //    break;

                            case TextureType.MonsterSkin1:
                                if (this.DisplayOptions.TextureVariationFiles.Count > this.DisplayOptions.TextureVariation)
                                {
	                                if (!string.IsNullOrEmpty(this.DisplayOptions.TextureVariationFiles[this.DisplayOptions.TextureVariation].Item1))
	                                {
		                                this.mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture(this.DisplayOptions.TextureVariationFiles[this.DisplayOptions.TextureVariation].Item1);
	                                }
                                }
	                            break;
                            case TextureType.MonsterSkin2:
                                if (this.DisplayOptions.TextureVariationFiles.Count > this.DisplayOptions.TextureVariation)
                                {
	                                if (!string.IsNullOrEmpty(this.DisplayOptions.TextureVariationFiles[this.DisplayOptions.TextureVariation].Item2))
	                                {
		                                this.mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture(this.DisplayOptions.TextureVariationFiles[this.DisplayOptions.TextureVariation].Item2);
	                                }
                                }
	                            break;
                            case TextureType.MonsterSkin3:
                                if (this.DisplayOptions.TextureVariationFiles.Count > this.DisplayOptions.TextureVariation)
                                {
	                                if (!string.IsNullOrEmpty(this.DisplayOptions.TextureVariationFiles[this.DisplayOptions.TextureVariation].Item3))
	                                {
		                                this.mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture(this.DisplayOptions.TextureVariationFiles[this.DisplayOptions.TextureVariation].Item3);
	                                }
                                }
	                            break;
                            default:
	                            this.mTextures[i] = Scene.Texture.TextureManager.Instance.GetTexture("default_texture");
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

	                this.TextureInfos[i] = new TextureInfo
                    {
                        Texture = this.mTextures[i],
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
	            return Path.Combine(Path.GetDirectoryName(this.mFileName), f); //Add full directory location
            };

	        this.DisplayOptions.TextureVariationFiles = new List<Tuple<string, string, string>>();
            HashSet<Tuple<string, string, string>> variations = new HashSet<Tuple<string, string, string>>(); //Unique combinations only

            //First check creatures/characters
            if (this.mDirectoryParts.Length > 0 && this.mDirectoryParts[0].ToLower() != "character" && this.mDirectoryParts[0].ToLower() != "creature")
            {
	            return;
            }

	        //Second check MDX exists
            string mdx = Path.ChangeExtension(this.mFileName, ".mdx").ToLower();
	        if (DbcStorage.CreatureModelData.GetAllRows<CreatureModelDataEntry>().All(x => x.ModelPath.ToLower() != mdx))
	        {
		        return;
	        }

	        var modelDisplay = from cmd in DbcStorage.CreatureModelData.GetAllRows<CreatureModelDataEntry>()
                               join cdi in DbcStorage.CreatureDisplayInfo.GetAllRows<CreatureDisplayInfoEntry>()
                               on cmd.ID equals cdi.ModelId
                               where cmd.ModelPath.ToLower() == mdx
                               select cdi;

            if (!modelDisplay.Any())
            {
                variations.Add(new Tuple<string, string, string>("default_texture", "", ""));
            }
            else
            {
                foreach (var display in modelDisplay)
                {
                    variations.Add(new Tuple<string, string, string>(FormatPath(display.TextureVariation1),
                                                                     FormatPath(display.TextureVariation2),
                                                                     FormatPath(display.TextureVariation3)));
                }

                //Get Extra display info
                var extraDisplay = from md in modelDisplay
                                   join cdi in DbcStorage.CreatureDisplayInfoExtra.GetAllRows<CreatureDisplayInfoExtraEntry>()
                                   on md.ExtendedDisplayInfoId equals cdi.ID
                                   select cdi;

                if(extraDisplay.Any())
                {
	                this.DisplayOptions.FaceOptions = extraDisplay.Select(x => x.FaceID).Distinct().ToArray();
	                this.DisplayOptions.FacialHairOptions = extraDisplay.Select(x => x.FacialHairID).Distinct().ToArray();
	                this.DisplayOptions.SkinOptions = extraDisplay.Select(x => x.SkinID).Distinct().ToArray();
	                this.DisplayOptions.HairColorOptions = extraDisplay.Select(x => x.HairColorID).Distinct().ToArray();
	                this.DisplayOptions.HairStyleOptions = extraDisplay.Select(x => x.HairStyleID).Distinct().ToArray();
                }
            }

	        this.DisplayOptions.TextureVariationFiles.AddRange(variations);
        }

        private void LoadSkins(BinaryReader reader)
        {
	        this.mSkin = new M2SkinFile(this.ModelRoot, this.mModelName, 0);
            if (this.mSkin.Load() == false)
            {
	            throw new InvalidOperationException("Unable to load skin file");
            }

	        this.Indices = this.mSkin.Indices;

            var texLookup = ReadArrayOf<ushort>(reader, this.mHeader.OfsTexLookup, this.mHeader.NTexLookup);
            var renderFlags = ReadArrayOf<uint>(reader, this.mHeader.OfsRenderFlags, this.mHeader.NRenderFlags);
            var uvAnimLookup = ReadArrayOf<short>(reader, this.mHeader.OfsUvAnimLookup, this.mHeader.NUvAnimLookup);

	        this.mSubMeshes = this.mSkin.SubMeshes.Select(sm => new M2SubMeshInfo
            {
                BoundingSphere =
                    new BoundingSphere(new Vector3(sm.centerBoundingBox.X, -sm.centerBoundingBox.Y, sm.centerBoundingBox.Z), sm.radius),
                NumIndices = sm.nTriangles,
                StartIndex = sm.startTriangle + (((sm.unk1 & 1) != 0) ? (ushort.MaxValue + 1) : 0)
            }).ToArray();

            foreach (var texUnit in this.mSkin.TexUnits)
            {
                var mesh = this.mSkin.SubMeshes[texUnit.submeshIndex];

                int uvIndex;
                if (texUnit.textureAnimIndex >= uvAnimLookup.Length || uvAnimLookup[texUnit.textureAnimIndex] < 0)
                {
	                uvIndex = -1;
                }
                else
                {
	                uvIndex = uvAnimLookup[texUnit.textureAnimIndex];
                }

	            var startTriangle = (int)mesh.startTriangle;
                if ((mesh.unk1 & 1) != 0)
                {
	                startTriangle += ushort.MaxValue + 1;
                }

	            var textures = new List<Graphics.Texture>();
                var texIndices = new List<int>();
                switch (texUnit.op_count)
                {
                    case 2:
                        textures.Add(this.mTextures[texLookup[texUnit.texture]]);
                        textures.Add(this.mTextures[texLookup[texUnit.texture + 1]]);
                        texIndices.Add(texLookup[texUnit.texture]);
                        texIndices.Add(texLookup[texUnit.texture + 1]);
                        break;
                    case 3:
                        textures.Add(this.mTextures[texLookup[texUnit.texture]]);
                        textures.Add(this.mTextures[texLookup[texUnit.texture + 1]]);
                        textures.Add(this.mTextures[texLookup[texUnit.texture + 2]]);
                        texIndices.Add(texLookup[texUnit.texture]);
                        texIndices.Add(texLookup[texUnit.texture + 1]);
                        texIndices.Add(texLookup[texUnit.texture + 2]);
                        break;
                    case 4:
                        textures.Add(this.mTextures[texLookup[texUnit.texture]]);
                        textures.Add(this.mTextures[texLookup[texUnit.texture + 1]]);
                        textures.Add(this.mTextures[texLookup[texUnit.texture + 2]]);
                        textures.Add(this.mTextures[texLookup[texUnit.texture + 3]]);
                        texIndices.Add(texLookup[texUnit.texture]);
                        texIndices.Add(texLookup[texUnit.texture + 1]);
                        texIndices.Add(texLookup[texUnit.texture + 2]);
                        texIndices.Add(texLookup[texUnit.texture + 3]);
                        break;
                    default:
                        textures.Add(this.mTextures[texLookup[texUnit.texture]]);
                        texIndices.Add(texLookup[texUnit.texture]);
                        break;
                }

                var flags = renderFlags[texUnit.renderFlags];
                var blendMode = flags >> 16;
                var flag = flags & 0xFFFF;

                if (this.mRemapBlend && texUnit.shaderId < this.mBlendMap.Length)
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

	            this.Passes.Add(new M2RenderPass
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
                    VertexShaderType = M2ShadersClass.GetVertexShaderTypeOld(texUnit.shaderId, texUnit.op_count),
                    PixelShaderType = M2ShadersClass.GetPixelShaderTypeOld(texUnit.shaderId, texUnit.op_count),
                });
            }

            SortPasses();
        }

        private void LoadAnimations(BinaryReader reader)
        {
            var bones = ReadArrayOf<M2Bone>(reader, this.mHeader.OfsBones, this.mHeader.NBones);
	        this.Bones = bones.Select(b => new M2AnimationBone(this, ref b, reader)).ToArray();

            if (this.Bones.Any(b => b.IsBillboarded))
            {
	            this.NeedsPerInstanceAnimation = true;
            }

	        this.AnimationLookup = ReadArrayOf<short>(reader, this.mHeader.OfsAnimLookup, this.mHeader.NAnimLookup);
	        this.Animations = ReadArrayOf<AnimationEntry>(reader, this.mHeader.OfsAnimations, this.mHeader.NAnimations);

	        this.AnimationIds = this.Animations.Select(x => x.animationID).ToArray();

            var uvAnims = ReadArrayOf<M2TexAnim>(reader, this.mHeader.OfsUvAnimation, this.mHeader.NUvAnimation);
	        this.UvAnimations = uvAnims.Select(uv => new M2UVAnimation(this, ref uv, reader)).ToArray();

            var colorAnims = ReadArrayOf<M2ColorAnim>(reader, this.mHeader.OfsSubmeshAnimations, this.mHeader.NSubmeshAnimations);
	        this.ColorAnimations = colorAnims.Select(c => new M2TexColorAnimation(this, ref c, reader)).ToArray();

            var transparencies = ReadArrayOf<AnimationBlock>(reader, this.mHeader.OfsTransparencies, this.mHeader.NTransparencies);
	        this.Transparencies = transparencies.Select(t => new M2AlphaAnimation(this, ref t, reader)).ToArray();
        }

        private void SortPasses()
        {
	        this.Passes.Sort((e1, e2) =>
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
            return this.Bones.Length;
        }

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
