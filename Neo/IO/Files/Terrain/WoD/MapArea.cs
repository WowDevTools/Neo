using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Neo.Editing;
using Neo.IO.Files.Models;
using Neo.Scene;
using Neo.Scene.Texture;
using OpenTK;
using SlimTK;
using Warcraft.WDT.Chunks;

namespace Neo.IO.Files.Terrain.WoD
{
	public class ChunkStreamInfo
    {
        public BinaryReader Stream;
        public int PosStart;
        public int Size;
    }

	public class MapArea : Terrain.MapArea
    {
        private Stream mMainStream;
        private Stream mTexStream;
        private Stream mObjStream;

        private BinaryReader mReader;
        private BinaryReader mTexReader;
        private BinaryReader mObjReader;

        private List<float> mTextureScales = new List<float>();
        private List<ChunkStreamInfo> mMainChunks = new List<ChunkStreamInfo>();
        private List<ChunkStreamInfo> mTexChunks = new List<ChunkStreamInfo>();
        private List<ChunkStreamInfo> mObjChunks = new List<ChunkStreamInfo>();

        private List<MapChunk> mChunks = new List<MapChunk>();

        private List<LoadedModel> mWmoInstances = new List<LoadedModel>();

        private Dictionary<uint, DataChunk> mBaseChunks = new Dictionary<uint, DataChunk>();
        private Dictionary<uint, DataChunk> mObjOrigChunks = new Dictionary<uint, DataChunk>();
        private Dictionary<uint, DataChunk> mTexOrigChunks = new Dictionary<uint, DataChunk>();

        private bool mWasChanged;

        private Mddf[] mDoodadDefs = new Mddf[0];
        private List<string> mDoodadNames = new List<string>();
        private int[] mDoodadNameIds = new int[0];

        public MapArea(string continent, int ix, int iy)
        {
	        this.Continent = continent;
	        this.IndexX = ix;
	        this.IndexY = iy;
        }

        public override void Save()
        {
            if (this.mWasChanged == false)
            {
	            return;
            }

	        var hasMccv = this.mChunks.Any(c => c != null && c.HasMccv);
            if(hasMccv)
            {
	            var wdt = WorldFrame.Instance.MapManager.CurrentWdt;
	            if(!wdt.Header.Flags.HasFlag(WorldTableFlags.UsesVertexShading))
	            {
		            wdt.Header.Flags |= WorldTableFlags.UsesVertexShading;
		            // TODO: Where and how?
		            // wdt.Save(WorldFrame.Instance.MapManager.Continent);
	            }
            }

            WriteBaseFile();
            WriteObjFile();
            WriteTexFile();
        }

        public override void OnUpdateModelPositions(TerrainChangeParameters parameters)
        {
            var center = new Vector2(parameters.Center.X, parameters.Center.Y);
            foreach (var inst in this.DoodadInstances)
            {
                if (inst == null || inst.RenderInstance == null)
                {
	                continue;
                }

	            var pos = this.mDoodadDefs[inst.MddfIndex].Position;
                var oldPos = pos;
                var invZ = 64.0f * Metrics.TileSize - pos.Z;
                var dist = (new Vector2(pos.X, invZ) - center).Length;
                if (dist > parameters.OuterRadius)
                {
	                continue;
                }

	            if (WorldFrame.Instance.MapManager.GetLandHeight(pos.X, pos.Z, out pos.Y))
                {
	                this.mDoodadDefs[inst.MddfIndex].Position = pos;
                    inst.RenderInstance.SetPosition(new Vector3(0, 0, pos.Y - oldPos.Y));
                }
            }
        }

        public override void UpdateNormals()
        {
            foreach (var chunk in this.mChunks)
            {
                if (chunk != null)
                {
	                chunk.UpdateNormals();
                }
            }
        }

        public override bool OnTextureTerrain(TextureChangeParameters parameters)
        {
            var changed = false;
            foreach (var chunk in this.mChunks)
            {
                if (chunk == null)
                {
	                continue;
                }

	            if (chunk.OnTextureTerrain(parameters))
	            {
		            changed = true;
	            }
            }

            return changed;
        }

