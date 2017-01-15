using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK;
using SlimTK;

namespace Neo.IO.Files.Models.WoD
{
    public sealed class WorldModelGroup : IWorldModelGroup
    {
        private WmoVertex[] mVertices;
        private List<ushort> mIndices = new List<ushort>();
        private readonly List<WmoBatch> mBatches = new List<WmoBatch>();

        private readonly WeakReference<WorldModelRoot> mParent;
        private readonly string mFileName;

        private Mogp mHeader;
        private bool mTexCoordsLoaded;
        private uint[] mColors = new uint[0];
        private Vector3[] mPositions = new Vector3[0];
        private Vector3[] mNormals = new Vector3[0];
        private Vector2[] mTexCoords = new Vector2[0];

        public Vector3 MinPosition { get { return this.BoundingBox.Minimum; } }
        public Vector3 MaxPosition { get { return this.BoundingBox.Maximum; } }

	    public BoundingBox BoundingBox { get; private set; }
	    public IList<ushort> Indices { get; private set; }
	    public IList<WmoBatch> Batches { get; private set; }
	    public WmoVertex[] Vertices { get; private set; }
	    public bool IsIndoor { get; private set; }
	    public string Name { get; private set; }
	    public bool DisableRendering { get; private set; }

	    public WorldModelGroup(string fileName, WorldModelRoot root)
        {
	        this.Batches = new List<WmoBatch>();
	        this.Indices = new List<ushort>();
	        this.Vertices = new WmoVertex[0];

	        this.DisableRendering = false;

            this.mFileName = fileName;
            this.mParent = new WeakReference<WorldModelRoot>(root);
        }

        public bool Load()
        {
            using (var file = FileManager.Instance.Provider.OpenFile(this.mFileName))
            {
                if(file == null)
                {
                    Log.Error("Unable to load WMO group. File not found: " + this.mFileName);
                    return false;
                }

                var reader = new BinaryReader(file);
                var readChunks = true;
                try
                {
                    while (readChunks)
                    {
                        var signature = reader.ReadUInt32();
                        var size = reader.ReadInt32();

                        var oldPos = file.Position;
                        switch (signature)
                        {
                            case 0x4D4F4750:
	                            this.mHeader = reader.Read<Mogp>();
	                            this.IsIndoor = (this.mHeader.flags & 0x2000) != 0 && (this.mHeader.flags & 0x8) == 0;
                                if (!LoadGroupChunks(reader, size - SizeCache<Mogp>.Size))
                                {
	                                return false;
                                }
	                            readChunks = false;
                                break;
                        }

                        file.Position = oldPos + size;
                    }
                }
                catch(EndOfStreamException)
                {

                }
                catch(Exception e)
                {
                    Log.Error("Unable to load WMO group: " + e);
                    return false;
                }
            }

            WorldModelRoot root;
            if (this.mParent.TryGetTarget(out root))
            {
	            this.Name = root.GetGroupNameByOffset(this.mHeader.groupName);

                if (this.Name == "antiportal")
                {
	                this.DisableRendering = true;
                }
            }

            return true;
        }

        private bool LoadGroupChunks(BinaryReader reader, int size)
        {
            var endPos = reader.BaseStream.Position + size;
            var hasVertices = false;
            var hasTexCoords = false;
            var hasNormals = false;
            var hasColors = false;
            var hasIndices = false;
            var hasBatches = false;
            while(reader.BaseStream.Position + 8 < endPos && !(hasVertices && hasTexCoords && hasNormals && hasColors && hasIndices && hasBatches))
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
                        LoadVertices(reader, chunkSize);
                        hasVertices = true;
                        break;

                    case 0x4D4F5456:
                        LoadTexCoords(reader, chunkSize);
                        hasTexCoords = true;
                        break;

                    case 0x4D4F4E52:
                        LoadNormals(reader, chunkSize);
                        hasNormals = true;
                        break;

                    case 0x4D4F4356:
                        LoadColors(reader, chunkSize);
                        hasColors = true;
                        break;

                    case 0x4D4F5649:
                        LoadIndices(reader, chunkSize);
                        hasIndices = true;
                        break;

                    case 0x4D4F4241:
                        if (!LoadBatches(reader, chunkSize))
                        {
	                        return false;
                        }
	                    hasBatches = true;
                        break;
                }

                reader.BaseStream.Position = curPos + chunkSize;
            }

            if(this.mTexCoords.Length == 0 && this.mPositions.Length != 0)
            {
	            this.mTexCoords = new Vector2[this.mPositions.Length];
                for (var i = 0; i < this.mTexCoords.Length; ++i)
                {
	                this.mTexCoords[i] = new Vector2(0, 0);
                }
            }

            if(this.mPositions.Length == 0 || this.mPositions.Length != this.mNormals.Length || this.mNormals.Length != this.mTexCoords.Length)
            {
                Log.Error(
                    string.Format(
                        "Invalid format in WMO group. Inconsistent sizes in positions, texture coordinates and normals. {0}/{1}/{2}", this.mPositions.Length, this.mTexCoords.Length, this.mNormals.Length));
                return false;
            }

