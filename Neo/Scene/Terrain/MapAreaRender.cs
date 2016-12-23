using System;
using Neo.Graphics;
using Neo.Scene.Models;
using SlimTK;

namespace Neo.Scene.Terrain
{
	internal class MapAreaRender : IDisposable
    {
        private bool mAsyncLoaded;
        private bool mSyncLoaded;

        private VertexBuffer mVertexBuffer;
        private MapChunkRender[] mChunks = new MapChunkRender[256];
        private BoundingBox mBoundingBox;
        private BoundingBox mModelBox;
        private bool mIsDirty;

        public int IndexX { get; private set; }
        public int IndexY { get; private set; }

        public IO.Files.Terrain.MapArea AreaFile { get; private set; }

        public MapAreaRender(int indexX, int indexY)
        {
            IndexX = indexX;
            IndexY = indexY;
        }

        public void OnTextureChange(Editing.TextureChangeParameters parameters)
        {
            if (mAsyncLoaded == false || AreaFile.IsValid == false || mSyncLoaded == false)
            {
	            return;
            }

	        AreaFile.OnTextureTerrain(parameters);
        }

        public void OnTerrainChange(Editing.TerrainChangeParameters parameters)
        {
            if (mAsyncLoaded == false || AreaFile.IsValid == false || mSyncLoaded == false)
            {
	            return;
            }

	        mIsDirty = AreaFile.OnChangeTerrain(parameters);
            if (!mIsDirty)
            {
	            return;
            }

	        mBoundingBox = AreaFile.BoundingBox;
            foreach (var chunk in mChunks)
            {
                if (chunk == null)
                {
	                continue;
                }

	            chunk.UpdateBoundingBox();
            }
        }

        public void OnUpdateModelPositions(Editing.TerrainChangeParameters parameters)
        {
            if (mAsyncLoaded == false || AreaFile.IsValid == false || mSyncLoaded == false)
            {
	            return;
            }

	        AreaFile.OnUpdateModelPositions(parameters);
        }

        public void OnFrame()
        {
            if (mAsyncLoaded == false)
            {
	            return;
            }

	        if (AreaFile.IsValid == false)
	        {
		        return;
	        }

	        // INVESTIGATE: Possible performance issue
	        if(mSyncLoaded == false)
            {
                mVertexBuffer = new VertexBuffer();
                mVertexBuffer.BufferData(AreaFile.FullVertices);
                mSyncLoaded = true;
            }

            if(WorldFrame.Instance.MapManager.IsInitialLoad == false)
            {
                if (WorldFrame.Instance.MapManager.SkySphere.BoundingSphere.Intersects(ref mBoundingBox) == false)
                {
                    // step 1: reject the area if we dont have to update m2 models if its not in the sky sphere
                    if (!M2Manager.IsViewDirty || WorldFrame.Instance.State != AppState.World)
                    {
	                    return;
                    }

	                // step 2: if models are supposed to be updated and the model box is not contained in the sky sphere
                    // it has to be rejected as well as there is no way it could ever contribute
                    if (WorldFrame.Instance.MapManager.SkySphere.BoundingSphere.Intersects(ref mModelBox) == false)
                    {
	                    return;
                    }
                }

                if (WorldFrame.Instance.ActiveCamera.Contains(ref mBoundingBox) == false)
                {
	                if (!M2Manager.IsViewDirty || WorldFrame.Instance.State != AppState.World)
	                {
		                return;
	                }

	                if (!WorldFrame.Instance.ActiveCamera.Contains(ref mModelBox))
	                {
		                return;
	                }

	                // INVESTIGATE: Possible early return bug
	                foreach (var chunk in mChunks)
	                {
		                chunk.PushDoodadReferences();
	                }

                    return;
                }
            }

	        // INVESTIGATE: Possible performance issue
	        if (mIsDirty)
            {
                AreaFile.UpdateNormals();
                mVertexBuffer.BufferData(AreaFile.FullVertices);
                mIsDirty = false;
            }

            MapChunkRender.ChunkMesh.UpdateVertexBuffer(mVertexBuffer);

            foreach (var chunk in mChunks)
            {
	            chunk.OnFrame();
            }
        }

        public void AsyncLoaded(IO.Files.Terrain.MapArea area)
        {
            AreaFile = area;
            if (AreaFile.IsValid == false)
            {
	            return;
            }

	        for(var i = 0; i < 256; ++i)
            {
                var chunk = new MapChunkRender();
                chunk.OnAsyncLoad(area.GetChunk(i), this);
                mChunks[i] = chunk;
            }

            mBoundingBox = area.BoundingBox;
            mModelBox = area.ModelBox;
            mAsyncLoaded = true;
        }

        ~MapAreaRender()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (mChunks != null)
            {
                for (var i = 0; i < 256; ++i)
                {
                    if (mChunks[i] == null)
                    {
	                    continue;
                    }

	                mChunks[i].Dispose();
                    mChunks[i] = null;
                }

                mChunks = null;
            }

            if (AreaFile != null)
            {
                AreaFile.Dispose();
                AreaFile = null;
            }

            mAsyncLoaded = false;

            if (mVertexBuffer != null)
            {
                var vertexBuffer = mVertexBuffer;
                WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
                {
                    if (vertexBuffer != null)
                    {
	                    vertexBuffer.Dispose();
                    }
                });

                mVertexBuffer = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
