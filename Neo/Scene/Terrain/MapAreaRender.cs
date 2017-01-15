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
	        this.IndexX = indexX;
	        this.IndexY = indexY;
        }

        public void OnTextureChange(Editing.TextureChangeParameters parameters)
        {
            if (this.mAsyncLoaded == false || this.AreaFile.IsValid == false || this.mSyncLoaded == false)
            {
	            return;
            }

	        this.AreaFile.OnTextureTerrain(parameters);
        }

        public void OnTerrainChange(Editing.TerrainChangeParameters parameters)
        {
            if (this.mAsyncLoaded == false || this.AreaFile.IsValid == false || this.mSyncLoaded == false)
            {
	            return;
            }

	        this.mIsDirty = this.AreaFile.OnChangeTerrain(parameters);
            if (!this.mIsDirty)
            {
	            return;
            }

	        this.mBoundingBox = this.AreaFile.BoundingBox;
            foreach (var chunk in this.mChunks)
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
            if (this.mAsyncLoaded == false || this.AreaFile.IsValid == false || this.mSyncLoaded == false)
            {
	            return;
            }

	        this.AreaFile.OnUpdateModelPositions(parameters);
        }

        public void OnFrame()
        {
            if (this.mAsyncLoaded == false)
            {
	            return;
            }

	        if (this.AreaFile.IsValid == false)
	        {
		        return;
	        }

	        // INVESTIGATE: Possible performance issue
	        if(this.mSyncLoaded == false)
            {
	            this.mVertexBuffer = new VertexBuffer();
	            this.mVertexBuffer.BufferData(this.AreaFile.FullVertices);
	            this.mSyncLoaded = true;
            }

            if(WorldFrame.Instance.MapManager.IsInitialLoad == false)
            {
                if (WorldFrame.Instance.MapManager.SkySphere.BoundingSphere.Intersects(ref this.mBoundingBox) == false)
                {
                    // step 1: reject the area if we dont have to update m2 models if its not in the sky sphere
                    if (!M2Manager.IsViewDirty || WorldFrame.Instance.State != AppState.World)
                    {
	                    return;
                    }

	                // step 2: if models are supposed to be updated and the model box is not contained in the sky sphere
                    // it has to be rejected as well as there is no way it could ever contribute
                    if (WorldFrame.Instance.MapManager.SkySphere.BoundingSphere.Intersects(ref this.mModelBox) == false)
                    {
	                    return;
                    }
                }

                if (WorldFrame.Instance.ActiveCamera.Contains(ref this.mBoundingBox) == false)
                {
	                if (!M2Manager.IsViewDirty || WorldFrame.Instance.State != AppState.World)
	                {
		                return;
	                }

	                if (!WorldFrame.Instance.ActiveCamera.Contains(ref this.mModelBox))
	                {
		                return;
	                }

	                // INVESTIGATE: Possible early return bug
	                foreach (var chunk in this.mChunks)
	                {
		                chunk.PushDoodadReferences();
	                }

                    return;
                }
            }

	        // INVESTIGATE: Possible performance issue
	        if (this.mIsDirty)
            {
	            this.AreaFile.UpdateNormals();
	            this.mVertexBuffer.BufferData(this.AreaFile.FullVertices);
	            this.mIsDirty = false;
            }

            MapChunkRender.ChunkMesh.UpdateVertexBuffer(this.mVertexBuffer);

            foreach (var chunk in this.mChunks)
            {
	            chunk.OnFrame();
            }
        }

        public void AsyncLoaded(IO.Files.Terrain.MapArea area)
        {
	        this.AreaFile = area;
            if (this.AreaFile.IsValid == false)
            {
	            return;
            }

	        for(var i = 0; i < 256; ++i)
            {
                var chunk = new MapChunkRender();
                chunk.OnAsyncLoad(area.GetChunk(i), this);
	            this.mChunks[i] = chunk;
            }

	        this.mBoundingBox = area.BoundingBox;
	        this.mModelBox = area.ModelBox;
	        this.mAsyncLoaded = true;
        }

        ~MapAreaRender()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (this.mChunks != null)
            {
                for (var i = 0; i < 256; ++i)
                {
                    if (this.mChunks[i] == null)
                    {
	                    continue;
                    }

	                this.mChunks[i].Dispose();
	                this.mChunks[i] = null;
                }

	            this.mChunks = null;
            }

            if (this.AreaFile != null)
            {
	            this.AreaFile.Dispose();
	            this.AreaFile = null;
            }

	        this.mAsyncLoaded = false;

            if (this.mVertexBuffer != null)
            {
                var vertexBuffer = this.mVertexBuffer;
                WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
                {
                    if (vertexBuffer != null)
                    {
	                    vertexBuffer.Dispose();
                    }
                });

	            this.mVertexBuffer = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