        public void UpdateBoundingBox(BoundingBox chunkBox)
        {
            var minPos = chunkBox.Minimum;
            var maxPos = chunkBox.Maximum;

            var omin = this.BoundingBox.Minimum;
            var omax = this.BoundingBox.Maximum;

            omin.X = Math.Min(omin.X, minPos.X);
            omin.Y = Math.Min(omin.Y, minPos.Y);
            omin.Z = Math.Min(omin.Z, minPos.Z);
            omax.X = Math.Max(omax.X, maxPos.X);
            omax.Y = Math.Max(omax.Y, maxPos.Y);
            omax.Z = Math.Max(omax.Z, maxPos.Z);

	        this.BoundingBox = new BoundingBox(omin, omax);
        }

        public void UpdateVertices(MapChunk chunk)
        {
            if (chunk == null)
            {
	            return;
            }

	        var ix = chunk.IndexX;
            var iy = chunk.IndexY;

            var index = (ix + iy * 16) * 145;
            for (var i = 0; i < 145; ++i)
            {
	            this.FullVertices[i + index] = chunk.Vertices[i];
            }
        }

        public override bool OnChangeTerrain(TerrainChangeParameters parameters)
        {
            var changed = false;
            foreach (var chunk in this.mChunks)
            {
                if (chunk == null)
                {
	                continue;
                }

	            if (chunk.OnTerrainChange(parameters))
	            {
		            changed = true;
	            }
            }

            if (changed)
            {
	            this.mWasChanged = true;
            }

	        return changed;
        }

        public override bool Intersect(ref Ray ray, out Terrain.MapChunk chunk, out float distance)
        {
            distance = float.MaxValue;
            chunk = null;

            var mindistance = float.MaxValue;
            if (this.BoundingBox.Intersects(ref ray) == false)
            {
	            return false;
            }

	        Terrain.MapChunk chunkHit = null;
            var hasHit = false;
            foreach(var cnk in this.mChunks)
            {
                float dist;
                if (cnk.Intersect(ref ray, out dist) == false)
                {
	                continue;
                }

	            hasHit = true;
                if (dist >= mindistance)
                {
	                continue;
                }

	            mindistance = dist;
                chunkHit = cnk;
            }

            chunk = chunkHit;
            distance = mindistance;
            return hasHit;
        }

        public float GetTextureScale(int index)
        {
            if (index >= this.mTextureScales.Count)
            {
	            throw new IndexOutOfRangeException();
            }

	        return this.mTextureScales[index];
        }

        public override Terrain.MapChunk GetChunk(int index)
        {
            if (index >= this.mChunks.Count)
            {
	            throw new IndexOutOfRangeException();
            }

	        return this.mChunks[index];
        }

        public override void AsyncLoad()
        {
            try
            {
	            this.mMainStream =
                    FileManager.Instance.Provider.OpenFile(string.Format(@"World\Maps\{0}\{0}_{1}_{2}.adt", this.Continent, this.IndexX, this.IndexY));

	            this.mTexStream = FileManager.Instance.Provider.OpenFile(string.Format(@"World\Maps\{0}\{0}_{1}_{2}_tex0.adt", this.Continent, this.IndexX, this.IndexY));

	            this.mObjStream = FileManager.Instance.Provider.OpenFile(string.Format(@"World\Maps\{0}\{0}_{1}_{2}_obj0.adt", this.Continent, this.IndexX, this.IndexY));

                if (this.mMainStream == null || this.mTexStream == null || this.mObjStream == null)
                {
	                this.IsValid = false;
                    return;
                }

	            this.mReader = new BinaryReader(this.mMainStream);
	            this.mTexReader = new BinaryReader(this.mTexStream);
	            this.mObjReader = new BinaryReader(this.mObjStream);

                InitChunkInfos();

	            this.mTexStream.Position = 0;
                InitTextureNames();

	            this.mObjStream.Position = 0;
                InitModels();

                InitChunks();

                BuildDataForSave();
            }
            catch(Exception e)
            {
                Log.Error(e.ToString());
                Log.Warning(
	                $"Attempted to load ADT {this.Continent}_{this.IndexX}_{this.IndexY}.adt but it caused an error: {e.Message}. Skipping the adt.");
	            this.IsValid = false;
                return;
            }

	        this.IsValid = true;
        }

