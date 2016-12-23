using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Neo.Editing;
using Neo.Scene;
using OpenTK;
using SlimTK;

namespace Neo.IO.Files.Terrain.WoD
{
	public sealed class MapChunk : Terrain.MapChunk
    {
        private ChunkStreamInfo mMainInfo;
        private ChunkStreamInfo mTexInfo;
        private ChunkStreamInfo mObjInfo;

        private BinaryReader mReader;
        private BinaryReader mTexReader;
        private BinaryReader mObjReader;

        private Mcnk mHeader;

        private IntPtr mAlphaDataCompressed;

        private WeakReference<MapArea> mParent;
        private Vector4[] mShadingFloats = new Vector4[145];

        private Dictionary<uint, DataChunk> mOriginalMainChunks = new Dictionary<uint, DataChunk>();
        private Dictionary<uint, DataChunk> mOriginalObjChunks = new Dictionary<uint, DataChunk>();
        private Dictionary<uint, DataChunk> mOriginalTexChunks = new Dictionary<uint, DataChunk>();
        private static readonly uint[] Indices = new uint[768];

        public bool HasMccv { get; private set; }
        public override int AreaId { get { return this.mHeader.AreaId; } set { this.mHeader.AreaId = value; } }
        public override uint Flags { get { return this.mHeader.Flags; } set { this.mHeader.Flags = value; } }

        public MapChunk(ChunkStreamInfo mainInfo, ChunkStreamInfo texInfo, ChunkStreamInfo objInfo, int indexX, int indexY, MapArea parent)
        {
	        this.SpecularFactors = new float[4];
	        this.mIsYInverted = true;
	        this.Parent = new WeakReference<Terrain.MapArea>(parent);
	        this.mParent = new WeakReference<MapArea>(parent);
	        this.mMainInfo = mainInfo;
	        this.mTexInfo = texInfo;
	        this.mObjInfo = objInfo;

	        this.mReader = mainInfo.Stream;
	        this.mTexReader = texInfo.Stream;
	        this.mObjReader = objInfo.Stream;

	        this.IndexX = indexX;
	        this.IndexY = indexY;

            for (var i = 0; i < 145; ++i)
            {
	            this.mShadingFloats[i] = Vector4.One;
            }
        }

        public void AddDoodad(int mcrfValue, BoundingBox box)
        {
            var references = this.DoodadReferences;
            Array.Resize(ref references, references.Length + 1);
            references[references.Length - 1] = mcrfValue;
	        this.DoodadReferences = references;

	        this.DoodadsChanged = true;
        }

        public void TryAddDoodad(int mcrfValue, BoundingBox box)
        {
            var chunkBox = new BoundingBox(new Vector3(this.BoundingBox.Minimum.X, this.BoundingBox.Minimum.Y, float.MinValue),
                new Vector3(this.BoundingBox.Maximum.X, this.BoundingBox.Maximum.Y, float.MaxValue));

            var intersects = chunkBox.Intersects(ref box);
            if (intersects == false)
            {
	            return;
            }

	        var references = this.DoodadReferences;
            Array.Resize(ref references, references.Length + 1);
            references[references.Length - 1] = mcrfValue;
	        this.DoodadReferences = references;

	        this.DoodadsChanged = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.mReader != null)
            {
	            this.mReader.Dispose();
	            this.mReader = null;
            }

            if (this.mTexReader != null)
            {
	            this.mTexReader.Dispose();
	            this.mTexReader = null;
            }

            if (this.mObjReader != null)
            {
	            this.mObjReader.Dispose();
	            this.mObjReader = null;
            }

            if (this.mOriginalMainChunks != null)
            {
	            this.mOriginalMainChunks.Clear();
	            this.mOriginalMainChunks = null;
            }

            if (this.mOriginalObjChunks != null)
            {
	            this.mOriginalObjChunks.Clear();
	            this.mOriginalObjChunks = null;
            }

            if (this.mOriginalTexChunks != null)
            {
	            this.mOriginalTexChunks.Clear();
	            this.mOriginalTexChunks = null;
            }

	        this.mMainInfo = null;
	        this.mTexInfo = null;
	        this.mObjInfo = null;
	        this.mParent = null;
	        this.mShadingFloats = null;

            base.Dispose(disposing);
        }

