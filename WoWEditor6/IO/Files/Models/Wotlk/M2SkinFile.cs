using System.IO;
using System.Linq;

namespace WoWEditor6.IO.Files.Models.Wotlk
{
    class M2SkinFile
    {
        private readonly string mFileName;
        private M2Skin mSkin;
        public ushort[] Indices { get; private set; } = new ushort[0];

        public M2TexUnit[] TexUnits { get; private set; } = new M2TexUnit[0];
        public M2SubMesh[] SubMeshes { get; private set; } = new M2SubMesh[0];

        public M2SkinFile(string rootPath, string modelName, int index)
        {
            mFileName = string.Format("{0}\\{1}{2:D2}.skin", rootPath, modelName, index);
        }

        public bool Load()
        {
            using (var strm = FileManager.Instance.Provider.OpenFile(mFileName))
            {
                if (strm == null)
                    return false;

                var reader = new BinaryReader(strm);
                mSkin = reader.Read<M2Skin>();
                var indexLookup = ReadArrayOf<ushort>(reader, mSkin.ofsIndices, mSkin.nIndices);
                var triangles = ReadArrayOf<ushort>(reader, mSkin.ofsTriangles, mSkin.nTriangles);

                Indices = triangles.Select(t => indexLookup[t]).ToArray();
                SubMeshes = ReadArrayOf<M2SubMesh>(reader, mSkin.ofsSubmeshes, mSkin.nSubmeshes);
                TexUnits = ReadArrayOf<M2TexUnit>(reader, mSkin.ofsTexUnits, mSkin.nTexUnits);
                return true;
            }
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