        private void BuildDataForSave()
        {
	        this.mReader.BaseStream.Position = 0;
            while(this.mReader.BaseStream.Position + 8 < this.mReader.BaseStream.Length)
            {
                var signature = this.mReader.ReadUInt32();
                var size = this.mReader.ReadInt32();
                // skip the MCNK, chunks will write it
                if(signature == 0x4D434E4B)
                {
	                this.mReader.BaseStream.Position += size;
                    continue;
                }

                var data = this.mReader.ReadBytes(size);
	            this.mBaseChunks.Add(signature, new DataChunk
                {
                    Data = data,
                    Signature = signature,
                    Size = size
                });
            }

	        this.mObjReader.BaseStream.Position = 0;
            while (this.mObjReader.BaseStream.Position + 8 < this.mObjReader.BaseStream.Length)
            {
                var signature = this.mObjReader.ReadUInt32();
                var size = this.mObjReader.ReadInt32();
                if (signature == 0x4D434E4B)
                {
	                this.mObjReader.BaseStream.Position += size;
                    continue;
                }

                var data = this.mObjReader.ReadBytes(size);
	            this.mObjOrigChunks.Add(signature, new DataChunk
                {
                    Data = data,
                    Signature = signature,
                    Size = size
                });
            }

	        this.mTexReader.BaseStream.Position = 0;
            while (this.mTexReader.BaseStream.Position + 8 < this.mTexReader.BaseStream.Length)
            {
                var signature = this.mTexReader.ReadUInt32();
                var size = this.mTexReader.ReadInt32();
                if (signature == 0x4D434E4B)
                {
	                this.mTexReader.BaseStream.Position += size;
                    continue;
                }

                var data = this.mTexReader.ReadBytes(size);
	            this.mTexOrigChunks.Add(signature, new DataChunk
                {
                    Data = data,
                    Signature = signature,
                    Size = size
                });
            }
        }

        private void InitChunkInfos()
        {
            for(var i = 0; i < 256; ++i)
            {
	            if (SeekNextMcnk(this.mReader) == false)
	            {
		            throw new InvalidOperationException("Unable to read MCNK from ADT");
	            }

	            if (SeekNextMcnk(this.mTexReader) == false)
	            {
		            throw new InvalidOperationException("Unable to read MCNK from TEX ADT");
	            }

	            if (SeekNextMcnk(this.mObjReader) == false)
	            {
		            throw new InvalidOperationException("Unable to read MCNK from obj ADT");
	            }

	            this.mMainChunks.Add(new ChunkStreamInfo
                {
                    PosStart = (int) this.mMainStream.Position,
                    Size = this.mReader.ReadInt32(),
                    Stream = this.mReader
                });

	            this.mTexChunks.Add(new ChunkStreamInfo
                {
                    PosStart = (int) this.mTexStream.Position,
                    Size = this.mTexReader.ReadInt32(),
                    Stream = this.mTexReader
                });

	            this.mObjChunks.Add(new ChunkStreamInfo
                {
                    PosStart = (int) this.mObjStream.Position,
                    Size = this.mObjReader.ReadInt32(),
                    Stream = this.mObjReader
                });

	            this.mReader.ReadBytes(this.mMainChunks.Last().Size);
	            this.mTexReader.ReadBytes(this.mTexChunks.Last().Size);
	            this.mObjReader.ReadBytes(this.mObjChunks.Last().Size);
            }
        }

        private void InitModels()
        {
            InitWmoModels();
            InitM2Models();
        }

