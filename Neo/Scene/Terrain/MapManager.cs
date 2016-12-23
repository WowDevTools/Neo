using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Neo.IO;
using Neo.IO.Files.Sky;
using Neo.IO.Files.Terrain;
using Neo.UI;
using OpenTK;
using SlimTK;
using Warcraft.WDL;
using Warcraft.WDT;
using Warcraft.WDT.Chunks;

namespace Neo.Scene.Terrain
{
	internal class MapManager
    {
        private const int MapRadius = 2;

        private Vector2 mEntryPoint;
        private int mTotalLoadSteps;
        private int mLoadStepsDone;
        private readonly List<MapArea> mDataToLoad = new List<MapArea>();
        private readonly List<MapArea> mLoadedData = new List<MapArea>();
        private readonly List<MapAreaRender> mUnloadList = new List<MapAreaRender>();
        private readonly Dictionary<int, MapAreaRender> mAreas = new Dictionary<int, MapAreaRender>();
        private Thread mLoadThread;
        private Thread mUnloadThread;
        private Thread mLightUpdateThread;
        private bool mIsRunning;
        private readonly MapLowManager mAreaLowManager = new MapLowManager();
        private readonly List<int> mCurrentValidLinks = new List<int>();

        public string Continent { get; private set; }
        public WorldTable CurrentWdt { get; private set; }
        public bool HasNewBlend { get; private set; }
        public bool IsInitialLoad { get; private set; }
        public SkySphere SkySphere { get; private set; }
        public bool HideTerrain { get; set; } = false;

        public void Initialize()
        {
	        this.SkySphere = new SkySphere(999.0f, 25, 25);
	        this.mIsRunning = true;
	        this.mLoadThread = new Thread(LoadProc);
	        this.mLoadThread.Start();
	        this.mLightUpdateThread = new Thread(LightUpdateProc);
	        this.mLightUpdateThread.Start();
	        this.mUnloadThread = new Thread(UnloadProc);
	        this.mUnloadThread.Start();
        }

        public MapAreaRender GetAreaByIndex(int ix, int iy)
        {
            var index = iy * 0xFF + ix;
            lock (this.mAreas)
            {
	            return this.mAreas.ContainsKey(index) ? this.mAreas[index] : null;
            }
        }

        public MapAreaRender GetAreaByPosition(Vector3 position)
        {
            lock(this.mAreas)
            {
	            return this.mAreas.Values.FirstOrDefault(x => x.AreaFile.BoundingBox.Contains(position) == ContainmentType.Contains);
            }
        }

        public void OnEditTerrain(Editing.TerrainChangeParameters parameters)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            foreach (var pair in this.mAreas)
            {
	            pair.Value.OnTerrainChange(parameters);
            }

	        if(parameters.AlignModels)
            {
                foreach (var pair in this.mAreas)
                {
	                pair.Value.OnUpdateModelPositions(parameters);
                }
            }
        }

        public void OnTextureTerrain(Editing.TextureChangeParameters parameters)
        {
            foreach (var pair in this.mAreas)
            {
	            pair.Value.OnTextureChange(parameters);
            }
        }

        public void OnSaveAllFiles()
        {
            foreach (var pair in this.mAreas)
            {
	            pair.Value.AreaFile.Save();
            }
        }

        public void Shutdown()
        {
	        this.mIsRunning = false;
	        this.mLoadThread.Join();
	        this.mLightUpdateThread.Join();
	        this.mUnloadThread.Join();
	        this.mAreaLowManager.Shutdown();
        }

        public void OnFrame(Camera camera)
        {
            ProcessLoadedTiles();

            if (WorldFrame.Instance.State == AppState.World)
            {
                SkyManager.Instance.SyncUpdate();
	            this.SkySphere.Render();
	            this.mAreaLowManager.OnFrame();
            }

            if(!this.HideTerrain)
            {
                MapChunkRender.ChunkMesh.BeginDraw();
                MapChunkRender.ChunkMesh.Program.SetPixelSampler(0, MapChunkRender.ColorSampler);
                MapChunkRender.ChunkMesh.Program.SetPixelSampler(1, MapChunkRender.AlphaSampler);

                if (WorldFrame.Instance.LastMouseIntersection.ChunkHit != null)
                {
                    EditorWindowController.Instance.OnUpdateChunkIndex(WorldFrame.Instance.LastMouseIntersection.ChunkHit.IndexX, WorldFrame.Instance.LastMouseIntersection.ChunkHit.IndexY);
                }
                else
                {
                    EditorWindowController.Instance.OnUpdateChunkIndex(0, 0);
                }

	            foreach (var pair in this.mAreas)
	            {
		            pair.Value.OnFrame();
	            }
            }
        }

