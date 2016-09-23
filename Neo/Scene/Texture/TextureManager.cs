using System;
using System.Collections.Generic;
using System.Threading;
using Neo.IO.Files.Texture;

namespace Neo.Scene.Texture
{
    class TextureWorkItem
    {
        public Graphics.Texture Texture { get; private set; }
        public string FileName { get; private set; }

        public TextureWorkItem(string file, Graphics.Texture tex)
        {
            Texture = tex;
            FileName = file;
        }
    }

    class TextureManager
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
                mThreads.Add(t);
            }
        }

        public void Shutdown()
        {
            mIsRunning = false;
            lock (mWorkItems)
                mWorkItems.Clear();

            lock (mWorkEvent)
                Monitor.PulseAll(mWorkEvent);

            mThreads.ForEach(t => t.Join());
        }

        public Graphics.Texture GetTexture(string path)
        {
            if (string.IsNullOrEmpty(path))
                return new Graphics.Texture();

            var hash = path.ToUpperInvariant().GetHashCode();
            TextureWorkItem workItem;
            Graphics.Texture retTexture;
            lock (mCache)
            {
                WeakReference<Graphics.Texture> ret;
                if(mCache.TryGetValue(hash, out ret))
                {
                    if (ret.TryGetTarget(out retTexture))
                        return retTexture;

                    mCache.Remove(hash);
                }

                retTexture = new Graphics.Texture();
                mCache.Add(hash, new WeakReference<Graphics.Texture>(retTexture));
                workItem = new TextureWorkItem(path, retTexture);
            }

            lock (mWorkItems)
            {
                mWorkItems.Add(workItem);
                lock (mWorkEvent)
                    Monitor.Pulse(mWorkEvent);
            }

            return retTexture;
        }

        private void ThreadProc()
        {
            while(mIsRunning)
            {
                TextureWorkItem workItem = null;
                lock(mWorkItems)
                {
                    if(mWorkItems.Count > 0)
                    {
                        workItem = mWorkItems[0];
                        mWorkItems.RemoveAt(0);
                    }
                }

                if (workItem == null)
                {
                    lock (mWorkEvent)
                        Monitor.Wait(mWorkEvent);
                }
                else
                {
                    var loadInfo = TextureLoader.Load(workItem.FileName);
                    if (loadInfo != null)
                        WorldFrame.Instance.Dispatcher.BeginInvoke(() => workItem.Texture.LoadFromLoadInfo(loadInfo));
                    else
                        Log.Warning("Load failed: " + workItem.FileName);
                }
            }
        }
    }
}