        private void InitM2Models()
        {
	        if (SeekChunk(this.mObjReader, Chunks.Mmdx) == false)
	        {
		        return;
	        }

            var size = this.mObjReader.ReadInt32();
            var bytes = this.mObjReader.ReadBytes(size);
            var fullString = Encoding.ASCII.GetString(bytes);
            var modelNames = fullString.Split('\0');
	        this.mDoodadNames.AddRange(modelNames.ToList());
            var modelNameLookup = new Dictionary<int, string>();
            var curOffset = 0;
            foreach (var name in modelNames)
            {
                modelNameLookup.Add(curOffset, name);
                curOffset += name.Length + 1;
            }

	        if (SeekChunk(this.mObjReader, Chunks.Mmid) == false)
	        {
		        return;
	        }

            size = this.mObjReader.ReadInt32();
	        this.mDoodadNameIds = this.mObjReader.ReadArray<int>(size / 4);

	        if (SeekChunk(this.mObjReader, Chunks.Mddf) == false)
	        {
		        return;
	        }

            size = this.mObjReader.ReadInt32();
	        this.mDoodadDefs = this.mObjReader.ReadArray<Mddf>(size / SizeCache<Mddf>.Size);

            var index = -1;
            foreach (var entry in this.mDoodadDefs)
            {
                ++index;
	            if (entry.Mmid >= this.mDoodadNameIds.Length)
	            {
		            continue;
	            }

                var nameId = this.mDoodadNameIds[entry.Mmid];
                string modelName;
	            if (modelNameLookup.TryGetValue(nameId, out modelName) == false)
	            {
		            continue;
	            }

                var position = new Vector3(entry.Position.X, 64.0f * Metrics.TileSize - entry.Position.Z,
                    entry.Position.Y);
                var rotation = new Vector3(360.0f - entry.Rotation.X, 360.0f - entry.Rotation.Z, entry.Rotation.Y - 90);
                var scale = entry.Scale / 1024.0f;

                var instance = WorldFrame.Instance.M2Manager.AddInstance(modelName, entry.UniqueId, position, rotation,
                    new Vector3(scale));

	            this.DoodadInstances.Add(new M2Instance
                {
                    Hash = modelName.ToUpperInvariant().GetHashCode(),
                    Uuid = entry.UniqueId,
                    BoundingBox = (instance != null ? instance.BoundingBox : new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue))),
                    RenderInstance = instance,
                    MddfIndex = index
                });
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void InitWmoModels()
        {
	        if (SeekChunk(this.mObjReader, Chunks.Mwmo) == false)
	        {
		        return;
	        }

            var size = this.mObjReader.ReadInt32();
            var bytes = this.mObjReader.ReadBytes(size);
            var modelNameLookup = new Dictionary<int, string>();
            var curOffset = 0;
            var curBytes = new List<byte>();

            for(var i = 0; i < bytes.Length; ++i)
            {
	            if (bytes[i] == 0)
	            {
		            if (curBytes.Count > 0)
		            {
			            modelNameLookup.Add(curOffset, Encoding.ASCII.GetString(curBytes.ToArray()));
		            }

		            curOffset = i + 1;
		            curBytes.Clear();
	            }
	            else
	            {
		            curBytes.Add(bytes[i]);
	            }
            }

	        if (SeekChunk(this.mObjReader, Chunks.Mwid) == false)
	        {
		        return;
	        }

            size = this.mObjReader.ReadInt32();
            var modelNameIds = this.mObjReader.ReadArray<int>(size / 4);

	        if (SeekChunk(this.mObjReader, Chunks.Modf) == false)
	        {
		        return;
	        }

            size = this.mObjReader.ReadInt32();
            var modf = this.mObjReader.ReadArray<Modf>(size / SizeCache<Modf>.Size);

            foreach(var entry in modf)
            {
	            if (entry.Mwid >= modelNameIds.Length)
	            {
		            continue;
	            }

                var nameId = modelNameIds[entry.Mwid];
                string modelName;
	            if (modelNameLookup.TryGetValue(nameId, out modelName) == false)
	            {
		            continue;
	            }

                var position = new Vector3(entry.Position.X, 64.0f * Metrics.TileSize - entry.Position.Z,
                    entry.Position.Y);
                var rotation = new Vector3(360.0f - entry.Rotation.X, 360.0f - entry.Rotation.Z, entry.Rotation.Y - 90);

                WorldFrame.Instance.WmoManager.AddInstance(modelName, entry.UniqueId, position, rotation);
	            this.mWmoInstances.Add(new LoadedModel(modelName, entry.UniqueId));
            }
        }

        private void InitTextureNames()
        {
	        if (SeekChunk(this.mTexReader, Chunks.Mtex) == false)
	        {
		        return;
	        }

            var size = this.mTexReader.ReadInt32();
            var bytes = this.mTexReader.ReadBytes(size);
            var fullString = Encoding.ASCII.GetString(bytes);
	        this.TextureNames.AddRange(fullString.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries));
	        this.TextureNames.ForEach(t => this.mTextures.Add(TextureManager.Instance.GetTexture(t)));

	        this.TextureNames.ForEach(t =>
            {
                var loadInfo = Texture.TextureLoader.LoadHeaderOnly(t);
                var width = 256;
                var height = 256;
                if(loadInfo != null)
                {
                    width = loadInfo.Width;
                    height = loadInfo.Height;
                }

	            if (width <= 256 || height <= 256 || loadInfo == null)
	            {
		            this.mTextureScales.Add(1.0f);
	            }
	            else
	            {
		            this.mTextureScales.Add(256.0f / (2 * loadInfo.Width));
	            }
            });