        public void EnterWorld(Vector2 entryPoint, int mapId)
        {
            var row = Storage.DbcStorage.Map.GetRowById(mapId);
	        if (row == null)
	        {
		        return;
	        }

            var continent = row.GetString(Storage.MapFormatGuess.FieldMapName);

            MapChunkRender.InitIndices();
            WorldFrame.Instance.LeftHandedCamera = FileManager.Instance.Version > FileDataVersion.Cataclysm;

	        this.mEntryPoint = entryPoint;
	        this.Continent = continent;
            WorldFrame.Instance.State = AppState.LoadingScreen;

	        var wdtPath = string.Format(@"World\Maps\{0}\{0}.wdt", continent);
	        using (MemoryStream wdtFileStream = new MemoryStream())
	        {
		        using (var strm = FileManager.Instance.Provider.OpenFile(wdtPath))
		        {
			        strm.CopyTo(wdtFileStream);
		        }

		        this.CurrentWdt = new WorldTable(wdtFileStream.ToArray());
	        }

	        // PORT: Possible introduction of bug (bitwise flag comparison to HasFlag)
	        this.HasNewBlend = this.CurrentWdt.Header.Flags.HasFlag(WorldTableFlags.UsesEnvironmentMapping) || this.CurrentWdt.Header.Flags.HasFlag(WorldTableFlags.UsesHardAlphaFalloff);

            MapChunkRender.ChunkMesh.Program = this.HasNewBlend ? MapChunkRender.BlendNew : MapChunkRender.BlendOld;

	        this.IsInitialLoad = true;

            SkyManager.Instance.OnEnterWorld(mapId);
	        this.mAreaLowManager.OnEnterWorld(this.Continent, ref entryPoint);
            LoadInitial();
        }

        public void OnLoadProgress()
        {
            Interlocked.Increment(ref this.mLoadStepsDone);
            var pct = (float) this.mLoadStepsDone / this.mTotalLoadSteps;
	        if (this.IsInitialLoad)
	        {
		        EditorWindowController.Instance.LoadingScreen.UpdateProgress(pct);
	        }

	        if (this.mLoadStepsDone < this.mTotalLoadSteps || !this.IsInitialLoad)
	        {
		        return;
	        }

	        this.IsInitialLoad = false;
            OnInitialLoadDone();
        }

        public void UpdatePosition(Vector3 position, bool updateTerrain)
        {
            WorldFrame.Instance.UpdatePosition(position);
	        this.SkySphere.UpdatePosition(position);

            if(updateTerrain)
            {
                SkyManager.Instance.UpdatePosition(position);
                var pos2D = new Vector2(position.X, position.Y);
	            this.mAreaLowManager.UpdatePosition(ref pos2D);
                UpdateVisibility(ref position);
            }

            EditorWindowController.Instance.OnUpdatePosition(position);

            var x = position.X;
            var y = position.Y;

	        if (FileManager.Instance.Version <= FileDataVersion.Warlords)
	        {
		        y = 64.0f * Metrics.TileSize - y;
	        }

            var tilex = (int)Math.Floor(x / Metrics.TileSize);
            var tiley = (int)Math.Floor(y / Metrics.TileSize);

            EditorWindowController.Instance.OnUpdateTileIndex(tilex, tiley);

        }

        public bool GetLandHeight(float x, float y, out float z)
        {
            z = 0.0f;

            var tilex = (int) Math.Floor(x / Metrics.TileSize);
            var tiley = (int) Math.Floor(y / Metrics.TileSize);

            x -= tilex * Metrics.TileSize;
            y -= tiley * Metrics.TileSize;

            var index = tilex + tiley * 0xFF;
            MapAreaRender tile;
	        lock (this.mAreas)
	        {
		        this.mAreas.TryGetValue(index, out tile);
	        }

	        if (tile == null)
	        {
		        return false;
	        }

            var chunkx = (int) Math.Floor(x / Metrics.ChunkSize);
            var chunky = (int) Math.Floor(y / Metrics.ChunkSize);

	        if (chunkx < 0 || chunky < 0 || chunkx > 15 || chunky > 15)
	        {
		        return false;
	        }

            var chunk = tile.AreaFile.GetChunk(chunkx + chunky * 16);
	        if (chunk == null)
	        {
		        return false;
	        }

            x -= chunkx * Metrics.ChunkSize;
            y -= chunky * Metrics.ChunkSize;

            var row = (int)(y / (Metrics.UnitSize * 0.5f) + 0.5f);
            var col = (int)((x - Metrics.UnitSize * 0.5f * (row % 2)) / Metrics.UnitSize + 0.5f);

	        if (row < 0 || col < 0 || row > 16 || col > (((row % 2) != 0) ? 8 : 9))
	        {
		        return false;
	        }

            z = chunk.Vertices[17 * (row / 2) + (((row % 2) != 0) ? 9 : 0) + col].Position.Z;
            return true;
        }

