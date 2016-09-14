using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SharpDX;

namespace Neo.Scene.Terrain
{
    class MapLowManager
    {
        private IO.Files.Terrain.WdlFile mWdlFile;

        private readonly List<MapAreaLowRender> mAreas = new List<MapAreaLowRender>();
        private readonly List<MapAreaLowRender> mDataToLoad = new List<MapAreaLowRender>();
        private readonly List<MapAreaLowRender> mLoadedData = new List<MapAreaLowRender>();
        private readonly List<MapAreaLowRender> mUnloadAreas = new List<MapAreaLowRender>();
        private readonly List<int> mCurrentValidLinks = new List<int>();

        private readonly Thread mLoadThread;
        private readonly Thread mUnloadThread;

        private bool mIsRunning = true;

        public MapLowManager()
        {
            mLoadThread = new Thread(LoadProc);
            mUnloadThread = new Thread(UnloadProc);

            mLoadThread.Start();
            mUnloadThread.Start();
        }

        public void OnEnterWorld(string continent, ref Vector2 position)
        {
            mWdlFile = new IO.Files.Terrain.WdlFile();
            mWdlFile.Load(continent);

            UpdatePosition(ref position);
        }

        public void Shutdown()
        {
            mIsRunning = false;
            mLoadThread.Join();
            mUnloadThread.Join();

            mAreas.Clear();
            // ReSharper disable InconsistentlySynchronizedField
            mDataToLoad.Clear();
            mLoadedData.Clear();
        }

        public void UpdatePosition(ref Vector2 position)
        {
            var cx = position.X;
            var cy = position.Y;

            if (IO.FileManager.Instance.Version < IO.FileDataVersion.Lichking)
                cy = 64.0f * Metrics.TileSize - cy;

            var ix = (int)Math.Floor(cx / Metrics.TileSize);
            var iy = (int)Math.Floor(cy / Metrics.TileSize);

            var oldCount = mCurrentValidLinks.Count;
            mCurrentValidLinks.RemoveAll(index =>
            {
                var x = index % 0xFF;
                var y = index / 0xFF;
                return (x > ix + 4 || x < ix - 4 || y > iy + 4 || y < iy - 4);
            });

            if (oldCount == mCurrentValidLinks.Count && oldCount != 0)
                return;

            mCurrentValidLinks.Clear();
            for (var x = ix - 4; x <= ix + 4; ++x)
            {
                for (var y = iy - 4; y <= iy + 4; ++y)
                {
                    if (x < 0 || y < 0 || x > 63 || y > 63)
                        continue;

                    mCurrentValidLinks.Add(y * 0xFF + x);
                }
            }

            var loadMask = new List<int>();
            var invalidList = new List<MapAreaLowRender>();
            foreach (var tile in mAreas)
            {
                var index = tile.IndexX + tile.IndexY * 0xFF;
                var indexX = tile.IndexX;
                var indexY = tile.IndexY;
                if (indexX < ix - 4 || indexX > ix + 4 || indexY < iy - 4 || indexY > iy + 4)
                {
                    invalidList.Add(tile);
                    continue;
                }

                loadMask.Add(index);
            }

            lock(mUnloadAreas)
            {
                foreach(var area in invalidList)
                {
                    mAreas.Remove(area);
                    mUnloadAreas.Add(area);
                }
            }

            lock(mDataToLoad)
            {
                loadMask.AddRange(mDataToLoad.Select(tile => tile.IndexX + tile.IndexY * 0xFF));
            }

            foreach(var link in mCurrentValidLinks.Where(i => loadMask.Contains(i) == false))
            {
                var x = link % 0xFF;
                var y = link / 0xFF;

                var provider = new MapAreaLowRender(x, y);
                lock (mDataToLoad)
                    mDataToLoad.Add(provider);
            }
        }

        public void OnFrame()
        {
            MapAreaLowRender.Mesh.BeginDraw();

            lock(mLoadedData)
            {
                foreach(var data in mLoadedData)
                {
                    var index = data.IndexX + data.IndexY * 0xFF;
                    if (mCurrentValidLinks.Contains(index) == false)
                        continue;

                    mAreas.Add(data);
                }

                mLoadedData.Clear();
            }

            foreach (var area in mAreas)
                area.OnFrame();
        }

        private void LoadProc()
        {
            while(mIsRunning)
            {
                MapAreaLowRender item = null;
                lock(mDataToLoad)
                {
                    if(mDataToLoad.Count > 0)
                    {
                        item = mDataToLoad[0];
                        mDataToLoad.RemoveAt(0);
                    }
                }

                if(item == null)
                {
                    Thread.Sleep(30);
                    continue;
                }

                item.InitFromHeightData(mWdlFile.GetEntry(item.IndexX, item.IndexY));
                lock (mLoadedData)
                    mLoadedData.Add(item);
            }
        }

        private void UnloadProc()
        {
            while(mIsRunning)
            {
                lock (mUnloadAreas)
                {
                    foreach (var area in mUnloadAreas)
                        area.Dispose();

                    mUnloadAreas.Clear();
                }

                Thread.Sleep(1000);
            }
        }
    }
}
