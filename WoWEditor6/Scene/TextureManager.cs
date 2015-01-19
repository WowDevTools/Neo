using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using WoWEditor6.Graphics;
using WoWEditor6.IO.Files.Texture;

namespace WoWEditor6.Scene
{
    class TextureWorkItem
    {
        public Texture Texture { get; private set; }
        public string FileName { get; }

        public TextureWorkItem(string file, Texture tex)
        {
            Texture = tex;
            FileName = file;
        }
    }

    class TextureManager
    {
        public static TextureManager Instance { get; } = new TextureManager();

        private GxContext mContext;
        private readonly Dictionary<int, WeakReference<Texture>> mCache = new Dictionary<int, WeakReference<Texture>>();
        private readonly List<TextureWorkItem> mWorkItems = new List<TextureWorkItem>();
        private readonly object mWorkEvent = new object();
        private bool mIsRunning = true;
        private readonly List<Thread> mThreads = new List<Thread>();

        private Dispatcher mDispatcher;
        
        public void Initialize(GxContext context)
        {
            mContext = context;
            mDispatcher = Dispatcher.CurrentDispatcher;

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

        public Texture GetTexture(string path)
        {
            var hash = path.ToUpperInvariant().GetHashCode();
            TextureWorkItem workItem;
            Texture retTexture;
            lock (mCache)
            {
                WeakReference<Texture> ret;
                if(mCache.TryGetValue(hash, out ret))
                {
                    if (ret.TryGetTarget(out retTexture))
                        return retTexture;

                    mCache.Remove(hash);
                }

                retTexture = new Texture(mContext);
                mCache.Add(hash, new WeakReference<Texture>(retTexture));
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
                    mDispatcher.BeginInvoke(new Action(() => workItem.Texture.LoadFromLoadInfo(loadInfo)));
                }
            }
        }
    }
}