        public bool ToggleWireframe()
        {
            MapChunkRender.WireFrame = !MapChunkRender.WireFrame;
            return MapChunkRender.WireFrame;
        }

        public void Intersect(IntersectionParams parameters)
        {
            var ray = Picking.Build(ref parameters.ScreenPosition,
                ref parameters.InverseView, ref parameters.InverseProjection);

            var hasHit = false;
            var minDist = float.MaxValue;
            MapChunk chunkHit = null;

            // ReSharper disable once InconsistentlySynchronizedField
            foreach(var pair in this.mAreas)
            {
                MapChunk chunk;
                float distance;
	            if (!pair.Value.AreaFile.Intersect(ref ray, out chunk, out distance))
	            {
		            continue;
	            }

                hasHit = true;
	            if (distance >= minDist)
	            {
		            continue;
	            }

                minDist = distance;
                chunkHit = chunk;
            }

            parameters.TerrainHit = hasHit;
	        if (hasHit)
	        {
		        parameters.TerrainPosition = ray.Position + minDist * ray.Direction;
		        parameters.TerrainDistance = minDist;
	        }
	        else
	        {
		        parameters.TerrainPosition = new Vector3(float.MaxValue);
	        }

            parameters.ChunkHit = chunkHit;
        }

        private void LoadInitial()
        {
            var ix = (int) Math.Floor(this.mEntryPoint.X / Metrics.TileSize);
            var iy = (int) Math.Floor((64.0f * Metrics.TileSize - this.mEntryPoint.Y) / Metrics.TileSize);

	        this.mTotalLoadSteps = 0;
	        this.mLoadStepsDone = 0;

            lock(this.mDataToLoad)
            {
	            this.mDataToLoad.Clear();
	            this.mLoadedData.Clear();
	            this.mAreas.Clear();

                for (var x = ix - MapRadius; x <= ix + MapRadius; ++x)
                {
                    for (var y = iy - MapRadius; y <= iy + MapRadius; ++y)
                    {
	                    if (x < 0 || y < 0 || x > 63 || y > 63)
	                    {
		                    continue;
	                    }

	                    if (FileManager.Instance.Provider.Exists(string.Format(@"World\Maps\{0}\{0}_{1}_{2}.adt", this.Continent, x, y)) == false)
	                    {
		                    continue;
	                    }

                        var tile = AdtFactory.Instance.CreateArea(this.Continent, x, y);
	                    this.mDataToLoad.Add(tile);
	                    this.mTotalLoadSteps += 2 * 256;
	                    this.mCurrentValidLinks.Add(x + y * 64);
                    }
                }
            }
        }

