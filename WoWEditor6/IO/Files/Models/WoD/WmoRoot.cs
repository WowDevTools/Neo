using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpDX;

namespace WoWEditor6.IO.Files.Models.WoD
{
    class WmoRoot : Models.WmoRoot
    {
        private Mohd mHeader;
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly Dictionary<int, string> mTextureNames = new Dictionary<int, string>();
        private readonly Dictionary<int, Graphics.Texture> mTextures = new Dictionary<int, Graphics.Texture>();
        private List<WmoMaterial> mMaterials = new List<WmoMaterial>();
        private string mFileName;
        private readonly List<WmoGroup> mGroups = new List<WmoGroup>();

        public override string FileName => mFileName;

        public uint AmbientColor => mHeader.ambientColor;

        public override void Dispose()
        {
            mMaterials.Clear();
            mTextures.Clear();
            mGroups.Clear();
        }

        public override Graphics.Texture GetTexture(int index)
        {
            return mTextures[index];
        }

        public override WmoMaterial GetMaterial(int index)
        {
            if (index >= mMaterials.Count)
                throw new IndexOutOfRangeException();

            return mMaterials[index];
        }

        public override bool Load(string fileName)
        {
            Groups = new List<Models.WmoGroup>();

            mFileName = fileName;

            using (var file = FileManager.Instance.Provider.OpenFile(fileName))
            {
                if (file == null)
                    throw new ArgumentException("WMO not found: " + fileName);

                var reader = new BinaryReader(file);

                try
                {
                    var hasHeader = false;
                    var hasTextures = false;
                    var hasMaterials = false;
                    while (hasHeader == false || hasTextures == false || hasMaterials == false)
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
                        }

                        file.Position = curPos + size;
                    }
                }
                catch (EndOfStreamException)
                {

                }
                catch(Exception e)
                {
                    Log.Error("Unable to load WMO: " + e.Message);
                    return false;
                }

                return LoadGroups();
            }
        }

        private bool LoadGroups()
        {
            if(mHeader.nGroups == 0)
            {
                Log.Warning("WMO has no groups - Skipping");
                return true;
            }

            var rootPath = Path.ChangeExtension(mFileName, null);

            var minPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for(var i = 0; i < mHeader.nGroups; ++i)
            {
                var groupName = string.Format("{0}_{1:D3}.wmo", rootPath, i);
                var group = new WmoGroup(groupName, this);
                if (group.Load())
                {
                    mGroups.Add(group);
                    var gmin = group.MinPosition;
                    var gmax = group.MaxPosition;

                    if (gmin.X < minPos.X) minPos.X = gmin.X;
                    if (gmin.Y < minPos.Y) minPos.Y = gmin.Y;
                    if (gmin.Z < minPos.Z) minPos.Z = gmin.Z;
                    if (gmax.X > maxPos.X) maxPos.X = gmax.X;
                    if (gmax.Y > maxPos.Y) maxPos.Y = gmax.Y;
                    if (gmax.Z > maxPos.Z) maxPos.Z = gmax.Z;
                }
                else
                    return false;
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
                    curBytes.Add(b);
            }
        }
    }
}
