using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpDX;

namespace WoWEditor6.IO.Files.Models.WoD
{
    class WmoGroup
    {
        private WmoVertex[] mVertices;
        private List<ushort> mIndices = new List<ushort>();
        private readonly List<WmoBatch> mBatches = new List<WmoBatch>();

        private readonly WeakReference<WmoRoot> mParent;
        private readonly string mFileName;
        private Mogp mHeader;
        private bool mTexCoordsLoaded;
        // Colors are saved in the instance since its possible that
        // there are less colors than vertices. Since the color chunk
        // however could appear before the full size MOVT/MONR/MOTV it
        // wouldn't know how many vertices to allocate.
        private uint[] mColors = new uint[0];

        private BoundingBox mBoundingBox;

        public Vector3 MinPosition => mBoundingBox.Minimum;
        public Vector3 MaxPosition => mBoundingBox.Maximum;

        public IList<ushort> Indices => mIndices.AsReadOnly();
        public IList<WmoBatch> Batches => mBatches.AsReadOnly();

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
                    case 0x4D4F5654:
                        if (!LoadVertices(reader, chunkSize))
                            return false;
                        break;

                    case 0x4D4F5456:
                        if (!LoadTexCoords(reader, chunkSize))
                            return false;
                        break;

                    case 0x4D4F4E52:
                        if (!LoadNormals(reader, chunkSize))
                            return false;
                        break;

                    case 0x4D4F4356:
                        if (!LoadColors(reader, chunkSize))
                            return false;
                        break;

                    case 0x4D4F5649:
                        LoadIndices(reader, chunkSize);
                        break;

                    case 0x4D4F4241:
                        if (!LoadBatches(reader, chunkSize))
                            return false;
                        break;
                }

                reader.BaseStream.Position = curPos + chunkSize;
            }

            if((mHeader.flags & 4) != 0)
            {
                for (var i = 0; i < mColors.Length && i < mVertices.Length; ++i)
                    mVertices[i].Color = mColors[i];

                for (var i = mColors.Length; i < mVertices.Length; ++i)
                    mVertices[i].Color = ((mHeader.flags & 0x2000) != 0) ? 0x7F7F7F7Fu : 0x00000000u;
            }
            else
            {
                var color = ((mHeader.flags & 0x2000) != 0) ? 0x7F7F7F7Fu : 0u;
                for (var i = 0; i < mVertices.Length; ++i)
                    mVertices[i].Color = color;
            }

            mColors = null;

            return true;
        }

        private void LoadIndices(BinaryReader reader, int size)
        {
            var numIndices = size / 2;
            mIndices = reader.ReadArray<ushort>(numIndices).ToList();
        }

        private bool LoadColors(BinaryReader reader, int size)
        {
            if ((mHeader.flags & 4) == 0)
                return true;

            var numColors = size / 4;
            mColors = reader.ReadArray<uint>(numColors);
            return true;
        }

        private bool LoadTexCoords(BinaryReader reader, int size)
        {
            if (mTexCoordsLoaded)
                return true;

            mTexCoordsLoaded = true;

            var numTexCoords = size / SizeCache<Vector2>.Size;
            var texCoords = reader.ReadArray<Vector2>(numTexCoords);
            if (mVertices == null)
                mVertices = new WmoVertex[numTexCoords];
            else if(mVertices.Length != numTexCoords)
            {
                Log.Error("Invalid format in WMO group. Inconsistent sizes in positions, texture coordinates and normals");
                return false;
            }

            for (var i = 0; i < numTexCoords; ++i)
                mVertices[i].TexCoord = texCoords[i];

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

            var minPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (var i = 0; i < numVertices; ++i)
            {
                var v = vertices[i];
                mVertices[i].Position = v;
                if (v.X < minPos.X) minPos.X = v.X;
                if (v.Y < minPos.Y) minPos.Y = v.Y;
                if (v.Z < minPos.Z) minPos.Z = v.Z;
                if (v.X > maxPos.X) maxPos.X = v.X;
                if (v.Y > maxPos.Y) maxPos.Y = v.Y;
                if (v.Z > maxPos.Z) maxPos.Z = v.Z;
            }

            mBoundingBox = new BoundingBox(minPos, maxPos);

            return true;
        }

        private bool LoadBatches(BinaryReader reader, int size)
        {
            var numBatches = size / SizeCache<Moba>.Size;
            if(numBatches != (mHeader.numBatchesA + mHeader.numBatchesB + mHeader.numBatchesC))
            {
                Log.Error("Group has inconsistent amount of batches.");
                return false;
            }

            WmoRoot parent;
            if(!mParent.TryGetTarget(out parent))
            {
                Log.Fatal("FATAL ERROR! Parent of WMO group is null!!");
                return false;
            }

            var batches = reader.ReadArray<Moba>(numBatches);
            for(var i = 0; i < numBatches; ++i)
            {
                var b = batches[i];
                var batch = new WmoBatch
                {
                    NumIndices = b.numFaces,
                    StartIndex = b.firstFace,
                    MaterialId = b.material,
                    BlendMode = parent.GetMaterial(b.material).Material.blendMode,
                    MaterialFlags = parent.GetMaterial(b.material).Material.flags
                };

                mBatches.Add(batch);
            }

            return true;
        }
    }
}
