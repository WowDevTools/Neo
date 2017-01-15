using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Neo.IO;
using OpenTK;
using Warcraft.WDL;

namespace Neo.Scene.Terrain
{
	internal class MapLowManager
    {
        private WorldLOD mWdlFile;

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
	        this.mLoadThread = new Thread(LoadProc);
	        this.mUnloadThread = new Thread(UnloadProc);

	        this.mLoadThread.Start();
	        this.mUnloadThread.Start();
        }

        public void OnEnterWorld(string continent, ref Vector2 position)
        {
	        var wdlPath = string.Format(@"World\Maps\{0}\{0}.wdl", continent);
	        using (MemoryStream wdlFileStream = new MemoryStream())
	        {
		        using (var strm = FileManager.Instance.Provider.OpenFile(wdlPath))
		        {
			        strm.CopyTo(wdlFileStream);
		        }

		        this.mWdlFile = new WorldLOD(wdlFileStream.ToArray());
	        }

	        //mWdlFile = new IO.Files.Terrain.WdlFile();
            //mWdlFile.Load(continent);

            UpdatePosition(ref position);
        }

        public void Shutdown()
        {
	        this.mIsRunning = false;
	        this.mLoadThread.Join();
	        this.mUnloadThread.Join();

	        this.mAreas.Clear();
            // ReSharper disable InconsistentlySynchronizedField
	        this.mDataToLoad.Clear();
	        this.mLoadedData.Clear();
        }

        public void UpdatePosition(ref Vector2 position)
        {
            var cx = position.X;
            var cy = position.Y;

            if (FileManager.Instance.Version < FileDataVersion.Lichking)
            {
	            cy = 64.0f * Metrics.TileSize - cy;
            }

	        var ix = (int)Math.Floor(cx / Metrics.TileSize);
            var iy = (int)Math.Floor(cy / Metrics.TileSize);

            var oldCount = this.mCurrentValidLinks.Count;
	        this.mCurrentValidLinks.RemoveAll(index =>
            {
                var x = index % 0xFF;
                var y = index / 0xFF;
                return (x > ix + 4 || x < ix - 4 || y > iy + 4 || y < iy - 4);
            });

            if (oldCount == this.mCurrentValidLinks.Count && oldCount != 0)
            {
	            return;
            }

	        this.mCurrentValidLinks.Clear();
            for (var x = ix - 4; x <= ix + 4; ++x)
            {
                for (var y = iy - 4; y <= iy + 4; ++y)
                {
                    if (x < 0 || y < 0 || x > 63 || y > 63)
                    {
	                    continue;
                    }

	                this.mCurrentValidLinks.Add(y * 0xFF + x);
                }
            }

            var loadMask = new List<int>();
            var invalidList = new List<MapAreaLowRender>();
            foreach (var tile in this.mAreas)
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

            lock(this.mUnloadAreas)
            {
                foreach(var area in invalidList)
                {
	                this.mAreas.Remove(area);
	                this.mUnloadAreas.Add(area);
                }
            }

            lock(this.mDataToLoad)
            {
                loadMask.AddRange(this.mDataToLoad.Select(tile => tile.IndexX + tile.IndexY * 0xFF));
            }

            foreach(var link in this.mCurrentValidLinks.Where(i => loadMask.Contains(i) == false))
            {
                var x = link % 0xFF;
                var y = link / 0xFF;

                var provider = new MapAreaLowRender(x, y);
                lock (this.mDataToLoad)
                {
	                this.mDataToLoad.Add(provider);
                }
            }
        }

        public void OnFrame()
        {
            MapAreaLowRender.Mesh.BeginDraw();

            lock(this.mLoadedData)
            {
                foreach(var data in this.mLoadedData)
                {
                    var index = data.IndexX + data.IndexY * 0xFF;
                    if (this.mCurrentValidLinks.Contains(index) == false)
                    {
	                    continue;
                    }

	                this.mAreas.Add(data);
                }

	            this.mLoadedData.Clear();
            }

            foreach (var area in this.mAreas)
            {
	            area.OnFrame();
            }
        }

        private void LoadProc()
        {
            while(this.mIsRunning)
            {
                MapAreaLowRender item = null;
                lock(this.mDataToLoad)
                {
                    if(this.mDataToLoad.Count > 0)
                    {
                        item = this.mDataToLoad[0];
	                    this.mDataToLoad.RemoveAt(0);
                    }
                }

                if(item == null)
                {
                    Thread.Sleep(30);
                    continue;
                }

                item.InitFromHeightData(this.mWdlFile.GetEntry(item.IndexX, item.IndexY));
                lock (this.mLoadedData)
                {
	                this.mLoadedData.Add(item);
                }
            }
        }

        private void UnloadProc()
        {
            while(this.mIsRunning)
            {
                lock (this.mUnloadAreas)
                {
                    foreach (var area in this.mUnloadAreas)
                    {
	                    area.Dispose();
                    }

	                this.mUnloadAreas.Clear();
                }

                Thread.Sleep(1000);
            }
        }
    }
}
