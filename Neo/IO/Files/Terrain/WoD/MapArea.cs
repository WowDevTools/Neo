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
            Continent = continent;
            IndexX = ix;
            IndexY = iy;
        }

        public override void Save()
        {
            if (mWasChanged == false)
                return;

            var hasMccv = mChunks.Any(c => c != null && c.HasMccv);
            if(hasMccv)
            {
                var wdt = WorldFrame.Instance.MapManager.CurrentWdt;
                if((wdt.Flags & 2) == 0)
                {
                    wdt.Flags |= 2;
                    wdt.Save(WorldFrame.Instance.MapManager.Continent);
                }
            }

            WriteBaseFile();
            WriteObjFile();
            WriteTexFile();
        }

        public override void OnUpdateModelPositions(TerrainChangeParameters parameters)
        {
            var center = new Vector2(parameters.Center.X, parameters.Center.Y);
            foreach (var inst in DoodadInstances)
            {
                if (inst == null || inst.RenderInstance == null)
                    continue;

                var pos = mDoodadDefs[inst.MddfIndex].Position;
                var old_pos = pos;
                var invZ = 64.0f * Metrics.TileSize - pos.Z;
                var dist = (new Vector2(pos.X, invZ) - center).Length;
                if (dist > parameters.OuterRadius)
                    continue;

                if (WorldFrame.Instance.MapManager.GetLandHeight(pos.X, pos.Z, out pos.Y))
                {
                    mDoodadDefs[inst.MddfIndex].Position = pos;
                    inst.RenderInstance.UpdatePosition(new Vector3(0, 0, pos.Y - old_pos.Y));
                }
            }
        }

        public override void UpdateNormals()
        {
            foreach (var chunk in mChunks)
            {
                if (chunk != null)
                    chunk.UpdateNormals();
            }
        }

        public override bool OnTextureTerrain(TextureChangeParameters parameters)
        {
            var changed = false;
            foreach (var chunk in mChunks)
            {
                if (chunk == null) continue;

                if (chunk.OnTextureTerrain(parameters))
                    changed = true;
            }

            return changed;
        }

        public void UpdateBoundingBox(BoundingBox chunkBox)
        {
            var minPos = chunkBox.Minimum;
            var maxPos = chunkBox.Maximum;

            var omin = BoundingBox.Minimum;
            var omax = BoundingBox.Maximum;

            omin.X = Math.Min(omin.X, minPos.X);
            omin.Y = Math.Min(omin.Y, minPos.Y);
            omin.Z = Math.Min(omin.Z, minPos.Z);
            omax.X = Math.Max(omax.X, maxPos.X);
            omax.Y = Math.Max(omax.Y, maxPos.Y);
            omax.Z = Math.Max(omax.Z, maxPos.Z);

            BoundingBox = new BoundingBox(omin, omax);
        }

        public void UpdateVertices(MapChunk chunk)
        {
            if (chunk == null)
                return;

            var ix = chunk.IndexX;
            var iy = chunk.IndexY;

            var index = (ix + iy * 16) * 145;
            for (var i = 0; i < 145; ++i)
                FullVertices[i + index] = chunk.Vertices[i];
        }

        public override bool OnChangeTerrain(TerrainChangeParameters parameters)
        {
            var changed = false;
            foreach (var chunk in mChunks)
            {
                if (chunk == null) continue;

                if (chunk.OnTerrainChange(parameters))
                    changed = true;
            }

            if (changed)
                mWasChanged = true;

            return changed;
        }

        public override bool Intersect(ref Ray ray, out Terrain.MapChunk chunk, out float distance)
        {
            distance = float.MaxValue;
            chunk = null;

            var mindistance = float.MaxValue;
            if (BoundingBox.Intersects(ref ray) == false)
                return false;

            Terrain.MapChunk chunkHit = null;
            var hasHit = false;
            foreach(var cnk in mChunks)
            {
                float dist;
                if (cnk.Intersect(ref ray, out dist) == false)
                    continue;

                hasHit = true;
                if (dist >= mindistance) continue;

                mindistance = dist;
                chunkHit = cnk;
            }

            chunk = chunkHit;
            distance = mindistance;
            return hasHit;
        }

        public float GetTextureScale(int index)
        {
            if (index >= mTextureScales.Count)
                throw new IndexOutOfRangeException();

            return mTextureScales[index];
        }

        public override Terrain.MapChunk GetChunk(int index)
        {
            if (index >= mChunks.Count)
                throw new IndexOutOfRangeException();

            return mChunks[index];
        }

        public override void AsyncLoad()
        {
            try
            {
                mMainStream =
                    FileManager.Instance.Provider.OpenFile(string.Format(@"World\Maps\{0}\{0}_{1}_{2}.adt", Continent,
                        IndexX, IndexY));

                mTexStream = FileManager.Instance.Provider.OpenFile(string.Format(@"World\Maps\{0}\{0}_{1}_{2}_tex0.adt", Continent,
                        IndexX, IndexY));

                mObjStream = FileManager.Instance.Provider.OpenFile(string.Format(@"World\Maps\{0}\{0}_{1}_{2}_obj0.adt", Continent,
                        IndexX, IndexY));

                if (mMainStream == null || mTexStream == null || mObjStream == null)
                {
                    IsValid = false;
                    return;
                }

                mReader = new BinaryReader(mMainStream);
                mTexReader = new BinaryReader(mTexStream);
                mObjReader = new BinaryReader(mObjStream);

                InitChunkInfos();

                mTexStream.Position = 0;
                InitTextureNames();

                mObjStream.Position = 0;
                InitModels();

                InitChunks();

                BuildDataForSave();
            }
            catch(Exception e)
            {
                Log.Error(e.ToString());
                Log.Warning(string.Format("Attempted to load ADT {0}_{1}_{2}.adt but it caused an error: {3}. Skipping the adt.", Continent, IndexX, IndexY, e.Message));
                IsValid = false;
                return;
            }

            IsValid = true;
        }

        private void BuildDataForSave()
        {
            mReader.BaseStream.Position = 0;
            while(mReader.BaseStream.Position + 8 < mReader.BaseStream.Length)
            {
                var signature = mReader.ReadUInt32();
                var size = mReader.ReadInt32();
                // skip the MCNK, chunks will write it
                if(signature == 0x4D434E4B)
                {
                    mReader.BaseStream.Position += size;
                    continue;
                }

                var data = mReader.ReadBytes(size);
                mBaseChunks.Add(signature, new DataChunk
                {
                    Data = data,
                    Signature = signature,
                    Size = size
                });
            }

            mObjReader.BaseStream.Position = 0;
            while (mObjReader.BaseStream.Position + 8 < mObjReader.BaseStream.Length)
            {
                var signature = mObjReader.ReadUInt32();
                var size = mObjReader.ReadInt32();
                if (signature == 0x4D434E4B)
                {
                    mObjReader.BaseStream.Position += size;
                    continue;
                }

                var data = mObjReader.ReadBytes(size);
                mObjOrigChunks.Add(signature, new DataChunk
                {
                    Data = data,
                    Signature = signature,
                    Size = size
                });
            }

            mTexReader.BaseStream.Position = 0;
            while (mTexReader.BaseStream.Position + 8 < mTexReader.BaseStream.Length)
            {
                var signature = mTexReader.ReadUInt32();
                var size = mTexReader.ReadInt32();
                if (signature == 0x4D434E4B)
                {
                    mTexReader.BaseStream.Position += size;
                    continue;
                }

                var data = mTexReader.ReadBytes(size);
                mTexOrigChunks.Add(signature, new DataChunk
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
                if (SeekNextMcnk(mReader) == false)
                    throw new InvalidOperationException("Unable to read MCNK from ADT");

                if (SeekNextMcnk(mTexReader) == false)
                    throw new InvalidOperationException("Unable to read MCNK from TEX ADT");

                if (SeekNextMcnk(mObjReader) == false)
                    throw new InvalidOperationException("Unable to read MCNK from obj ADT");

                mMainChunks.Add(new ChunkStreamInfo
                {
                    PosStart = (int) mMainStream.Position,
                    Size = mReader.ReadInt32(),
                    Stream = mReader
                });

                mTexChunks.Add(new ChunkStreamInfo
                {
                    PosStart = (int)mTexStream.Position,
                    Size = mTexReader.ReadInt32(),
                    Stream = mTexReader
                });

                mObjChunks.Add(new ChunkStreamInfo
                {
                    PosStart = (int)mObjStream.Position,
                    Size = mObjReader.ReadInt32(),
                    Stream = mObjReader
                });

                mReader.ReadBytes(mMainChunks.Last().Size);
                mTexReader.ReadBytes(mTexChunks.Last().Size);
                mObjReader.ReadBytes(mObjChunks.Last().Size);
            }
        }

        private void InitModels()
        {
            InitWmoModels();
            InitM2Models();
        }

        private void InitM2Models()
        {
            if (SeekChunk(mObjReader, Chunks.Mmdx) == false)
                return;

            var size = mObjReader.ReadInt32();
            var bytes = mObjReader.ReadBytes(size);
            var fullString = Encoding.ASCII.GetString(bytes);
            var modelNames = fullString.Split('\0');
            mDoodadNames.AddRange(modelNames.ToList());
            var modelNameLookup = new Dictionary<int, string>();
            var curOffset = 0;
            foreach (var name in modelNames)
            {
                modelNameLookup.Add(curOffset, name);
                curOffset += name.Length + 1;
            }

            if (SeekChunk(mObjReader, Chunks.Mmid) == false)
                return;

            size = mObjReader.ReadInt32();
            mDoodadNameIds = mObjReader.ReadArray<int>(size / 4);

            if (SeekChunk(mObjReader, Chunks.Mddf) == false)
                return;

            size = mObjReader.ReadInt32();
            mDoodadDefs = mObjReader.ReadArray<Mddf>(size / SizeCache<Mddf>.Size);

            var index = -1;
            foreach (var entry in mDoodadDefs)
            {
                ++index;
                if (entry.Mmid >= mDoodadNameIds.Length)
                    continue;

                var nameId = mDoodadNameIds[entry.Mmid];
                string modelName;
                if (modelNameLookup.TryGetValue(nameId, out modelName) == false)
                    continue;

                var position = new Vector3(entry.Position.X, 64.0f * Metrics.TileSize - entry.Position.Z,
                    entry.Position.Y);
                var rotation = new Vector3(360.0f - entry.Rotation.X, 360.0f - entry.Rotation.Z, entry.Rotation.Y - 90);
                var scale = entry.Scale / 1024.0f;

                var instance = WorldFrame.Instance.M2Manager.AddInstance(modelName, entry.UniqueId, position, rotation,
                    new Vector3(scale));

                DoodadInstances.Add(new M2Instance
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
            if (SeekChunk(mObjReader, Chunks.Mwmo) == false)
                return;

            var size = mObjReader.ReadInt32();
            var bytes = mObjReader.ReadBytes(size);
            var modelNameLookup = new Dictionary<int, string>();
            var curOffset = 0;
            var curBytes = new List<byte>();

            for(var i = 0; i < bytes.Length; ++i)
            {
                if (bytes[i] == 0)
                {
                    if (curBytes.Count > 0)
                        modelNameLookup.Add(curOffset, Encoding.ASCII.GetString(curBytes.ToArray()));

                    curOffset = i + 1;
                    curBytes.Clear();
                }
                else
                    curBytes.Add(bytes[i]);
            }

            if (SeekChunk(mObjReader, Chunks.Mwid) == false)
                return;

            size = mObjReader.ReadInt32();
            var modelNameIds = mObjReader.ReadArray<int>(size / 4);

            if (SeekChunk(mObjReader, Chunks.Modf) == false)
                return;

            size = mObjReader.ReadInt32();
            var modf = mObjReader.ReadArray<Modf>(size / SizeCache<Modf>.Size);

            foreach(var entry in modf)
            {
                if (entry.Mwid >= modelNameIds.Length)
                    continue;

                var nameId = modelNameIds[entry.Mwid];
                string modelName;
                if (modelNameLookup.TryGetValue(nameId, out modelName) == false)
                    continue;

                var position = new Vector3(entry.Position.X, 64.0f * Metrics.TileSize - entry.Position.Z,
                    entry.Position.Y);
                var rotation = new Vector3(360.0f - entry.Rotation.X, 360.0f - entry.Rotation.Z, entry.Rotation.Y - 90);

                WorldFrame.Instance.WmoManager.AddInstance(modelName, entry.UniqueId, position, rotation);
                mWmoInstances.Add(new LoadedModel(modelName, entry.UniqueId));
            }
        }

        private void InitTextureNames()
        {
            if (SeekChunk(mTexReader, Chunks.Mtex) == false)
                return;

            var size = mTexReader.ReadInt32();
            var bytes = mTexReader.ReadBytes(size);
            var fullString = Encoding.ASCII.GetString(bytes);
            TextureNames.AddRange(fullString.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries));
            TextureNames.ForEach(t => mTextures.Add(TextureManager.Instance.GetTexture(t)));

            TextureNames.ForEach(t =>
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
                    mTextureScales.Add(1.0f);
                else
                    mTextureScales.Add(256.0f / (2 * loadInfo.Width));
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
                var chunk = new MapChunk(mMainChunks[i], mTexChunks[i], mObjChunks[i], i % 16, i / 16, this);
                chunk.AsyncLoad();
                var bbmin = chunk.BoundingBox.Minimum;
                var bbmax = chunk.BoundingBox.Maximum;
                if (bbmin.X < minPos.X)
                    minPos.X = bbmin.X;
                if (bbmax.X > maxPos.X)
                    maxPos.X = bbmax.X;
                if (bbmin.Y < minPos.Y)
                    minPos.Y = bbmin.Y;
                if (bbmax.Y > maxPos.Y)
                    maxPos.Y = bbmax.Y;
                if (bbmin.Z < minPos.Z)
                    minPos.Z = bbmin.Z;
                if (bbmax.Z > maxPos.Z)
                    maxPos.Z = bbmax.Z;

                bbmin = chunk.ModelBox.Minimum;
                bbmax = chunk.ModelBox.Maximum;
                if (bbmin.X < modelMin.X)
                    modelMin.X = bbmin.X;
                if (bbmax.X > modelMax.X)
                    modelMax.X = bbmax.X;
                if (bbmin.Y < modelMin.Y)
                    modelMin.Y = bbmin.Y;
                if (bbmax.Y > modelMax.Y)
                    modelMax.Y = bbmax.Y;
                if (bbmin.Z < modelMin.Z)
                    modelMin.Z = bbmin.Z;
                if (bbmax.Z > modelMax.Z)
                    modelMax.Z = bbmax.Z;

                mChunks.Add(chunk);
                Array.Copy(chunk.Vertices, 0, FullVertices, i * 145, 145);
            }

            BoundingBox = new BoundingBox(minPos, maxPos);
            ModelBox = new BoundingBox(modelMin, modelMax);
        }

        private void WriteBaseFile()
        {
            using (var strm = FileManager.Instance.GetOutputStream(string.Format(@"World\Maps\{0}\{0}_{1}_{2}.adt", Continent, IndexX, IndexY)))
            {
                var writer = new BinaryWriter(strm);
                foreach (var pair in mBaseChunks)
                {
                    writer.Write(pair.Value.Signature);
                    writer.Write(pair.Value.Size);
                    writer.Write(pair.Value.Data);
                }

                foreach (var chunk in mChunks)
                {
                    if (chunk == null) continue;

                    chunk.WriteBaseChunks(writer);
                }
            }
        }

        private void WriteObjFile()
        {
            using (var strm = FileManager.Instance.GetOutputStream(string.Format(@"World\Maps\{0}\{0}_{1}_{2}_obj0.adt", Continent, IndexX, IndexY)))
            {
                var writer = new BinaryWriter(strm);
                CreateOrUpdateObjChunk(Chunks.Mddf, mDoodadDefs);
                CreateOrUpdateObjChunk(Chunks.Mmid, mDoodadNameIds);
                CreateOrUpdateObjChunk(Chunks.Mmdx, mDoodadNames.SelectMany(s => Encoding.ASCII.GetBytes(s).Concat(new byte[] {0})).ToArray());

                foreach (var pair in mObjOrigChunks)
                {
                    writer.Write(pair.Value.Signature);
                    writer.Write(pair.Value.Size);
                    writer.Write(pair.Value.Data);
                }

                foreach (var chunk in mChunks.Where(chunk => chunk != null))
                {
                    chunk.WriteObjChunks(writer);
                }
            }
        }

        private void WriteTexFile()
        {
            using (var strm = FileManager.Instance.GetOutputStream(string.Format(@"World\Maps\{0}\{0}_{1}_{2}_tex0.adt", Continent, IndexX, IndexY)))
            {
                var writer = new BinaryWriter(strm);
                var texData = TextureNames.SelectMany(t => Encoding.ASCII.GetBytes(t).Concat(new byte[] {0})).ToArray();
                CreateOrUpdateTexChunk(Chunks.Mtex, texData);

                foreach (var pair in mTexOrigChunks)
                {
                    writer.Write(pair.Value.Signature);
                    writer.Write(pair.Value.Size);
                    writer.Write(pair.Value.Data);
                }

                foreach (var chunk in mChunks.Where(chunk => chunk != null))
                {
                    chunk.WriteTexChunks(writer);
                }
            }
        }

        private unsafe void CreateOrUpdateObjChunk<T>(uint signature, T[] values) where T : struct
        {
            var chunk = mObjOrigChunks.ContainsKey(signature) ? mObjOrigChunks[signature] : new DataChunk {Signature = signature};

            var totalSize = values.Length * SizeCache<T>.Size;
            chunk.Data = new byte[totalSize];
            chunk.Size = totalSize;

            var ptr = SizeCache<T>.GetUnsafePtr(ref values[0]);
            fixed (byte* bptr = chunk.Data)
                UnsafeNativeMethods.CopyMemory(bptr, (byte*) ptr, totalSize);

            if (mObjOrigChunks.ContainsKey(signature))
                mObjOrigChunks[signature] = chunk;
            else
                mObjOrigChunks.Add(signature, chunk);
        }

        private unsafe void CreateOrUpdateTexChunk<T>(uint signature, T[] values) where T : struct
        {
            var chunk = mTexOrigChunks.ContainsKey(signature) ? mTexOrigChunks[signature] : new DataChunk { Signature = signature };

            var totalSize = values.Length * SizeCache<T>.Size;
            chunk.Data = new byte[totalSize];
            chunk.Size = totalSize;

            var ptr = SizeCache<T>.GetUnsafePtr(ref values[0]);
            fixed (byte* bptr = chunk.Data)
                UnsafeNativeMethods.CopyMemory(bptr, (byte*)ptr, totalSize);

            if (mTexOrigChunks.ContainsKey(signature))
                mTexOrigChunks[signature] = chunk;
            else
                mTexOrigChunks.Add(signature, chunk);
        }

        private static bool SeekNextMcnk(BinaryReader reader) { return SeekChunk(reader, 0x4D434E4B, false); }

        private static bool SeekChunk(BinaryReader reader, uint signature, bool begin = true)
        {
            if (begin)
                reader.BaseStream.Position = 0;

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
            foreach (var s in mDoodadNames)
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
                mmidValue = mDoodadNames.Sum(s => s.Length + 1);
                mmidIndex = mDoodadNameIds.Length;
                Array.Resize(ref mDoodadNameIds, mDoodadNameIds.Length + 1);
                mDoodadNameIds[mDoodadNameIds.Length - 1] = mmidValue;
                mDoodadNames.Add(modelName);
            }
            else
            {
                mmidIndex = -1;
                for (var i = 0; i < mDoodadNameIds.Length; ++i)
                {
                    if (mDoodadNameIds[i] == mmidValue)
                    {
                        mmidIndex = i;
                        break;
                    }
                }

                if (mmidIndex < 0)
                {
                    mmidIndex = mDoodadNameIds.Length;
                    Array.Resize(ref mDoodadNameIds, mDoodadNameIds.Length + 1);
                    mDoodadNameIds[mDoodadNameIds.Length - 1] = mmidValue;
                }
            }

            var mcrfValue = mDoodadDefs.Length;
            Array.Resize(ref mDoodadDefs, mDoodadDefs.Length + 1);
            mDoodadDefs[mDoodadDefs.Length - 1] = new Mddf
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

            DoodadInstances.Add(new M2Instance
            {
                Hash = modelName.ToUpperInvariant().GetHashCode(),
                Uuid = uuid,
                BoundingBox = (instance != null ? instance.BoundingBox : new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue))),
                RenderInstance = instance,
                MddfIndex = mDoodadDefs.Length - 1
            });

	        foreach (var chunk in mChunks)
	        {
		        chunk.TryAddDoodad(mcrfValue, boundingBox);
	        }

	        mWasChanged = true;
        }

        // ReSharper disable once FunctionComplexityOverflow
        protected override void Dispose(bool disposing)
        {
            if (mMainStream != null)
            {
                mMainStream.Dispose();
                mMainStream = null;
            }

            if (mTexStream != null)
            {
                mTexStream.Dispose();
                mTexStream = null;
            }

            if (mObjStream != null)
            {
                mObjStream.Dispose();
                mObjStream = null;
            }

            if (mChunks != null)
            {
                foreach (var chunk in mChunks)
                    chunk.Dispose();

                mChunks.Clear();
                mChunks = null;
            }

            if (mWmoInstances != null)
            {
                foreach (var instance in mWmoInstances)
                    WorldFrame.Instance.WmoManager.RemoveInstance(instance.FileName, instance.Uuid,false);

                mWmoInstances.Clear();
                mWmoInstances = null;
            }

            if (mTextureScales != null)
            {
                mTextureScales.Clear();
                mTextureScales = null;
            }

            if (mMainChunks != null)
            {
                mMainChunks.Clear();
                mMainChunks = null;
            }

            if (mTexChunks != null)
            {
                mTexChunks.Clear();
                mTexChunks = null;
            }

            if (mObjChunks != null)
            {
                mObjChunks.Clear();
                mObjChunks = null;
            }

            if (mBaseChunks != null)
            {
                mBaseChunks.Clear();
                mBaseChunks = null;
            }

            if (mObjOrigChunks != null)
            {
                mObjOrigChunks.Clear();
                mObjOrigChunks = null;
            }

            if (mTexOrigChunks != null)
            {
                mTexOrigChunks.Clear();
                mTexOrigChunks = null;
            }

            mDoodadDefs = null;
            base.Dispose(disposing);
        }
    }
}
