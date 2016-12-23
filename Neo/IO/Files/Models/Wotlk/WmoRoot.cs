using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK;
using SlimTK;
using Warcraft.Core;

namespace Neo.IO.Files.Models.Wotlk
{
	internal class WmoRoot : Models.WmoRoot
    {
        private Mohd mHeader;
        // ReSharper disable once CollectionNeverQueried.Local
        private Dictionary<int, string> mTextureNames = new Dictionary<int, string>();
        private Dictionary<int, Graphics.Texture> mTextures = new Dictionary<int, Graphics.Texture>();
        private List<WmoMaterial> mMaterials = new List<WmoMaterial>();
        private Dictionary<uint, string> mGroupNameTable = new Dictionary<uint, string>();
        private List<Mogi> mGroupInfos = new List<Mogi>();
        private List<WmoGroup> mGroups = new List<WmoGroup>();

        public uint AmbientColor { get { return this.mHeader.ambientColor; } }
        public bool UseParentAmbient { get { return (this.mHeader.flags & 2) == 0; } }

        ~WmoRoot()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (this.mMaterials != null)
            {
	            this.mMaterials.Clear();
	            this.mMaterials = null;
            }

            if (this.mTextures != null)
            {
	            this.mTextures.Clear();
	            this.mTextures = null;
            }

            if (this.mTextureNames != null)
            {
	            this.mTextureNames.Clear();
	            this.mTextureNames = null;
            }

            if (this.mGroups != null)
            {
	            this.mGroups.Clear();
	            this.mGroups = null;
            }

            if (this.mGroupNameTable != null)
            {
	            this.mGroupNameTable.Clear();
	            this.mGroupNameTable = null;
            }

            if (this.mGroupInfos != null)
            {
	            this.mGroupInfos.Clear();
	            this.mGroupInfos = null;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public string GetGroupNameByOffset(uint offset)
        {
            if (this.mGroupNameTable.ContainsKey(offset))
            {
                return this.mGroupNameTable[offset];
            }

            return "";
        }

        public override Graphics.Texture GetTexture(int index)
        {
            return this.mTextures[index];
        }

        public override WmoMaterial GetMaterial(int index)
        {
            if (index >= this.mMaterials.Count)
            {
	            throw new IndexOutOfRangeException();
            }

	        return this.mMaterials[index];
        }

        public override bool Load(string fileName)
        {
	        this.Groups = new List<Models.WmoGroup>();
	        this.FileName = fileName;

            using (var file = FileManager.Instance.Provider.OpenFile(fileName))
            {
                if (file == null)
                {
	                throw new ArgumentException("WMO not found: " + fileName);
                }

	            var reader = new BinaryReader(file);

                try
                {
                    var hasHeader = false;
                    var hasTextures = false;
                    var hasMaterials = false;
                    var hasGroupNames = false;
                    var hasGroupInfos = false;
                    while (hasHeader == false || hasTextures == false || hasMaterials == false || hasGroupNames == false || hasGroupInfos == false)
                    {
                        var signature = reader.ReadUInt32();
                        var size = reader.ReadInt32();
                        var curPos = file.Position;
                        switch (signature)
                        {
                            case 0x4D4F4844:
	                            this.mHeader = reader.Read<Mohd>();
                                hasHeader = true;
                                break;

                            case 0x4D4F5458:
                                ReadTextures(reader, size);
                                hasTextures = true;
                                break;

                            case 0x4D4F4D54:
                                LoadMaterials(reader, size);
                                hasMaterials = true;
                                break;

                            case 0x4D4F474E:
                                LoadGroupNames(reader, size);
                                hasGroupNames = true;
                                break;

                            case 0x4D4F4749:
                                LoadGroupInfos(reader, size);
                                hasGroupInfos = true;
                                break;
                        }

                        file.Position = curPos + size;
                    }
                }
                catch (EndOfStreamException)
                {

                }
                catch (Exception e)
                {
                    Log.Error("Unable to load WMO: " + e.Message);
                    return false;
                }

                return LoadGroups();
            }
        }

        private bool LoadGroups()
        {
            if (this.mHeader.nGroups == 0)
            {
                Log.Warning("WMO has no groups - Skipping");
                return true;
            }

            var rootPath = Path.ChangeExtension(this.FileName, null);

            var minPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (var i = 0; i < this.mHeader.nGroups; ++i)
            {
                var groupName = string.Format("{0}_{1:D3}.wmo", rootPath, i);
                var group = new WmoGroup(groupName, this);

                if (group.Load())
                {
	                this.mGroups.Add(group);
                    var gmin = group.MinPosition;
                    var gmax = group.MaxPosition;

                    if (gmin.X < minPos.X)
                    {
	                    minPos.X = gmin.X;
                    }
	                if (gmin.Y < minPos.Y)
	                {
		                minPos.Y = gmin.Y;
	                }
	                if (gmin.Z < minPos.Z)
	                {
		                minPos.Z = gmin.Z;
	                }
	                if (gmax.X > maxPos.X)
	                {
		                maxPos.X = gmax.X;
	                }
	                if (gmax.Y > maxPos.Y)
	                {
		                maxPos.Y = gmax.Y;
	                }
	                if (gmax.Z > maxPos.Z)
	                {
		                maxPos.Z = gmax.Z;
	                }
                }
                else
                {
	                return false;
                }
            }


	        this.Groups = this.mGroups.Select(g => (Models.WmoGroup)g).ToList().AsReadOnly();

	        this.BoundingBox = new BoundingBox(minPos, maxPos);
            return true;
        }

        private void LoadMaterials(BinaryReader reader, int size)
        {
            var numMaterials = size / SizeCache<Momt>.Size;
            var materials = reader.ReadArray<Momt>(numMaterials);
	        this.mMaterials = materials.Select(m => new WmoMaterial(this, m.shader, m.texture1, m.texture2, m.texture3, m.blendMode, m.flags1, m.flags)).ToList();
        }

        private void LoadGroupNames(BinaryReader reader, int size)
        {
            var strBytes = reader.ReadBytes(size);
            uint curOffset = 0;
            var curBytes = new List<byte>();
            for (uint i = 0; i < strBytes.Length; ++i)
            {
                if (strBytes[i] != 0)
                {
                    curBytes.Add(strBytes[i]);
                    continue;
                }

	            this.mGroupNameTable.Add(curOffset, Encoding.UTF8.GetString(curBytes.ToArray()));
                curBytes.Clear();
                curOffset = i + 1;
            }
        }

        private void LoadGroupInfos(BinaryReader reader, int size)
        {
            var numGroups = size / SizeCache<Mogi>.Size;
            var groupInfos = reader.ReadArray<Mogi>(numGroups);
	        this.mGroupInfos = groupInfos.ToList();
        }

        private void ReadTextures(BinaryReader reader, int size)
        {
            var offset = 0;
            var curBytes = new List<byte>();

            var bytes = reader.ReadBytes(size);
            for (var i = 0; i < size; ++i)
            {
                var b = bytes[i];
                if (b == 0)
                {
                    var texName = Encoding.ASCII.GetString(curBytes.ToArray());
	                this.mTextureNames.Add(offset, texName);
	                this.mTextures.Add(offset, Scene.Texture.TextureManager.Instance.GetTexture(texName));
                    offset = i + 1;
                    curBytes.Clear();
                }
                else
                {
	                curBytes.Add(b);
                }
            }
        }
    }
}