        public void WriteBaseChunks(BinaryWriter writer)
        {
            var minHeight = this.Vertices.Select(v => v.Position.Z).Min();
	        this.mHeader.Position.Z = minHeight;
            var heights = this.Vertices.Select(v => v.Position.Z - minHeight).ToArray();
            var normals = this.Vertices.SelectMany(
                    v => new[] {(sbyte) (-v.Normal.X * 127.0f), (sbyte) (-v.Normal.Y * 127.0f), (sbyte) (v.Normal.Z * 127.0f)})
                    .ToArray();

            var colors = this.mShadingFloats.Select(v =>
            {
                uint b = (byte) Math.Max(Math.Min((v.Z / 2.0f) * 255.0f, 255), 0);
                uint g = (byte) Math.Max(Math.Min((v.Y / 2.0f) * 255.0f, 255), 0);
                uint r = (byte) Math.Max(Math.Min((v.X / 2.0f) * 255.0f, 255), 0);
                return 0x7F000000 | (b << 16) | (g << 8) | r;
            }).ToArray();

            AddOrReplaceChunk(0x4D435654, heights);
            AddOrReplaceChunk(0x4D434E52, normals);
            if (this.HasMccv)
            {
	            this.mHeader.Flags |= 0x40;
                AddOrReplaceChunk(0x4D434356, colors);
            }

            var totalSize = this.mOriginalMainChunks.Sum(pair => pair.Value.Size + 8);
            totalSize += SizeCache<Mcnk>.Size;

            writer.Write(0x4D434E4B);
            writer.Write(totalSize);
            writer.Write(this.mHeader);

            var startPos = writer.BaseStream.Position;
            foreach(var chunk in this.mOriginalMainChunks)
            {
                writer.Write(chunk.Key);
                writer.Write(chunk.Value.Size);
                writer.Write(chunk.Value.Data);
            }

            var endPos = writer.BaseStream.Position;
            writer.BaseStream.Position = startPos - SizeCache<Mcnk>.Size;
            writer.Write(this.mHeader);
            writer.BaseStream.Position = endPos;
        }

        public void WriteObjChunks(BinaryWriter writer)
        {
            AddOrUpdateChunk(this.mOriginalObjChunks, 0x4D435244, this.DoodadReferences);

            var totalSize = this.mOriginalObjChunks.Sum(pair => pair.Value.Size + 8);
            writer.Write(0x4D434E4B);
            writer.Write(totalSize);

            foreach (var chunk in this.mOriginalObjChunks)
            {
                writer.Write(chunk.Key);
                writer.Write(chunk.Value.Size);
                writer.Write(chunk.Value.Data);
            }
        }

        public void WriteTexChunks(BinaryWriter writer)
        {
            var alpha = SaveAlpha();
            AddOrUpdateChunk(this.mOriginalTexChunks, 0x4D43414C, alpha.ToArray());
            AddOrUpdateChunk(this.mOriginalTexChunks, 0x4D434C59, this.mLayers);

            var totalSize = this.mOriginalTexChunks.Sum(pair => pair.Value.Size + 8);
            writer.Write(0x4D434E4B);
            writer.Write(totalSize);

            foreach (var chunk in this.mOriginalTexChunks)
            {
                writer.Write(chunk.Key);
                writer.Write(chunk.Value.Size);
                writer.Write(chunk.Value.Data);
            }
        }

        public override bool OnTerrainChange(TerrainChangeParameters parameters)
        {
            var changed = base.OnTerrainChange(parameters);

            if(changed)
            {
                MapArea parent;
	            this.mParent.TryGetTarget(out parent);

                var omin = this.BoundingBox.Minimum;
                var omax = this.BoundingBox.Maximum;
	            this.BoundingBox = new BoundingBox(new Vector3(omin.X, omin.Y, this.mMinHeight),
                    new Vector3(omax.X, omax.Y, this.mMaxHeight));

                if (parent != null)
                {
	                parent.UpdateBoundingBox(this.BoundingBox);
                }
            }

            return changed;
        }

        public override void UpdateNormals()
        {
            base.UpdateNormals();

            MapArea parent;
	        this.mParent.TryGetTarget(out parent);
            if (parent != null)
            {
	            parent.UpdateVertices(this);
            }
        }

        public bool Intersect(ref Ray ray, out float distance)
        {
            distance = float.MaxValue;
            if (this.BoundingBox.Intersects(ref ray) == false)
            {
	            return false;
            }

	        var minDist = float.MaxValue;
            var hasHit = false;
            var dir = ray.Direction;
            var orig = ray.Position;

            Vector3 e1, e2, p, T, q;

            for (var i = 0; i < Indices.Length; i += 3)
            {
                var i0 = Indices[i];
                var i1 = Indices[i + 1];
                var i2 = Indices[i + 2];
                Vector3.Subtract(ref this.Vertices[i1].Position, ref this.Vertices[i0].Position, out e1);
                Vector3.Subtract(ref this.Vertices[i2].Position, ref this.Vertices[i0].Position, out e2);

                Vector3.Cross(ref dir, ref e2, out p);
                float det;
                Vector3.Dot(ref e1, ref p, out det);

                if (Math.Abs(det) < 1e-4)
                {
	                continue;
                }

	            var invDet = 1.0f / det;
                Vector3.Subtract(ref orig, ref this.Vertices[i0].Position, out T);
                float u;
                Vector3.Dot(ref T, ref p, out u);
                u *= invDet;

                if (u < 0 || u > 1)
                {
	                continue;
                }

	            Vector3.Cross(ref T, ref e1, out q);
                float v;
                Vector3.Dot(ref dir, ref q, out v);
                v *= invDet;
                if (v < 0 || (u + v) > 1)
                {
	                continue;
                }

	            float t;
                Vector3.Dot(ref e2, ref q, out t);
                t *= invDet;

                if (t < 1e-4)
                {
	                continue;
                }

	            hasHit = true;
                if (t < minDist)
                {
	                minDist = t;
                }
            }

            if(hasHit)
            {
	            distance = minDist;
            }

	        return hasHit;
        }

