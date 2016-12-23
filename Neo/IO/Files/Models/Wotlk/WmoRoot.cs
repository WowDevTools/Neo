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

        public uint AmbientColor { get { return mHeader.ambientColor; } }
        public bool UseParentAmbient { get { return (mHeader.flags & 2) == 0; } }

        ~WmoRoot()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (mMaterials != null)
            {
                mMaterials.Clear();
                mMaterials = null;
            }

            if (mTextures != null)
            {
                mTextures.Clear();
                mTextures = null;
            }

            if (mTextureNames != null)
            {
                mTextureNames.Clear();
                mTextureNames = null;
            }

            if (mGroups != null)
            {
                mGroups.Clear();
                mGroups = null;
            }

            if (mGroupNameTable != null)
            {
                mGroupNameTable.Clear();
                mGroupNameTable = null;
            }

            if (mGroupInfos != null)
            {
                mGroupInfos.Clear();
                mGroupInfos = null;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public string GetGroupNameByOffset(uint offset)
        {
            if (mGroupNameTable.ContainsKey(offset))
            {
                return mGroupNameTable[offset];
            }

            return "";
        }

        public override Graphics.Texture GetTexture(int index)
        {
            return mTextures[index];
        }

        public override WmoMaterial GetMaterial(int index)
        {
            if (index >= mMaterials.Count)
            {
	            throw new IndexOutOfRangeException();
            }

	        return mMaterials[index];
        }

        public override bool Load(string fileName)
        {
            Groups = new List<Models.WmoGroup>();
            FileName = fileName;

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
                                mHeader = reader.Read<Mohd>();
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
            if (mHeader.nGroups == 0)
            {
                Log.Warning("WMO has no groups - Skipping");
                return true;
            }

            var rootPath = Path.ChangeExtension(FileName, null);

            var minPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (var i = 0; i < mHeader.nGroups; ++i)
            {
                var groupName = string.Format("{0}_{1:D3}.wmo", rootPath, i);
                var group = new WmoGroup(groupName, this);

                if (group.Load())
                {
                    mGroups.Add(group);
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


            Groups = mGroups.Select(g => (Models.WmoGroup)g).ToList().AsReadOnly();

            BoundingBox = new BoundingBox(minPos, maxPos);
            return true;
        }

        private void LoadMaterials(BinaryReader reader, int size)
        {
            var numMaterials = size / SizeCache<Momt>.Size;
            var materials = reader.ReadArray<Momt>(numMaterials);
            mMaterials = materials.Select(m => new WmoMaterial(this, m.shader, m.texture1, m.texture2, m.texture3, m.blendMode, m.flags1, m.flags)).ToList();
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

                mGroupNameTable.Add(curOffset, Encoding.UTF8.GetString(curBytes.ToArray()));
                curBytes.Clear();
                curOffset = i + 1;
            }
        }

        private void LoadGroupInfos(BinaryReader reader, int size)
        {
            var numGroups = size / SizeCache<Mogi>.Size;
            var groupInfos = reader.ReadArray<Mogi>(numGroups);
            mGroupInfos = groupInfos.ToList();
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
                    mTextureNames.Add(offset, texName);
                    mTextures.Add(offset, Scene.Texture.TextureManager.Instance.GetTexture(texName));
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