        private void OnInitialLoadDone()
        {
            float height;
            if (GetLandHeight(this.mEntryPoint.X, 64.0f * Metrics.TileSize - this.mEntryPoint.Y, out height))
            {
                height += 50.0f;
                SkyManager.Instance.UpdatePosition(new Vector3(this.mEntryPoint.X, this.mEntryPoint.Y, height));

                var entryPoint = new Vector3(this.mEntryPoint.X, this.mEntryPoint.Y, height);
	            if (FileManager.Instance.Version > FileDataVersion.Mists)
	            {
		            entryPoint.Y = 64.0f * Metrics.TileSize - this.mEntryPoint.Y;
	            }

                SkyManager.Instance.AsyncUpdate();
                EditorWindowController.Instance.OnEnterWorld();
                WorldFrame.Instance.OnEnterWorld(entryPoint);
                WorldFrame.Instance.Dispatcher.BeginInvoke(
                    () =>
                    {
	                    this.SkySphere.UpdatePosition(new Vector3(this.mEntryPoint.X, this.mEntryPoint.Y, height));
                        SkyManager.Instance.SyncUpdate();
                        WorldFrame.Instance.CamControl.ForceUpdate(WorldFrame.Instance.ActiveCamera.Position);
                        WorldFrame.Instance.M2Manager.ViewChanged();
                    });
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void LoadProc()
        {
            while(this.mIsRunning)
            {
                MapArea loadTile = null;
                lock(this.mDataToLoad)
                {
                    if (this.mDataToLoad.Count > 0)
                    {
                        loadTile = this.mDataToLoad[0];
	                    this.mDataToLoad.RemoveAt(0);
                    }
                }

	            if (loadTile != null)
	            {
		            loadTile.AsyncLoad();
		            lock (this.mLoadedData)
		            {
			            this.mLoadedData.Add(loadTile);
		            }
	            }
	            else
	            {
		            Thread.Sleep(30);
	            }
            }
        }

        private void ProcessLoadedTiles()
        {
            MapArea data = null;
            var index = 0;
            lock(this.mLoadedData)
            {
                if (this.mLoadedData.Count > 0)
                {
                    data = this.mLoadedData[0];
	                this.mLoadedData.RemoveAt(0);
                    index = data.IndexX + data.IndexY * 0xFF;
                    if (this.mAreas.ContainsKey(index))
                    {
                        data.Dispose();
                        return;
                    }
                }
            }

	        if (data == null)
	        {
		        return;
	        }

            var tile = new MapAreaRender(data.IndexX, data.IndexY);
            tile.AsyncLoaded(data);
	        this.mAreas.Add(index, tile);
        }

        private void LightUpdateProc()
        {
            while(this.mIsRunning)
            {
	            if (SkyManager.Instance != null)
	            {
		            SkyManager.Instance.AsyncUpdate();
	            }

                Thread.Sleep(100);
            }
        }

        private void UpdateVisibility(ref Vector3 position)
        {
            var cx = position.X;
            var cy = position.Y;
	        if (FileManager.Instance.Version < FileDataVersion.Lichking)
	        {
		        cy = 64.0f * Metrics.TileSize - cy;
	        }

            var ix = (int) Math.Floor(cx / Metrics.TileSize);
            var iy = (int) Math.Floor(cy / Metrics.TileSize);

            var countPref = this.mCurrentValidLinks.Count;
	        this.mCurrentValidLinks.RemoveAll(index =>
            {
                var x = index % 64;
                var y = index / 64;
                return (x > ix + 2 || x < ix - 2 || y > iy + 2 || y < iy - 2);
            });

	        if (countPref == this.mCurrentValidLinks.Count)
	        {
		        return;
	        }

	        this.mCurrentValidLinks.Clear();
            for (var x = ix - MapRadius; x <= ix + MapRadius; ++x)
            {
                for (var y = iy - MapRadius; y <= iy + MapRadius; ++y)
                {
	                if (x < 0 || y < 0 || x > 63 || y > 63)
	                {
		                continue;
	                }

	                this.mCurrentValidLinks.Add(y * 64 + x);
                }
            }

            var loadMask = new List<int>();
            var invalidList = new List<MapAreaRender>();
            // ReSharper disable once InconsistentlySynchronizedField
            foreach(var pair in this.mAreas)
            {
                var tile = pair.Value;
                var index = tile.IndexX + tile.IndexY * 64;
                var indexX = tile.IndexX;
                var indexY = tile.IndexY;
                if (indexX < ix - 2 || indexX > ix + 2 || indexY < iy - 2 || indexY > iy + 2)
                {
                    invalidList.Add(tile);
                    continue;
                }

                loadMask.Add(index);
            }

            lock(this.mUnloadList)
            {
                foreach(var tile in invalidList)
                {
	                this.mUnloadList.Add(tile);
	                this.mAreas.Remove(tile.IndexX + tile.IndexY * 0xFF);
                }
            }

            invalidList.Clear();

            lock(this.mDataToLoad)
            {
                loadMask.AddRange(this.mDataToLoad.Select(tile => tile.IndexX + tile.IndexY * 64));
            }

            lock(this.mLoadedData)
            {
                loadMask.AddRange(this.mLoadedData.Select(tile => tile.IndexX + tile.IndexY * 64));
            }

            foreach(var link in this.mCurrentValidLinks)
            {
	            if (loadMask.Contains(link))
	            {
		            continue;
	            }

                var x = link % 64;
                var y = link / 64;

                var area = AdtFactory.Instance.CreateArea(this.Continent, x, y);
	            lock (this.mDataToLoad)
	            {
		            this.mDataToLoad.Add(area);
	            }
            }
        }

        private void UnloadProc()
        {
            while(this.mIsRunning)
            {
                lock(this.mUnloadList)
                {
	                foreach (var tile in this.mUnloadList)
	                {
		                tile.Dispose();
	                }

	                this.mUnloadList.Clear();
                }
                Thread.Sleep(500);
            }
        }
    }
}
