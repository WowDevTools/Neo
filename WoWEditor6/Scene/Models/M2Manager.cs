using System;
using System.Collections.Generic;
using SharpDX;
using WoWEditor6.IO.Files.Models;
using WoWEditor6.Scene.Models.M2;

namespace WoWEditor6.Scene.Models
{
    class M2Manager
    {
        private readonly Dictionary<int, M2BatchRenderer> mRenderer = new Dictionary<int, M2BatchRenderer>();
        private readonly object mAddLock = new object();

        public static bool IsViewDirty { get; private set; }

        public void OnFrame()
        {
            M2BatchRenderer.Mesh.BeginDraw();
            M2BatchRenderer.Mesh.Program.SetPixelSampler(0, M2BatchRenderer.Sampler);

            lock(mAddLock)
            {
                foreach (var pair in mRenderer)
                    pair.Value.OnFrame();
            }

            IsViewDirty = false;
        }

        public void PushMapReferences(M2Instance[] instances)
        {
            lock (mAddLock)
            {
                foreach (var instance in instances)
                {
                    M2BatchRenderer renderer;
                    if (mRenderer.TryGetValue(instance.Hash, out renderer))
                        renderer.PushMapReference(instance);
                }
            }
        }

        public void ViewChanged()
        {
            IsViewDirty = true;
            lock(mAddLock)
            {
                foreach (var pair in mRenderer)
                    pair.Value.ViewChanged();
            }
        }

        public BoundingBox AddInstance(string model, int uuid, Vector3 position, Vector3 rotation, Vector3 scaling)
        {
            var hash = model.ToUpperInvariant().GetHashCode();
            lock(mRenderer)
            {
                if (mRenderer.ContainsKey(hash))
                {
                    var renderer = mRenderer[hash];
                    return renderer.AddInstance(uuid, position, rotation, scaling);
                }

                var file = LoadModel(model);
                if (file == null)
                    return new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue));

                var batch = new M2BatchRenderer(file);
                lock (mAddLock)
                    mRenderer.Add(hash, batch);

                return batch.AddInstance(uuid, position, rotation, scaling);
            }
        }

        private static M2File LoadModel(string fileName)
        {
            var file = ModelFactory.Instance.CreateM2(fileName);
            try
            {
                return file.Load() == false ? null : file;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
