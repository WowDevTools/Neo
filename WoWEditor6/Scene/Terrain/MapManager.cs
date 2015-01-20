using System;
using System.Collections.Generic;
using System.Threading;
using SharpDX;
using WoWEditor6.UI;
using WoWEditor6.UI.Views;

namespace WoWEditor6.Scene.Terrain
{
    class MapManager
    {
        private Vector2 mEntryPoint;
        private int mTotalLoadSteps;
        private int mLoadStepsDone;
        private readonly List<IO.Files.Terrain.MapArea> mDataToLoad = new List<IO.Files.Terrain.MapArea>();
        private readonly List<IO.Files.Terrain.MapArea> mLoadedData = new List<IO.Files.Terrain.MapArea>();
        private readonly Dictionary<int, MapAreaRender> mAreas = new Dictionary<int, MapAreaRender>();
        private Thread mLoadThread;
        private Thread mLightUpdateThread;
        private bool mIsRunning;

        public string Continent { get; private set; }
        public int MapId { get; private set; }
        public IO.Files.Terrain.WdtFile CurrentWdt { get; private set; }
        public bool HasNewBlend { get; private set; }
        public bool IsInitialLoad { get; private set; }
        public SkySphere SkySphere { get; private set; }

        public void Initialize()
        {
            SkySphere = new SkySphere(999.0f, 25, 25, WorldFrame.Instance.GraphicsContext);
            mIsRunning = true;
            mLoadThread = new Thread(LoadProc);
            mLoadThread.Start();
            mLightUpdateThread = new Thread(LightUpdateProc);
            mLightUpdateThread.Start();
        }

        public void Shutdown()
        {
            mIsRunning = false;
            mLoadThread.Join();
            mLightUpdateThread.Join();
        }

        public void OnFrame()
        {
            ProcessLoadedTiles();

            if (WorldFrame.Instance.State == AppState.World)
            {
                IO.Files.Sky.SkyManager.Instance.SyncUpdate();
                SkySphere.Render();
            }

            MapChunkRender.ChunkMesh.BeginDraw();
            MapChunkRender.ChunkMesh.Program.SetPixelSampler(0, MapChunkRender.ColorSampler);
            MapChunkRender.ChunkMesh.Program.SetPixelSampler(1, MapChunkRender.AlphaSampler);

            // ReSharper disable once InconsistentlySynchronizedField
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

            MapChunkRender.ChunkMesh.Program = HasNewBlend ? MapChunkRender.BlendNew : MapChunkRender.BlendOld;

            IsInitialLoad = true;

            IO.Files.Sky.SkyManager.Instance.OnEnterWorld(mapId);

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
            var iy = (int) Math.Floor((64.0f * Metrics.TileSize - mEntryPoint.Y) / Metrics.TileSize);

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

                        var tile = IO.Files.Terrain.AdtFactory.Instance.CreateArea(Continent, x, y);
                        mDataToLoad.Add(tile);
                        mTotalLoadSteps += 2 * 256;
                    }
                }
            }
        }

        private void OnInitialLoadDone()
        {
            float height;
            if (GetLandHeight(mEntryPoint.X, 64.0f * Metrics.TileSize - mEntryPoint.Y, out height) == false)
                return;

            height += 50.0f;
            IO.Files.Sky.SkyManager.Instance.UpdatePosition(new Vector3(mEntryPoint, height));
            WorldFrame.Instance.OnEnterWorld(new Vector3(mEntryPoint, height));
            WorldFrame.Instance.Dispatcher.BeginInvoke(
                new Action(() => SkySphere.UpdatePosition(new Vector3(mEntryPoint, height))));
        }

        private void LoadProc()
        {
            while(mIsRunning)
            {
                IO.Files.Terrain.MapArea loadTile = null;
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

        private void LightUpdateProc()
        {
            while(mIsRunning)
            {
                if (IO.Files.Sky.SkyManager.Instance != null)
                    IO.Files.Sky.SkyManager.Instance.AsyncUpdate();

                Thread.Sleep(100);
            }
        }
    }
}
