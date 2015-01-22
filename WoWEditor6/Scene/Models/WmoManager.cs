using System.Collections.Generic;
using SharpDX;
using WoWEditor6.Scene.Models.WMO;

namespace WoWEditor6.Scene.Models
{
    class WmoManager
    {
        private readonly Dictionary<int, WmoBatchRender> mRenderer = new Dictionary<int, WmoBatchRender>();
        private readonly object mAddLock = new object();

        public void PreloadModel(string model)
        {
            var hash = model.ToUpperInvariant().GetHashCode();
            lock(mRenderer)
            {
                if (mRenderer.ContainsKey(hash))
                    return;

                var root = IO.Files.Models.ModelFactory.Instance.CreateWmo();
                if (root.Load(model) == false)
                    Log.Warning("Unable to load WMO '" + model + "'. Further instances wont be loaded again");

                var renderer = new WmoRootRender();
                renderer.OnAsyncLoad(root);

                var batch = new WmoBatchRender(renderer);

                lock (mAddLock)
                    mRenderer.Add(hash, batch);
            }
        }

        public void AddInstance(string model, int uuid, Vector3 position, Vector3 rotation)
        {
            var hash = model.ToUpperInvariant().GetHashCode();

            WmoBatchRender batch;
            lock(mRenderer)
            {
                if(mRenderer.TryGetValue(hash, out batch) == false)
                {
                    PreloadModel(model);
                    mRenderer.TryGetValue(hash, out batch);
                }
            }

            if(batch == null)
            {
                Log.Error("Internal error adding an instance of a WMO");
                return;
            }

            batch.AddInstance(uuid, position, rotation);
        }

        public void ForceVisibilityUpdate()
        {
            lock(mAddLock)
            {
                foreach (var pair in mRenderer)
                    pair.Value.UpdateVisibility();
            }
        }

        public void OnFrame()
        {
            WmoGroupRender.Mesh.Program.SetPixelSampler(0, WmoGroupRender.Sampler);
            lock(mAddLock)
            {
                foreach (var pair in mRenderer)
                    pair.Value.OnFrame();
            }
        }
    }
}
