using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using WoWEditor6.Scene.Models.M2;

namespace WoWEditor6.Scene.Models
{
    class M2Manager
    {
        private readonly Dictionary<int, M2BatchRenderer> mRenderer = new Dictionary<int, M2BatchRenderer>();
        private readonly object mAddLock = new object();

        public void OnFrame()
        {
            lock(mAddLock)
            {
                foreach (var pair in mRenderer)
                    pair.Value.OnFrame();
            }
        }

        public void ViewChanged()
        {
            lock(mAddLock)
            {
                foreach (var pair in mRenderer)
                    pair.Value.ViewChanged();
            }
        }

        public void AddInstance(string model, int uuid, Vector3 position, Quaternion rotation, Vector3 scaling)
        {
            var hash = model.ToUpperInvariant().GetHashCode();
            lock(mRenderer)
            {
                if (mRenderer.ContainsKey(hash))
                {
                    var renderer = mRenderer[hash];
                    renderer.AddInstance(uuid, position, rotation, scaling);
                    return;
                }

                var file = LoadModel(model);
                if (file == null)
                    return;

                var batch = new M2BatchRenderer(file);
                lock (mAddLock)
                    mRenderer.Add(hash, batch);
            }
        }

        private static IO.Files.Models.M2File LoadModel(string fileName)
        {
            var file = IO.Files.Models.ModelFactory.Instance.CreateM2(fileName);
            return file.Load() == false ? null : file;
        }
    }
}
