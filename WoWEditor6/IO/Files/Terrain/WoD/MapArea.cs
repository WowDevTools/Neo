using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpDX;
using WoWEditor6.Scene.Texture;

namespace WoWEditor6.IO.Files.Terrain.WoD
{
    class ChunkStreamInfo
    {
        public BinaryReader Stream;
        public int PosStart;
        public int Size;
    }

    class MapArea : IDisposable
    {
        private Stream mMainStream;
        private Stream mTexStream;
        private Stream mObjStream;

        private BinaryReader mReader;
        private BinaryReader mTexReader;
        private BinaryReader mObjReader;

        private readonly List<string> mTextureNames = new List<string>();
        private readonly List<Graphics.Texture> mTextures = new List<Graphics.Texture>();

        private readonly List<ChunkStreamInfo> mMainChunks = new List<ChunkStreamInfo>();
        private readonly List<ChunkStreamInfo> mTexChunks = new List<ChunkStreamInfo>();
        private readonly List<ChunkStreamInfo> mObjChunks = new List<ChunkStreamInfo>();

        private readonly List<MapChunk> mChunks = new List<MapChunk>();

        public int IndexX { get; }
        public int IndexY { get; }
        public string Continent { get; }

        public AdtVertex[] FullVertices { get; } = new AdtVertex[145 * 256];

        public BoundingBox BoundingBox { get; private set; }

        public MapArea(string continent, int ix, int iy)
        {
            Continent = continent;
            IndexX = ix;
            IndexY = iy;
        }

        public Graphics.Texture GetTexture(int index)
        {
            if (index >= mTextures.Count)
                throw new IndexOutOfRangeException();

            return mTextures[index];
        }

        public MapChunk GetChunk(int index)
        {
            if (index >= mChunks.Count)
                throw new IndexOutOfRangeException();

            return mChunks[index];
        }

        public void AsyncLoad()
        {
            mMainStream =
                FileManager.Instance.Provider.OpenFile(string.Format(@"World\Maps\{0}\{0}_{1:D2}_{2:D2}.adt", Continent,
                    IndexX, IndexY));

            mTexStream = FileManager.Instance.Provider.OpenFile(string.Format(@"World\Maps\{0}\{0}_{1:D2}_{2:D2}_tex0.adt", Continent,
                    IndexX, IndexY));

            mObjStream = FileManager.Instance.Provider.OpenFile(string.Format(@"World\Maps\{0}\{0}_{1:D2}_{2:D2}_obj0.adt", Continent,
                    IndexX, IndexY));

            mReader = new BinaryReader(mMainStream);
            mTexReader = new BinaryReader(mTexStream);
            mObjReader = new BinaryReader(mObjStream);

            InitChunkInfos();

            mTexStream.Position = 0;
            InitTextureNames();
            InitChunks();
        }

        private void InitChunkInfos()
        {
            for(var i = 0; i < 256; ++i)
            {
                if (SeekNextMcnk(mReader) == false)
                    throw new InvalidOperationException("Unable to read MCNK from adt");

                if (SeekNextMcnk(mTexReader) == false)
                    throw new InvalidOperationException("Unable to read MCNK from tex adt");

                if (SeekNextMcnk(mObjReader) == false)
                    throw new InvalidOperationException("Unable to read MCNK from obj adt");

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

        private void InitTextureNames()
        {
            if (SeekChunk(mTexReader, 0x4D544558) == false)
                return;

            var size = mTexReader.ReadInt32();
            var bytes = mTexReader.ReadBytes(size);
            var fullString = Encoding.ASCII.GetString(bytes);
            mTextureNames.AddRange(fullString.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries));
            mTextureNames.ForEach(t => mTextures.Add(TextureManager.Instance.GetTexture(t)));
        }

        private void InitChunks()
        {
            var minHeight = float.MaxValue;
            var maxHeight = float.MinValue;
            for (var i = 0; i < 256; ++i)
            {
                var chunk = new MapChunk(mMainChunks[i], mTexChunks[i], mObjChunks[i], i % 16, i / 16, this);
                chunk.AsyncLoad();
                if (chunk.BoundingBox.Minimum.Z > minHeight)
                    minHeight = chunk.BoundingBox.Minimum.Z;
                if (chunk.BoundingBox.Maximum.Z > maxHeight)
                    maxHeight = chunk.BoundingBox.Maximum.Z;

                mChunks.Add(chunk);
                Array.Copy(chunk.Vertices, 0, FullVertices, i * 145, 145);
            }

            BoundingBox = new BoundingBox(new Vector3(IndexX * Metrics.TileSize, IndexY * Metrics.TileSize, minHeight),
                new Vector3((IndexX + 1) * Metrics.TileSize, (IndexY + 1) * Metrics.TileSize, maxHeight));
        }

        private static bool SeekNextMcnk(BinaryReader reader) => SeekChunk(reader, 0x4D434E4B);

        private static bool SeekChunk(BinaryReader reader, uint signature)
        {
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

        public void Dispose()
        {
            mMainStream?.Dispose();
            mTexStream?.Dispose();
            mObjStream?.Dispose();
        }
    }
}