            LoadSpecularTextures();
        }

        private void InitChunks()
        {
            var minPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            var modelMin = new Vector3(float.MaxValue);
            var modelMax = new Vector3(float.MinValue);

            for (var i = 0; i < 256; ++i)
            {
                var chunk = new MapChunk(this.mMainChunks[i], this.mTexChunks[i], this.mObjChunks[i], i % 16, i / 16, this);
                chunk.AsyncLoad();
                var bbmin = chunk.BoundingBox.Minimum;
                var bbmax = chunk.BoundingBox.Maximum;
	            if (bbmin.X < minPos.X)
	            {
		            minPos.X = bbmin.X;
	            }
	            if (bbmax.X > maxPos.X)
	            {
		            maxPos.X = bbmax.X;
	            }
	            if (bbmin.Y < minPos.Y)
	            {
		            minPos.Y = bbmin.Y;
	            }
	            if (bbmax.Y > maxPos.Y)
	            {
		            maxPos.Y = bbmax.Y;
	            }
	            if (bbmin.Z < minPos.Z)
	            {
		            minPos.Z = bbmin.Z;
	            }
	            if (bbmax.Z > maxPos.Z)
	            {
		            maxPos.Z = bbmax.Z;
	            }

                bbmin = chunk.ModelBox.Minimum;
                bbmax = chunk.ModelBox.Maximum;
	            if (bbmin.X < modelMin.X)
	            {
		            modelMin.X = bbmin.X;
	            }
	            if (bbmax.X > modelMax.X)
	            {
		            modelMax.X = bbmax.X;
	            }
	            if (bbmin.Y < modelMin.Y)
	            {
		            modelMin.Y = bbmin.Y;
	            }
	            if (bbmax.Y > modelMax.Y)
	            {
		            modelMax.Y = bbmax.Y;
	            }
	            if (bbmin.Z < modelMin.Z)
	            {
		            modelMin.Z = bbmin.Z;
	            }
	            if (bbmax.Z > modelMax.Z)
	            {
		            modelMax.Z = bbmax.Z;
	            }

	            this.mChunks.Add(chunk);
                Array.Copy(chunk.Vertices, 0, this.FullVertices, i * 145, 145);
            }

	        this.BoundingBox = new BoundingBox(minPos, maxPos);
	        this.ModelBox = new BoundingBox(modelMin, modelMax);
        }

        private void WriteBaseFile()
        {
            using (var strm = FileManager.Instance.GetOutputStream(string.Format(@"World\Maps\{0}\{0}_{1}_{2}.adt", this.Continent, this.IndexX, this.IndexY)))
            {
                var writer = new BinaryWriter(strm);
                foreach (var pair in this.mBaseChunks)
                {
                    writer.Write(pair.Value.Signature);
                    writer.Write(pair.Value.Size);
                    writer.Write(pair.Value.Data);
                }

                foreach (var chunk in this.mChunks)
                {
                    if (chunk == null)
                    {
	                    continue;
                    }

	                chunk.WriteBaseChunks(writer);
                }
            }
        }

        private void WriteObjFile()
        {
            using (var strm = FileManager.Instance.GetOutputStream(string.Format(@"World\Maps\{0}\{0}_{1}_{2}_obj0.adt", this.Continent, this.IndexX, this.IndexY)))
            {
                var writer = new BinaryWriter(strm);
                CreateOrUpdateObjChunk(Chunks.Mddf, this.mDoodadDefs);
                CreateOrUpdateObjChunk(Chunks.Mmid, this.mDoodadNameIds);
                CreateOrUpdateObjChunk(Chunks.Mmdx, this.mDoodadNames.SelectMany(s => Encoding.ASCII.GetBytes(s).Concat(new byte[] {0})).ToArray());

                foreach (var pair in this.mObjOrigChunks)
                {
                    writer.Write(pair.Value.Signature);
                    writer.Write(pair.Value.Size);
                    writer.Write(pair.Value.Data);
                }

                foreach (var chunk in this.mChunks.Where(chunk => chunk != null))
                {
                    chunk.WriteObjChunks(writer);
                }
            }
        }

