using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.IO.Files.Models.WoD
{
    class WmoGroup
    {
        private WmoVertex[] mVertices;
        private List<ushort> mIndices = new List<ushort>();
        private List<WmoBatch> mBatches = new List<WmoBatch>();

        private WeakReference<WmoRoot> mParent;
        private readonly string mFileName;
        private Mogp mHeader;

        private BoundingBox mBoundingBox;

        public Vector3 MinPosition => mBoundingBox.Minimum;
        public Vector3 MaxPosition => mBoundingBox.Maximum;

        public WmoGroup(string fileName, WmoRoot root)
        {
            mFileName = fileName;
            mParent = new WeakReference<WmoRoot>(root);
        }

        public bool Load()
        {
            using (var file = FileManager.Instance.Provider.OpenFile(mFileName))
            {
                if(file == null)
                {
                    Log.Error("Unable to load WMO group. File not found: " + mFileName);
                    return false;
                }

                var reader = new BinaryReader(file);

                while(true)
                {
                    var signature = reader.ReadUInt32();
                    var size = reader.ReadInt32();

                    var oldPos = file.Position;
                    switch(signature)
                    {
                        case 0x4D4F4750:
                            mHeader = reader.Read<Mogp>();
                            if (!LoadGroupChunks(reader, size - SizeCache<Mogp>.Size))
                                return false;

                            break;
                    }

                    file.Position = oldPos + size;
                }
            }
        }

        private bool LoadGroupChunks(BinaryReader reader, int size)
        {
            var endPos = reader.BaseStream.Position + size;
            while(reader.BaseStream.Position + 8 < endPos)
            {
                var signature = reader.ReadUInt32();
                var chunkSize = reader.ReadInt32();
                if(reader.BaseStream.Position + chunkSize > endPos)
                {
                    Log.Error("Invalid format in WMO group. Sub chunks of MOGP are outside the size of the MOGP chunk");
                    return false;
                }

                var curPos = reader.BaseStream.Position;

                switch(signature)
                {

                }

                reader.BaseStream.Position = curPos + chunkSize;
            }

            return true;
        }

        private bool LoadTexCoords(BinaryReader reader, int size)
        {
            // TODO Implement
            return true;
        }

        private bool LoadNormals(BinaryReader reader, int size)
        {
            var numNormals = size / SizeCache<Vector3>.Size;
            var normals = reader.ReadArray<Vector3>(numNormals);

            if (mVertices == null)
                mVertices = new WmoVertex[numNormals];
            else if(mVertices.Length != numNormals)
            {
                Log.Error("Invalid format in WMO group. Inconsistent sizes in positions, texture coordinates and normals");
                return false;
            }

            for (var i = 0; i < numNormals; ++i)
                mVertices[i].Normal = normals[i];

            return true;
        }

        private bool LoadVertices(BinaryReader reader, int size)
        {
            var numVertices = size / SizeCache<Vector3>.Size;
            var vertices = reader.ReadArray<Vector3>(numVertices);

            if(mVertices == null)
                mVertices = new WmoVertex[numVertices];
            else if(mVertices.Length != numVertices)
            {
                Log.Error("Invalid format in WMO group. Inconsistent sizes in positions, texture coordinates and normals");
                return false;
            }

            for (var i = 0; i < numVertices; ++i)
                mVertices[i].Position = vertices[i];

            return true;
        }
    }
}
