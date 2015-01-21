using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.IO.Files.Models.WoD
{
    class WmoMaterial
    {
        public Momt Material { get; }

        public List<Graphics.Texture> Textures { get; } = new List<Graphics.Texture>();
        public int ShaderType => Material.shader;

        public WmoMaterial(WmoRoot root, Momt material)
        {
            Material = material;
            LoadTextures(root);
        }

        private void LoadTextures(WmoRoot root)
        {
            switch(Material.shader)
            {
                case 11:
                case 12:
                case 7:
                    Textures.Add(root.GetTexture(Material.texture1));
                    Textures.Add(root.GetTexture(Material.texture2));
                    Textures.Add(root.GetTexture(Material.texture3));
                    break;

                case 5:
                case 6:
                case 8:
                case 9:
                case 13:
                case 15:
                case 3:
                    Textures.Add(root.GetTexture(Material.texture1));
                    Textures.Add(root.GetTexture(Material.texture2));
                    break;

                default:
                    Textures.Add(root.GetTexture(Material.texture1));
                    break;
            }
        }
    }

    class WmoRoot
    {
        private Mohd mHeader;
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly Dictionary<int, string> mTextureNames = new Dictionary<int, string>();
        private readonly Dictionary<int, Graphics.Texture> mTextures = new Dictionary<int, Graphics.Texture>();
        private List<WmoMaterial> mMaterials = new List<WmoMaterial>();
        private string mFileName;
        private readonly List<WmoGroup> mGroups = new List<WmoGroup>();
        private BoundingBox mBoundingBox;

        public Graphics.Texture GetTexture(int index)
        {
            if (index >= mTextures.Count)
                throw new IndexOutOfRangeException();

            return mTextures[index];
        }

        public WmoMaterial GetMaterial(int index)
        {
            if (index >= mMaterials.Count)
                throw new IndexOutOfRangeException();

            return mMaterials[index];
        }

        public void Load(string fileName)
        {
            mFileName = fileName;

            using (var file = FileManager.Instance.Provider.OpenFile(fileName))
            {
                if (file == null)
                    throw new ArgumentException("WMO not found: " + fileName);

                var reader = new BinaryReader(file);

                while(true)
                {
                    var signature = reader.ReadUInt32();
                    var size = reader.ReadInt32();
                    var curPos = file.Position;
                    switch(signature)
                    {
                        case 0x4D4F4844:
                            mHeader = reader.Read<Mohd>();
                            break;

                        case 0x4D4F5458:
                            ReadTextures(reader, size);
                            break;

                        case 0x4D4F4D54:
                            LoadMaterials(reader, size);
                            break;
                    }

                    file.Position = curPos + size;
                }
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
            }

            mBoundingBox = new BoundingBox(minPos, maxPos);
            return true;
        }

        private void LoadMaterials(BinaryReader reader, int size)
        {
            var numMaterials = size / SizeCache<Momt>.Size;
            var materials = reader.ReadArray<Momt>(numMaterials);
            mMaterials = materials.Select(m => new WmoMaterial(this, m)).ToList();
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