        public void AsyncLoad()
        {
	        this.mReader.BaseStream.Position = this.mMainInfo.PosStart;
            var chunkSize = this.mReader.ReadInt32();
	        this.mHeader = this.mReader.Read<Mcnk>();
            var hasMccv = false;

            for (var i = 0; i < 4096; ++i)
            {
	            this.AlphaValues[i] = 0xFF;
            }

	        while (this.mReader.BaseStream.Position + 8 <= this.mMainInfo.PosStart + 8 + chunkSize)
            {
                var id = this.mReader.ReadUInt32();
                var size = this.mReader.ReadInt32();

                if (this.mReader.BaseStream.Position + size > this.mMainInfo.PosStart + 8 + chunkSize)
                {
	                break;
                }

	            var cur = this.mReader.BaseStream.Position;

                switch(id)
                {
                    case 0x4D435654:
                        LoadMcvt();
                        break;

                    case 0x4D434E52:
                        LoadMcnr();
                        break;

                    case 0x4D434356:
                        hasMccv = true;
                        LoadMccv();
                        break;

                    case 0x4D434C56:
                        LoadMclv();
                        break;
                }

	            this.mReader.BaseStream.Position = cur;
                var data = this.mReader.ReadBytes(size);
	            this.mOriginalMainChunks.Add(id, new DataChunk {Data = data, Signature = id, Size = size});
            }

            LoadHoles();

            if (hasMccv == false)
            {
                for (var i = 0; i < 145; ++i)
                {
	                this.Vertices[i].Color = 0x7F7F7F7F;
                }
            }

            LoadTexData();
            LoadObjData();

            WorldFrame.Instance.MapManager.OnLoadProgress();
        }

        private void LoadObjData()
        {
	        this.mObjReader.BaseStream.Position = this.mObjInfo.PosStart;
            var chunkSize = this.mObjReader.ReadInt32();

            while(this.mObjReader.BaseStream.Position + 8 <= this.mObjInfo.PosStart + 8 + chunkSize)
            {
                var id = this.mObjReader.ReadUInt32();
                var size = this.mObjReader.ReadInt32();

                if (this.mObjReader.BaseStream.Position + size > this.mObjInfo.PosStart + 8 + chunkSize)
                {
	                break;
                }

	            var cur = this.mObjReader.BaseStream.Position;
                switch(id)
                {
                    case 0x4D435244:
                        LoadMcrd(size);
                        break;
                }

	            this.mObjReader.BaseStream.Position = cur;
                var data = this.mObjReader.ReadBytes(size);
	            this.mOriginalObjChunks.Add(id, new DataChunk { Data = data, Signature = id, Size = size });
	            this.mObjReader.BaseStream.Position = cur + size;
            }
        }

        private void LoadMcrd(int size)
        {
	        this.DoodadReferences = this.mObjReader.ReadArray<int>(size / 4);
            var minPos = this.BoundingBox.Minimum;
            var maxPos = this.BoundingBox.Maximum;

            MapArea parent;
            if (this.mParent.TryGetTarget(out parent) == false)
            {
	            return;
            }

	        foreach (var reference in this.DoodadReferences)
            {
                var inst = parent.DoodadInstances[reference];
                var min = inst.BoundingBox.Minimum;
                var max = inst.BoundingBox.Maximum;

                if (min.X < minPos.X)
                {
	                minPos.X = min.X;
                }
	            if (min.Y < minPos.Y)
	            {
		            minPos.Y = min.Y;
	            }
	            if (min.Z < minPos.Z)
	            {
		            minPos.Z = min.Z;
	            }
	            if (max.X > maxPos.X)
	            {
		            maxPos.X = max.X;
	            }
	            if (max.Y > maxPos.Y)
	            {
		            maxPos.Y = max.Y;
	            }
	            if (max.Z > maxPos.Z)
	            {
		            maxPos.Z = max.Z;
	            }
            }

	        this.ModelBox = new BoundingBox(minPos, maxPos);
        }

