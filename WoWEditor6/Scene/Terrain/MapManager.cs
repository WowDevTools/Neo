using System;
using System.Collections.Generic;
using System.Threading;
using SharpDX;
using WoWEditor6.UI;
using WoWEditor6.UI.Views;
using System.Linq;

namespace WoWEditor6.Scene.Terrain
{
    class MapManager
    {
        private Vector2 mEntryPoint;
        private int mTotalLoadSteps;
        private int mLoadStepsDone;
        private readonly List<IO.Files.Terrain.WoD.MapArea> mDataToLoad = new List<IO.Files.Terrain.WoD.MapArea>();
        private List<IO.Files.Terrain.WoD.MapArea> mLoadedData = new List<IO.Files.Terrain.WoD.MapArea>();
        private Dictionary<int, MapAreaRender> mAreas = new Dictionary<int, MapAreaRender>();
        private Thread mLoadThread;
        private bool mIsRunning;

        public string Continent { get; private set; }
        public int MapId { get; private set; }
        public IO.Files.Terrain.WdtFile CurrentWdt { get; private set; }
        public bool HasNewBlend { get; private set; }
        public bool IsInitialLoad { get; private set; }

        public void Initialize()
        {
            mIsRunning = true;
            mLoadThread = new Thread(LoadProc);
            mLoadThread.Start();
        }

        public void Shutdown()
        {
            mIsRunning = false;
            mLoadThread.Join();
        }

        public void OnFrame()
        {
            ProcessLoadedTiles();

            MapChunkRender.ChunkMesh.BeginDraw();
            MapChunkRender.ChunkMesh.Program.SetPixelSampler(0, MapChunkRender.ColorSampler);
            MapChunkRender.ChunkMesh.Program.SetPixelSampler(1, MapChunkRender.AlphaSampler);

            foreach (var pair in mAreas)
                pair.Value.OnFrame();
        }

        public void EnterWorld(Vector2 entryPoint, int mapId, string continent)
        {
            mEntryPoint = entryPoint;
            MapId = mapId;
            Continent = continent;
            WorldFrame.Instance.State = AppState.LoadingScreen;

            CurrentWdt = new IO.Files.Terrain.WdtFile();
            CurrentWdt.Load(continent);
            HasNewBlend = (CurrentWdt.Flags & 0x84) != 0;

            IsInitialLoad = true;

            LoadInitial();
        }

        public void OnLoadProgress()
        {
            ++mLoadStepsDone;
            var pct = (float) mLoadStepsDone / mTotalLoadSteps;
            if (IsInitialLoad)
                InterfaceManager.Instance.GetViewForState<LoadingScreenView>(AppState.LoadingScreen).UpdateProgress(pct);

            if (mLoadStepsDone < mTotalLoadSteps || !IsInitialLoad) return;

            IsInitialLoad = false;
            OnInitialLoadDone();
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
            lock(mAreas)
                mAreas.TryGetValue(index, out tile);

            if (tile == null)
                return false;

            var chunkx = (int) Math.Floor(x / Metrics.ChunkSize);
            var chunky = (int) Math.Floor(y / Metrics.ChunkSize);

            if (chunkx < 0 || chunky < 0 || chunkx > 15 || chunky > 15)
                return false;

            var chunk = tile.AreaFile.GetChunk(chunkx + chunky * 16);
            if (chunk == null)
                return false;

            x -= chunkx * Metrics.ChunkSize;
            y -= chunky * Metrics.ChunkSize;

            var row = (int) Math.Floor(y / (Metrics.UnitSize * 0.5f) + 0.5f);
            var col =
                (int) Math.Floor((x + Metrics.UnitSize * 0.5f * ((row % 2 != 0) ? 1 : 0)) / Metrics.UnitSize + 0.5f);
            if (row < 0 || col < 0 || row > 16 || col > (((row % 2) != 0) ? 8 : 9))
                return false;

            z = chunk.Vertices[17 * (row / 2) + (((row % 2) != 0) ? 9 : 0) + col].Position.Z;
            return true;
        }

        private void LoadInitial()
        {
            var ix = (int) Math.Floor(mEntryPoint.X / Metrics.TileSize);
            var iy = (int) Math.Floor(mEntryPoint.Y / Metrics.TileSize);
            iy = 64 - iy;

            mTotalLoadSteps = 0;
            mLoadStepsDone = 0;

            lock(mDataToLoad)
            {
                mDataToLoad.Clear();
                mLoadedData.Clear();
                mAreas.Clear();

                for (var x = ix - 1; x < ix + 2; ++x)
                {
                    for (var y = iy - 1; y < iy + 2; ++y)
                    {
                        if (x < 0 || y < 0 || x > 63 || y > 63)
                            continue;

                        var tile = new IO.Files.Terrain.WoD.MapArea(Continent, x, y);
                        mDataToLoad.Add(tile);
                        mTotalLoadSteps += 2 * 256;
                    }
                }
            }
        }

        private void OnInitialLoadDone()
        {
            InterfaceManager.Instance.UpdateState(AppState.World);
            float height;
            if (GetLandHeight(mEntryPoint.X, 64.0f * Metrics.TileSize - mEntryPoint.Y, out height) == false)
                return;

            height += 50.0f;
            WorldFrame.Instance.OnEnterWorld(new Vector3(mEntryPoint, height));
        }

        private void LoadProc()
        {
            while(mIsRunning)
            {
                IO.Files.Terrain.WoD.MapArea loadTile = null;
                lock(mDataToLoad)
                {
                    if (mDataToLoad.Count > 0)
                    {
                        loadTile = mDataToLoad[0];
                        mDataToLoad.RemoveAt(0);
                    }
                }

                if (loadTile != null)
                {
                    loadTile.AsyncLoad();
                    lock (mLoadedData)
                        mLoadedData.Add(loadTile);
                }
                else
                    Thread.Sleep(30);
            }
        }

        private void ProcessLoadedTiles()
        {
            lock(mLoadedData)
            {
                foreach (var data in mLoadedData)
                {
                    var tile = new MapAreaRender(data.IndexX, data.IndexY);
                    tile.AsyncLoaded(data);
                    mAreas.Add(data.IndexX + data.IndexY * 0xFF, tile);
                }

                mLoadedData.Clear();
            }
        }
    }
}