        private void WriteTexFile()
        {
            using (var strm = FileManager.Instance.GetOutputStream(string.Format(@"World\Maps\{0}\{0}_{1}_{2}_tex0.adt", this.Continent, this.IndexX, this.IndexY)))
            {
                var writer = new BinaryWriter(strm);
                var texData = this.TextureNames.SelectMany(t => Encoding.ASCII.GetBytes(t).Concat(new byte[] {0})).ToArray();
                CreateOrUpdateTexChunk(Chunks.Mtex, texData);

                foreach (var pair in this.mTexOrigChunks)
                {
                    writer.Write(pair.Value.Signature);
                    writer.Write(pair.Value.Size);
                    writer.Write(pair.Value.Data);
                }

                foreach (var chunk in this.mChunks.Where(chunk => chunk != null))
                {
                    chunk.WriteTexChunks(writer);
                }
            }
        }

        private unsafe void CreateOrUpdateObjChunk<T>(uint signature, T[] values) where T : struct
        {
            var chunk = this.mObjOrigChunks.ContainsKey(signature) ? this.mObjOrigChunks[signature] : new DataChunk {Signature = signature};

            var totalSize = values.Length * SizeCache<T>.Size;
            chunk.Data = new byte[totalSize];
            chunk.Size = totalSize;

            var ptr = SizeCache<T>.GetUnsafePtr(ref values[0]);
            fixed (byte* bptr = chunk.Data)
            {
	            UnsafeNativeMethods.CopyMemory(bptr, (byte*) ptr, totalSize);
            }

	        if (this.mObjOrigChunks.ContainsKey(signature))
	        {
		        this.mObjOrigChunks[signature] = chunk;
	        }
	        else
	        {
		        this.mObjOrigChunks.Add(signature, chunk);
	        }
        }

        private unsafe void CreateOrUpdateTexChunk<T>(uint signature, T[] values) where T : struct
        {
            var chunk = this.mTexOrigChunks.ContainsKey(signature) ? this.mTexOrigChunks[signature] : new DataChunk { Signature = signature };

            var totalSize = values.Length * SizeCache<T>.Size;
            chunk.Data = new byte[totalSize];
            chunk.Size = totalSize;

            var ptr = SizeCache<T>.GetUnsafePtr(ref values[0]);
            fixed (byte* bptr = chunk.Data)
            {
	            UnsafeNativeMethods.CopyMemory(bptr, (byte*)ptr, totalSize);
            }

	        if (this.mTexOrigChunks.ContainsKey(signature))
	        {
		        this.mTexOrigChunks[signature] = chunk;
	        }
	        else
	        {
		        this.mTexOrigChunks.Add(signature, chunk);
	        }
        }

        private static bool SeekNextMcnk(BinaryReader reader) { return SeekChunk(reader, 0x4D434E4B, false); }

        private static bool SeekChunk(BinaryReader reader, uint signature, bool begin = true)
        {
	        if (begin)
	        {
		        reader.BaseStream.Position = 0;
	        }

            try
            {
                var sig = reader.ReadUInt32();
                while(sig != signature)
                {
                    var size = reader.ReadInt32();
                    reader.ReadBytes(size);
                    sig = reader.ReadUInt32();
                }

                return sig == signature;
            }
            catch (EndOfStreamException)
            {
                return false;
            }
        }