        private void LoadTexData()
        {
            try
            {
	            this.mTexReader.BaseStream.Position = this.mTexInfo.PosStart;
                var chunkSize = this.mTexReader.ReadInt32();

                while (this.mTexReader.BaseStream.Position + 8 <= this.mTexInfo.PosStart + 8 + chunkSize)
                {
                    var id = this.mTexReader.ReadUInt32();
                    var size = this.mTexReader.ReadInt32();

                    if (this.mTexReader.BaseStream.Position + size > this.mTexInfo.PosStart + 8 + chunkSize)
                    {
	                    break;
                    }

	                var cur = this.mTexReader.BaseStream.Position;

                    switch (id)
                    {
                        case 0x4D434C59:
                            LoadMcly(size);
                            break;

                        case 0x4D43414C:
	                        this.mAlphaDataCompressed = Marshal.AllocHGlobal(size);
	                        this.mTexReader.ReadToPointer(this.mAlphaDataCompressed, size);
                            break;

                        case 0x4D435348:
                            {
                                if (size >= 512)
                                {
                                    var curPtr = 0;
                                    for (var i = 0; i < 64; ++i)
                                    {
                                        for (var j = 0; j < 8; ++j)
                                        {
                                            var mask = this.mTexReader.ReadByte();
                                            for (var k = 0; k < 8; ++k)
                                            {
	                                            this.AlphaValues[curPtr] &= 0xFFFFFF00;
	                                            this.AlphaValues[curPtr++] |= ((mask & (1 << k)) == 0) ? (byte)0xFF : (byte)0xCC;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }

	                this.mTexReader.BaseStream.Position = cur;
                    var data = this.mTexReader.ReadBytes(size);
	                this.mOriginalTexChunks.Add(id, new DataChunk { Data = data, Signature = id, Size = size });
	                this.mTexReader.BaseStream.Position = cur + size;
                }

                LoadAlpha();

                var textures = new List<Graphics.Texture>();
                MapArea parent;
	            this.mParent.TryGetTarget(out parent);
                if (parent == null)
                {
	                throw new InvalidOperationException("Parent got disposed but loading was still invoked");
                }

	            this.TextureScales = new[] { 1.0f, 1.0f, 1.0f, 1.0f };
	            this.TextureNames = new string[this.mLayers.Length];
	            this.SpecularTextures = new List<Graphics.Texture>();
                for (var i = 0; i < this.mLayers.Length && i < 4; ++i)
                {
                    var texName = parent.GetTextureName(this.mLayers[i].TextureId);
	                this.TextureNames[i] = texName;
                    textures.Add(parent.GetTexture(this.mLayers[i].TextureId));
	                this.SpecularTextures.Add(parent.GetSpecularTexture(this.mLayers[i].TextureId));
	                this.SpecularFactors[i] = parent.IsSpecularTextureLoaded(this.mLayers[i].TextureId) ? 1 : 0;
	                this.TextureScales[i] = parent.GetTextureScale(this.mLayers[i].TextureId);
                }

	            this.Textures = textures;
            }
            finally
            {
                if (this.mAlphaDataCompressed != IntPtr.Zero)
                {
	                Marshal.FreeHGlobal(this.mAlphaDataCompressed);
                }
            }
        }

        private void LoadAlpha()
        {
            var nLayers = Math.Min(this.mLayers.Length, 4);
            for(var i = 1; i < nLayers; ++i)
            {
                if ((this.mLayers[i].Flags & 0x200) != 0)
                {
	                LoadLayerRle(this.mLayers[i], i);
                }
                else if ((this.mLayers[i].Flags & 0x100) != 0)
                {
                    if (WorldFrame.Instance.MapManager.HasNewBlend)
                    {
	                    LoadUncompressed(this.mLayers[i], i);
                    }
                    else
                    {
	                    LoadLayerCompressed(this.mLayers[i], i);
                    }
                }
                else
                {
                    for (var j = 0; j < 4096; ++j)
                    {
	                    this.AlphaValues[j] |= 0xFFu << (8 * i);
                    }
                }
            }

            //mAlphaDataCompressed = null;
        }

        private unsafe void LoadUncompressed(Mcly layerInfo, int layer)
        {
            var ptr = this.mAlphaDataCompressed.ToPointer();
            var startPos = layerInfo.OfsMcal;
            for (var i = 0; i < 4096; ++i)
            {
	            this.AlphaValues[i] |= (uint) ((byte*)ptr)[startPos++] << (8 * layer);
            }
        }

        private unsafe void LoadLayerCompressed(Mcly layerInfo, int layer)
        {
            var ptr = this.mAlphaDataCompressed.ToPointer();
            var startPos = layerInfo.OfsMcal;
            var counter = 0;
            for (var k = 0; k < 64; ++k)
            {
                for (var j = 0; j < 32; ++j)
                {
                    var alpha = ((byte*)ptr)[startPos++];
                    var val1 = alpha & 0xF;
                    var val2 = alpha >> 4;
                    val2 = j == 31 ? val1 : val2;
                    val1 = (byte)((val1 / 15.0f) * 255.0f);
                    val2 = (byte)((val2 / 15.0f) * 255.0f);
	                this.AlphaValues[counter++] |= (uint)val1 << (8 * layer);
	                this.AlphaValues[counter++] |= (uint)val2 << (8 * layer);
                }
            }
        }

        private unsafe void LoadLayerRle(Mcly layerInfo, int layer)
        {
            var ptr = this.mAlphaDataCompressed.ToPointer();
            var counterOut = 0;
            var startPos = layerInfo.OfsMcal;
            while (counterOut < 4096)
            {
                var indicator = ((byte*)ptr)[startPos++];
                if ((indicator & 0x80) != 0)
                {
                    var value = ((byte*)ptr)[startPos++];
                    var repeat = indicator & 0x7F;
                    for (var k = 0; k < repeat && counterOut < 4096; ++k)
                    {
	                    this.AlphaValues[counterOut++] |= (uint)value << (layer * 8);
                    }
                }
                else
                {
                    for (var k = 0; k < (indicator & 0x7F) && counterOut < 4096; ++k)
                    {
	                    this.AlphaValues[counterOut++] |= (uint)((byte*)ptr)[startPos++] << (8 * layer);
                    }
                }
            }
        }

        private void LoadMcvt()
        {
            var heights = this.mReader.ReadArray<float>(145);

            var posx = Metrics.MapMidPoint - this.mHeader.Position.Y;
            var posy = Metrics.MapMidPoint + this.mHeader.Position.X;
            var posz = this.mHeader.Position.Z;

            var counter = 0;

            var minPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for(var i = 0; i < 17; ++i)
            {
                for(var j = 0; j < (((i % 2) != 0) ? 8 : 9); ++j)
                {
                    var height = posz + heights[counter];
                    var x = posx + j * Metrics.UnitSize;
                    if ((i % 2) != 0)
                    {
	                    x += 0.5f * Metrics.UnitSize;
                    }
	                var y = posy - i * Metrics.UnitSize * 0.5f;

	                this.Vertices[counter].Position = new Vector3(x, y, height);

                    if (height < minPos.Z)
                    {
	                    minPos.Z = height;
                    }
	                if (height > maxPos.Z)
	                {
		                maxPos.Z = height;
	                }

	                if (x < minPos.X)
	                {
		                minPos.X = x;
	                }
	                if (x > maxPos.X)
	                {
		                maxPos.X = x;
	                }
	                if (y < minPos.Y)
	                {
		                minPos.Y = y;
	                }
	                if (y > maxPos.Y)
	                {
		                maxPos.Y = y;
	                }

	                this.Vertices[counter].TexCoordAlpha = new Vector2(j / 8.0f + ((i % 2) != 0 ? (0.5f / 8.0f) : 0), i / 16.0f);
	                this.Vertices[counter].TexCoord = new Vector2(j + ((i % 2) != 0 ? 0.5f : 0.0f), i * 0.5f);
                    ++counter;
                }
            }

	        this.mMinHeight = minPos.Z;
	        this.mMaxHeight = maxPos.Z;

	        this.BoundingBox = new BoundingBox(minPos, maxPos);
	        this.mMidPoint = minPos + (maxPos - minPos) / 2.0f;
        }

        private void LoadMcnr()
        {
            var normals = this.mReader.ReadArray<sbyte>(145 * 3);
            var counter = 0;

            for (var i = 0; i < 17; ++i)
            {
                for (var j = 0; j < (((i % 2) != 0) ? 8 : 9); ++j)
                {
                    var nx = normals[counter * 3] / -127.0f;
                    var ny = normals[counter * 3 + 1] / -127.0f;
                    var nz = normals[counter * 3 + 2] / 127.0f;

	                this.Vertices[counter].Normal = new Vector3(nx, ny, nz);
                    ++counter;
                }
            }
        }

        private void LoadMccv()
        {
            var colors = this.mReader.ReadArray<uint>(145);
            for (var i = 0; i < 145; ++i)
            {
	            this.Vertices[i].Color = colors[i];
                var r = (colors[i] >> 16) & 0xFF;
                var g = (colors[i] >> 8) & 0xFF;
                var b = (colors[i]) & 0xFF;
                var a = (colors[i] >> 24) & 0xFF;

	            this.mShadingFloats[i] = new Vector4(b * 2.0f / 255.0f, g * 2.0f / 255.0f, r * 2.0f / 255.0f, a * 2.0f / 255.0f);
            }
        }

        private void LoadMclv()
        {
            var colors = this.mReader.ReadArray<uint>(145);
            for(var i = 0; i < 145; ++i)
            {
	            this.Vertices[i].AdditiveColor = colors[i];
            }
        }

        private void LoadHoles()
        {
            if((this.mHeader.Flags & 0x10000) == 0)
            {
                for (var i = 0; i < 4; ++i)
                {
                    for (var j = 0; j < 4; ++j)
                    {
                        var baseIndex = i * 2 * 8 + j * 2;
                        var mask = (this.mHeader.Holes & (1 << (i * 4 + j))) != 0;
	                    this.HoleValues[baseIndex] = this.HoleValues[baseIndex + 1] = this.HoleValues[baseIndex + 8] = this.HoleValues[baseIndex + 9] = (byte)(mask ? 0x00 : 0xFF);
                    }
                }
            }
            else
            {
                var holeBytes = new byte[8];
                Buffer.BlockCopy(BitConverter.GetBytes(this.mHeader.Mcvt), 0, holeBytes, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(this.mHeader.Mcnr), 0, holeBytes, 4, 4);

                for(var i = 0; i < 8; ++i)
                {
                    for(var j = 0; j < 8; ++j)
                    {
	                    this.HoleValues[i * 8 + j] = (byte) (((holeBytes[i] >> j) & 1) != 0 ? 0x00 : 0xFF);
                    }
                }
            }
        }

        private void LoadMcly(int size)
        {
	        this.mLayers = this.mTexReader.ReadArray<Mcly>(size / SizeCache<Mcly>.Size);
        }

        private unsafe DataChunk ChunkFromArray<T>(uint signature, T[] data) where T : struct
        {
            var byteData = new byte[data.Length * SizeCache<T>.Size];
            fixed(byte* ptr = byteData)
            {
	            UnsafeNativeMethods.CopyMemory(ptr, (byte*) SizeCache<T>.GetUnsafePtr(ref data[0]), byteData.Length);
            }

	        return new DataChunk
            {
                Data = byteData,
                Signature = signature,
                Size = byteData.Length
            };
        }

        private void AddOrReplaceChunk<T>(uint signature, T[] data) where T : struct
        {
            var chunk = ChunkFromArray(signature, data);
            if (this.mOriginalMainChunks.ContainsKey(signature) == false)
            {
	            this.mOriginalMainChunks.Add(signature, chunk);
            }
            else
            {
                var old = this.mOriginalMainChunks[signature];
                if (old.Size >= chunk.Size)
                {
	                Buffer.BlockCopy(chunk.Data, 0, old.Data, 0, chunk.Size);
                }
                else
                {
	                old = chunk;
                }

	            this.mOriginalMainChunks[signature] = old;
            }
        }

        protected override bool HandleMccvPaint(TerrainChangeParameters parameters)
        {
            var amount = (parameters.Amount / 75.0f) * (float)parameters.TimeDiff.TotalSeconds;
            var changed = false;

            var destColor = parameters.Shading;
            if(parameters.Inverted)
            {
                destColor.X = 2 - destColor.X;
                destColor.Y = 2 - destColor.Y;
                destColor.Z = 2 - destColor.Z;
            }

            var radius = parameters.OuterRadius;
            for(var i = 0; i < 145; ++i)
            {
                var p = this.Vertices[i].Position;
                var dist = (new Vector2(p.X, p.Y) - new Vector2(parameters.Center.X, parameters.Center.Y)).Length;
                if (dist > radius)
                {
	                continue;
                }

	            this.HasMccv = true;
                changed = true;
                var factor = dist / radius;
                if (dist < parameters.InnerRadius)
                {
	                factor = 1.0f;
                }

	            var curColor = this.mShadingFloats[i];
                var dr = destColor.X - curColor.Z;
                var dg = destColor.Y - curColor.Y;
                var db = destColor.Z - curColor.X;

                var cr = Math.Min(Math.Abs(dr), amount * factor);
                var cg = Math.Min(Math.Abs(dg), amount * factor);
                var cb = Math.Min(Math.Abs(db), amount * factor);

                if (dr < 0)
                {
                    curColor.Z -= cr;
                    if (curColor.Z < destColor.X)
                    {
	                    curColor.Z = destColor.X;
                    }
                }
                else
                {
                    curColor.Z += cr;
                    if (curColor.Z > destColor.X)
                    {
	                    curColor.Z = destColor.X;
                    }
                }
                if (dg < 0)
                {
                    curColor.Y -= cg;
                    if (curColor.Y < destColor.Y)
                    {
	                    curColor.Y = destColor.Y;
                    }
                }
                else
                {
                    curColor.Y += cg;
                    if (curColor.Y > destColor.Y)
                    {
	                    curColor.Y = destColor.Y;
                    }
                }
                if (db < 0)
                {
                    curColor.X -= cb;
                    if (curColor.X < destColor.Z)
                    {
	                    curColor.X = destColor.Z;
                    }
                }
                else
                {
                    curColor.X += cb;
                    if (curColor.X > destColor.Z)
                    {
	                    curColor.X = destColor.Z;
                    }
                }

	            this.mShadingFloats[i] = curColor;

                curColor.X = Math.Min(Math.Max(curColor.X, 0), 2);
                curColor.Y = Math.Min(Math.Max(curColor.Y, 0), 2);
                curColor.Z = Math.Min(Math.Max(curColor.Z, 0), 2);

                var r = (byte) ((curColor.Z / 2.0f) * 255.0f);
                var g = (byte) ((curColor.Y / 2.0f) * 255.0f);
                var b = (byte) ((curColor.X / 2.0f) * 255.0f);
                var a = (byte) ((curColor.W / 2.0f) * 255.0f);

                var color = (uint)((a << 24) | (r << 16) | (g << 8) | b);
	            this.Vertices[i].Color = color;
            }

            return changed;
        }

        protected override int AddTextureLayer(string textureName)
        {
            var old = this.TextureNames;
	        this.TextureNames = new string[this.TextureNames.Count + 1];
            for (var i = 0; i < old.Count; ++i)
            {
	            this.TextureNames[i] = old[i];
            }

	        this.TextureNames[this.TextureNames.Count - 1] = textureName;

            MapArea parent;
            if (this.mParent.TryGetTarget(out parent) == false)
            {
	            throw new InvalidOperationException("Couldnt get parent of map chunk");
            }

	        var texId = parent.GetOrAddTexture(textureName);
            var layer = new Mcly
            {
                Flags = 0,
                TextureId = texId,
                EffectId = -1,
                OfsMcal = 0,
                Padding = 0
            };

            var layers = this.mLayers;
	        this.mLayers = new Mcly[layers.Length + 1];
            for (var i = 0; i < layers.Length; ++i)
            {
	            this.mLayers[i] = layers[i];
            }

	        this.mLayers[layers.Length] = layer;

	        this.Textures.Add(parent.GetTexture(texId));
	        this.SpecularTextures.Add(parent.GetSpecularTexture(texId));
	        this.SpecularFactors[this.SpecularTextures.Count - 1] = parent.IsSpecularTextureLoaded(texId) ? 1 : 0;
	        this.TexturesChanged = true;
            return this.mLayers.Length - 1;
        }

        private unsafe void AddOrUpdateChunk<T>(Dictionary<uint, DataChunk> chunks, uint signature, T[] data) where T : struct
        {
            var chunk = chunks.ContainsKey(signature) ? chunks[signature] : new DataChunk {Signature = signature};
            var totalSize = SizeCache<T>.Size * data.Length;
            chunk.Size = totalSize;
            chunk.Data = new byte[totalSize];
            if (chunk.Data.Length > 0)
            {
                var ptr = SizeCache<T>.GetUnsafePtr(ref data[0]);
                fixed (byte* bptr = chunk.Data)
                {
	                UnsafeNativeMethods.CopyMemory(bptr, (byte*) ptr, totalSize);
                }
            }

            if (chunks.ContainsKey(signature))
            {
	            chunks[signature] = chunk;
            }
            else
            {
	            chunks.Add(signature, chunk);
            }
        }

        private MemoryStream SaveAlpha()
        {
            if (this.mLayers.Length == 0)
            {
	            return new MemoryStream();
            }

	        var strm = new MemoryStream();
            var writer = new BinaryWriter(strm);

            var curPos = 0;
	        this.mLayers[0].Flags &= ~0x300u;
	        this.mLayers[0].OfsMcal = 0;
            for (var i = 1; i < this.mLayers.Length; ++i)
            {
                bool compressed;
                var data = GetSavedAlphaForLayer(i, out compressed);
	            this.mLayers[i].OfsMcal = curPos;
                if (compressed)
                {
	                this.mLayers[i].Flags |= 0x300;
                }
                else
                {
	                this.mLayers[i].Flags |= 0x100;
	                this.mLayers[i].Flags &= ~0x200u;
                }

                writer.Write(data);
                curPos += data.Length;
            }

            return strm;
        }

        private float CalculateAlphaHomogenity(int layer)
        {
            var numCompressable = 1;
            var lastAlpha = (this.AlphaValues[0] >> (layer * 8)) & 0xFF;
            for (var i = 1; i < 4096; ++i)
            {
                var value = (this.AlphaValues[i] >> (layer * 8)) & 0xFF;
                if (value == lastAlpha)
                {
	                ++numCompressable;
                }

	            lastAlpha = value;
            }

            return numCompressable / 4096.0f;
        }

        private byte[] GetSavedAlphaForLayer(int layer, out bool compressed)
        {
            compressed = false;
            var homogenity = CalculateAlphaHomogenity(layer);
            if (homogenity < float.MaxValue)
            {
                compressed = true;
                return GetAlphaCompressed(layer);
            }

            return GetAlphaUncompressed(layer);
        }

        public override void SetHole(IntersectionParams intersection, bool add)
        {
            float holesize = CHUNKSIZE / 4.0f;
            var min = this.BoundingBox.Minimum;
            var intersect = new Vector2(intersection.TerrainPosition.X, intersection.TerrainPosition.Y);

            bool use64bit = (this.mHeader.Flags & 0x10000) == 0x10000;
            if (!use64bit)
            {
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        RectangleF bounds = new RectangleF
                        (
                            min.X + (x * holesize),
                            min.Y + (y * holesize),
                            holesize,
                            holesize
                        );

                        if (bounds.Contains(intersect))
                        {
                            y = 3 - y; //Inverse
                            var baseIndex = y * 2 * 8 + x * 2;
                            int bit = (1 << (y * 4 + x));

                            if (add)
                            {
	                            this.mHeader.Holes |= bit;
                            }
                            else
                            {
	                            this.mHeader.Holes &= ~bit;
                            }

	                        this.HoleValues[baseIndex] = this.HoleValues[baseIndex + 1] = this.HoleValues[baseIndex + 8] = this.HoleValues[baseIndex + 9] = (byte)(add ? 0x00 : 0xFF);
                            return;
                        }
                    }
                }
            }
            else
            {
                holesize = CHUNKSIZE / 8.0f;
                var holeBytes = new byte[8];
                Buffer.BlockCopy(BitConverter.GetBytes(this.mHeader.Mcvt), 0, holeBytes, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(this.mHeader.Mcnr), 0, holeBytes, 4, 4);

                for (var x = 0; x < 8; ++x)
                {
                    for (var y = 0; y < 8; ++y)
                    {
                        RectangleF bounds = new RectangleF
                        (
                            min.X + (x * holesize),
                            min.Y + (y * holesize),
                            holesize,
                            holesize
                        );

                        if (bounds.Contains(intersect))
                        {
                            y = (7 - y); //Inverse
                            byte bit = (byte)(1 << x);

                            if (add)
                            {
	                            holeBytes[y] |= bit;
                            }
                            else
                            {
	                            holeBytes[y] = (byte)(holeBytes[y] & ~bit);
                            }

	                        this.mHeader.Mcvt = BitConverter.ToInt32(holeBytes, 0);
	                        this.mHeader.Mcnr = BitConverter.ToInt32(holeBytes, 4);

                            int h = y * 8 + x;
	                        this.HoleValues[h] = (byte)(add ? 0x00 : 0xFF);
                            return;
                        }
                    }
                }
            }

        }

        public override void SetHoleBig(bool add)
        {
            bool use64bit = (this.mHeader.Flags & 0x10000) == 0x10000;
            if (!use64bit)
            {
	            this.mHeader.Holes = add ? int.MaxValue : 0; //Set all shown/hidden

                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        var baseIndex = y * 2 * 8 + x * 2;
	                    this.HoleValues[baseIndex] = this.HoleValues[baseIndex + 1] = this.HoleValues[baseIndex + 8] = this.HoleValues[baseIndex + 9] = (byte)(add ? 0x0 : 0xFF);
                    }
                }
            }
            else
            {
	            this.mHeader.Mcvt = add ? int.MaxValue : 0; //Set all shown/hidden
	            this.mHeader.Mcnr = add ? int.MaxValue : 0; //Set all shown/hidden
                for (var x = 0; x < 8; ++x)
                {
                    for (var y = 0; y < 8; ++y)
                    {
                        int h = y * 8 + x;
	                    this.HoleValues[h] = (byte)(add ? 0x00 : 0xFF);
                    }
                }
            }
        }

        static MapChunk()
        {
            var indices = Indices;
            for (uint y = 0; y < 8; ++y)
            {
                for (uint x = 0; x < 8; ++x)
                {
                    var i = y * 8 * 12 + x * 12;
                    indices[i + 0] = y * 17 + x;
                    indices[i + 2] = y * 17 + x + 1;
                    indices[i + 1] = y * 17 + x + 9;

                    indices[i + 3] = y * 17 + x + 1;
                    indices[i + 5] = y * 17 + x + 18;
                    indices[i + 4] = y * 17 + x + 9;

                    indices[i + 6] = y * 17 + x + 18;
                    indices[i + 8] = y * 17 + x + 17;
                    indices[i + 7] = y * 17 + x + 9;

                    indices[i + 9] = y * 17 + x + 17;
                    indices[i + 11] = y * 17 + x;
                    indices[i + 10] = y * 17 + x + 9;
                }
            }
        }
    }
}
