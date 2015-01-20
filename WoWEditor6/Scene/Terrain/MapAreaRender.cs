using WoWEditor6.Graphics;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace WoWEditor6.Scene.Terrain
{
    class MapAreaRender
    {
        private bool mAsyncLoaded;
        private bool mSyncLoaded;

        private VertexBuffer mVertexBuffer;
        private readonly MapChunkRender[] mChunks = new MapChunkRender[256];

        public int IndexX { get; private set; }
        public int IndexY { get; private set; }

        public IO.Files.Terrain.MapArea AreaFile { get; private set; }

        public MapAreaRender(int indexX, int indexY)
        {
            IndexX = indexX;
            IndexY = indexY;
        }

        public void OnFrame()
        {
            if (mAsyncLoaded == false)
                return;

            if(mSyncLoaded == false)
            {
                mVertexBuffer = new VertexBuffer(WorldFrame.Instance.GraphicsContext);
                mVertexBuffer.UpdateData(AreaFile.FullVertices);
                mSyncLoaded = true;
            }

            MapChunkRender.ChunkMesh.UpdateVertexBuffer(mVertexBuffer);

            foreach (var chunk in mChunks)
                chunk.OnFrame();
        }

        public void AsyncLoaded(IO.Files.Terrain.MapArea area)
        {
            AreaFile = area;
            for(var i = 0; i < 256; ++i)
            {
                var chunk = new MapChunkRender();
                chunk.OnAsyncLoad(area.GetChunk(i));
                mChunks[i] = chunk;
            }


            mAsyncLoaded = true;
        }
    }
}