        public override void AddDoodadInstance(int uuid, string modelName, BoundingBox boundingBox, Vector3 position, Vector3 rotation, float scale)
        {
            //boundingBox.Maximum.Y = 64.0f * Metrics.TileSize - boundingBox.Maximum.Y;
            //boundingBox.Minimum.Y = 64.0f * Metrics.TileSize - boundingBox.Minimum.Y;
            //if (boundingBox.Maximum.Y < boundingBox.Minimum.Y)
            //{
            //    var tmp = boundingBox.Maximum;
            //    boundingBox.Maximum.Y = boundingBox.Minimum.Y;
            //    boundingBox.Minimum.Y = tmp.Y;
            //}

            var mmidValue = 0;
            var nameFound = false;
            foreach (var s in this.mDoodadNames)
            {
                if (string.Equals(s, modelName, StringComparison.InvariantCultureIgnoreCase))
                {
                    nameFound = true;
                    break;
                }

                mmidValue += s.Length + 1;
            }

            int mmidIndex;
            if (nameFound == false)
            {
                mmidValue = this.mDoodadNames.Sum(s => s.Length + 1);
                mmidIndex = this.mDoodadNameIds.Length;
                Array.Resize(ref this.mDoodadNameIds, this.mDoodadNameIds.Length + 1);
	            this.mDoodadNameIds[this.mDoodadNameIds.Length - 1] = mmidValue;
	            this.mDoodadNames.Add(modelName);
            }
            else
            {
                mmidIndex = -1;
                for (var i = 0; i < this.mDoodadNameIds.Length; ++i)
                {
                    if (this.mDoodadNameIds[i] == mmidValue)
                    {
                        mmidIndex = i;
                        break;
                    }
                }

                if (mmidIndex < 0)
                {
                    mmidIndex = this.mDoodadNameIds.Length;
                    Array.Resize(ref this.mDoodadNameIds, this.mDoodadNameIds.Length + 1);
	                this.mDoodadNameIds[this.mDoodadNameIds.Length - 1] = mmidValue;
                }
            }

            var mcrfValue = this.mDoodadDefs.Length;
            Array.Resize(ref this.mDoodadDefs, this.mDoodadDefs.Length + 1);
	        this.mDoodadDefs[this.mDoodadDefs.Length - 1] = new Mddf
            {
                Position = new Vector3(position.X, position.Z, 64.0f * Metrics.TileSize - position.Y),
                Mmid = mmidIndex,
                Flags = 0,
                Scale = (ushort)(scale * 1024),
                UniqueId = uuid,
                Rotation = new Vector3(360 - rotation.X, rotation.Z + 90, 360 - rotation.Y)
            };

            var instance = WorldFrame.Instance.M2Manager.AddInstance(modelName, uuid, position, rotation,
                new Vector3(scale));

	        this.DoodadInstances.Add(new M2Instance
            {
                Hash = modelName.ToUpperInvariant().GetHashCode(),
                Uuid = uuid,
                BoundingBox = (instance != null ? instance.BoundingBox : new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue))),
                RenderInstance = instance,
                MddfIndex = this.mDoodadDefs.Length - 1
            });

	        foreach (var chunk in this.mChunks)
	        {
		        chunk.TryAddDoodad(mcrfValue, boundingBox);
	        }

	        this.mWasChanged = true;
        }

        // ReSharper disable once FunctionComplexityOverflow
        protected override void Dispose(bool disposing)
        {
            if (this.mMainStream != null)
            {
	            this.mMainStream.Dispose();
	            this.mMainStream = null;
            }

            if (this.mTexStream != null)
            {
	            this.mTexStream.Dispose();
	            this.mTexStream = null;
            }

            if (this.mObjStream != null)
            {
	            this.mObjStream.Dispose();
	            this.mObjStream = null;
            }

            if (this.mChunks != null)
            {
	            foreach (var chunk in this.mChunks)
	            {
		            chunk.Dispose();
	            }

	            this.mChunks.Clear();
	            this.mChunks = null;
            }

            if (this.mWmoInstances != null)
            {
	            foreach (var instance in this.mWmoInstances)
	            {
		            WorldFrame.Instance.WmoManager.RemoveInstance(instance.FileName, instance.Uuid,false);
	            }

	            this.mWmoInstances.Clear();
	            this.mWmoInstances = null;
            }

            if (this.mTextureScales != null)
            {
	            this.mTextureScales.Clear();
	            this.mTextureScales = null;
            }

            if (this.mMainChunks != null)
            {
	            this.mMainChunks.Clear();
	            this.mMainChunks = null;
            }

            if (this.mTexChunks != null)
            {
	            this.mTexChunks.Clear();
	            this.mTexChunks = null;
            }

            if (this.mObjChunks != null)
            {
	            this.mObjChunks.Clear();
	            this.mObjChunks = null;
            }

            if (this.mBaseChunks != null)
            {
	            this.mBaseChunks.Clear();
	            this.mBaseChunks = null;
            }

            if (this.mObjOrigChunks != null)
            {
	            this.mObjOrigChunks.Clear();
	            this.mObjOrigChunks = null;
            }

            if (this.mTexOrigChunks != null)
            {
	            this.mTexOrigChunks.Clear();
	            this.mTexOrigChunks = null;
            }

	        this.mDoodadDefs = null;
            base.Dispose(disposing);
        }

        public override void SetChanged()
        {
	        this.mWasChanged = true;
        }
    }
}
