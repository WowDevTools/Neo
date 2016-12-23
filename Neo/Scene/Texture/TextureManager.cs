using System;
using System.Collections.Generic;
using System.Threading;
using Neo.IO.Files.Texture;

namespace Neo.Scene.Texture
{
	internal class TextureWorkItem
    {
        public Graphics.Texture Texture { get; private set; }
        public string FileName { get; private set; }

        public TextureWorkItem(string file, Graphics.Texture tex)
        {
	        this.Texture = tex;
	        this.FileName = file;
        }
    }

	internal class TextureManager
    {
        public static TextureManager Instance { get; private set; }

        static TextureManager()
        {
            Instance = new TextureManager();
        }

        private readonly Dictionary<int, WeakReference<Graphics.Texture>> mCache = new Dictionary<int, WeakReference<Graphics.Texture>>();
        private readonly List<TextureWorkItem> mWorkItems = new List<TextureWorkItem>();
        private readonly object mWorkEvent = new object();
        private bool mIsRunning = true;
        private readonly List<Thread> mThreads = new List<Thread>();

        public void Initialize()
        {
            for(var i = 0; i < 2; ++i)
            {
                var t = new Thread(ThreadProc);
                t.Start();
	            this.mThreads.Add(t);
            }
        }

        public void Shutdown()
        {
	        this.mIsRunning = false;
	        lock (this.mWorkItems)
	        {
		        this.mWorkItems.Clear();
	        }

	        lock (this.mWorkEvent)
	        {
		        Monitor.PulseAll(this.mWorkEvent);
	        }

	        this.mThreads.ForEach(t => t.Join());
        }

        public Graphics.Texture GetTexture(string path)
        {
	        if (string.IsNullOrEmpty(path))
	        {
		        return new Graphics.Texture();
	        }

            var hash = path.ToUpperInvariant().GetHashCode();
            TextureWorkItem workItem;
            Graphics.Texture retTexture;
            lock (this.mCache)
            {
                WeakReference<Graphics.Texture> ret;
                if(this.mCache.TryGetValue(hash, out ret))
                {
	                if (ret.TryGetTarget(out retTexture))
	                {
		                return retTexture;
	                }

	                this.mCache.Remove(hash);
                }

                retTexture = new Graphics.Texture();
	            this.mCache.Add(hash, new WeakReference<Graphics.Texture>(retTexture));
                workItem = new TextureWorkItem(path, retTexture);
            }

            lock (this.mWorkItems)
            {
	            this.mWorkItems.Add(workItem);
	            lock (this.mWorkEvent)
	            {
		            Monitor.Pulse(this.mWorkEvent);
	            }
            }

            return retTexture;
        }

        private void ThreadProc()
        {
            while(this.mIsRunning)
            {
                TextureWorkItem workItem = null;
                lock(this.mWorkItems)
                {
                    if(this.mWorkItems.Count > 0)
                    {
                        workItem = this.mWorkItems[0];
	                    this.mWorkItems.RemoveAt(0);
                    }
                }

                if (workItem == null)
                {
	                lock (this.mWorkEvent)
	                {
		                Monitor.Wait(this.mWorkEvent);
	                }
                }
                else
                {
                    var loadInfo = TextureLoader.Load(workItem.FileName);
	                if (loadInfo != null)
	                {
		                WorldFrame.Instance.Dispatcher.BeginInvoke(() => workItem.Texture.LoadFromLoadInfo(loadInfo));
	                }
	                else
	                {
		                Log.Warning("Load failed: " + workItem.FileName);
	                }
                }
            }
        }
    }
}
