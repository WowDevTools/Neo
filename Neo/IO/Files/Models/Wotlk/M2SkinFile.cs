using System.IO;
using System.Linq;

namespace Neo.IO.Files.Models.Wotlk
{
	internal class M2SkinFile
    {
        private readonly string mFileName;
        private M2Skin mSkin;
        public ushort[] Indices { get; private set; }

        public M2TexUnit[] TexUnits { get; private set; }
        public M2SubMesh[] SubMeshes { get; private set; }

        public M2SkinFile(string rootPath, string modelName, int index)
        {
	        this.Indices = new ushort[0];
	        this.TexUnits = new M2TexUnit[0];
	        this.SubMeshes = new M2SubMesh[0];
	        this.mFileName = string.Format("{0}\\{1}{2:D2}.skin", rootPath, modelName, index);
        }

        public bool Load()
        {
            using (var strm = FileManager.Instance.Provider.OpenFile(this.mFileName))
            {
                if (strm == null)
                {
	                return false;
                }

	            var reader = new BinaryReader(strm);
	            this.mSkin = reader.Read<M2Skin>();
                var indexLookup = ReadArrayOf<ushort>(reader, this.mSkin.ofsIndices, this.mSkin.nIndices);
                var triangles = ReadArrayOf<ushort>(reader, this.mSkin.ofsTriangles, this.mSkin.nTriangles);

	            this.Indices = triangles.Select(t => indexLookup[t]).ToArray();
	            this.SubMeshes = ReadArrayOf<M2SubMesh>(reader, this.mSkin.ofsSubmeshes, this.mSkin.nSubmeshes);
	            this.TexUnits = ReadArrayOf<M2TexUnit>(reader, this.mSkin.ofsTexUnits, this.mSkin.nTexUnits);
                return true;
            }
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