            if (CombineVertexData() == false)
            {
	            return false;
            }

	        this.mNormals = null;
	        this.mTexCoords = null;
	        this.mColors = null;

            return true;
        }

        private void LoadIndices(BinaryReader reader, int size)
        {
            var numIndices = size / 2;
	        this.mIndices = reader.ReadArray<ushort>(numIndices).ToList();
	        this.Indices = this.mIndices.AsReadOnly();
        }

        private void LoadColors(BinaryReader reader, int size)
        {
            var numColors = size / 4;
	        this.mColors = reader.ReadArray<uint>(numColors);
        }

        private void LoadTexCoords(BinaryReader reader, int size)
        {
            if (this.mTexCoordsLoaded && this.mTexCoords.Length > 0)
            {
	            return;
            }

	        this.mTexCoordsLoaded = true;

            var numTexCoords = size / SizeCache<Vector2>.Size;
	        this.mTexCoords = reader.ReadArray<Vector2>(numTexCoords);
        }

        private void LoadNormals(BinaryReader reader, int size)
        {
            var numNormals = size / SizeCache<Vector3>.Size;
	        this.mNormals = reader.ReadArray<Vector3>(numNormals);
        }

        private void LoadVertices(BinaryReader reader, int size)
        {
            var numVertices = size / SizeCache<Vector3>.Size;
	        this.mPositions = reader.ReadArray<Vector3>(numVertices);
        }

        private bool CombineVertexData()
        {
            WorldModelRoot parent;
            if (!this.mParent.TryGetTarget(out parent))
            {
                Log.Fatal("FATAL ERROR! Parent of WMO group is null!!");
                return false;
            }

	        this.mVertices = new WmoVertex[this.mPositions.Length];
            if (this.mColors == null)
            {
	            this.mColors = new uint[this.mPositions.Length];
                for (var i = 0; i < this.mPositions.Length; ++i)
                {
	                this.mColors[i] = this.IsIndoor ? 0xFF7F7F7Fu : 0x00000000u;
                }
            }

            if (this.mColors.Length < this.mVertices.Length)
            {
                var colors = this.mColors;
	            this.mColors = new uint[this.mVertices.Length];
                if (colors.Length > 0)
                {
	                Buffer.BlockCopy(colors, 0, this.mColors, 0, colors.Length * 4);
                }

	            for (var i = colors.Length; i < this.mColors.Length; ++i)
	            {
		            this.mColors[i] = this.IsIndoor ? 0xFF7F7F7Fu : 0x00000000u;
	            }
            }

            var parentAmbient = parent.AmbientColor;
            var ar = parentAmbient & 0xFF;
            var ag = (parentAmbient >> 8) & 0xFF;
            var ab = (parentAmbient >> 16) & 0xFF;
            var aa = (parentAmbient >> 24) & 0xFF;

            var minPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (var i = 0; i < this.mVertices.Length; ++i)
            {
                var clr = this.mColors[i];
                if (parent.UseParentAmbient && this.IsIndoor)
                {
                    var r = Math.Min((clr & 0xFF) + ar, 255);
                    var g = Math.Min(((clr >> 8) & 0xFF) + ag, 255);
                    var b = Math.Min(((clr >> 16) & 0xFF) + ab, 255);
                    var a = Math.Min(((clr >> 24) & 0xFF) + aa, 255);
                    clr = r | (g << 8) | (b << 16) | (a << 24);
                }
                var v = this.mPositions[i];

	            this.mVertices[i] = new WmoVertex
                {
                    Position = v,
                    Normal = this.mNormals[i],
                    TexCoord = this.mTexCoords[i],
                    Color = clr
                };

                if (v.X < minPos.X)
                {
	                minPos.X = v.X;
                }
	            if (v.Y < minPos.Y)
	            {
		            minPos.Y = v.Y;
	            }
	            if (v.Z < minPos.Z)
	            {
		            minPos.Z = v.Z;
	            }
	            if (v.X > maxPos.X)
	            {
		            maxPos.X = v.X;
	            }
	            if (v.Y > maxPos.Y)
	            {
		            maxPos.Y = v.Y;
	            }
	            if (v.Z > maxPos.Z)
	            {
		            maxPos.Z = v.Z;
	            }
            }

	        this.BoundingBox = new BoundingBox(minPos, maxPos);
	        this.Vertices = this.mVertices;

            return true;
        }

        private bool LoadBatches(BinaryReader reader, int size)
        {
            var numBatches = size / SizeCache<Moba>.Size;
            if(numBatches != (this.mHeader.numBatchesA + this.mHeader.numBatchesB + this.mHeader.numBatchesC))
            {
                Log.Error("Group has inconsistent amount of batches.");
                return false;
            }

            WorldModelRoot parent;
            if(!this.mParent.TryGetTarget(out parent))
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
                    BlendMode = parent.GetMaterial(b.material).BlendMode,
                };

	            this.mBatches.Add(batch);
            }

	        this.Batches = this.mBatches.OrderBy(b => b.BlendMode).ToList().AsReadOnly();

            return true;
        }
    }
}
