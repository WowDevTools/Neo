using System;
using System.Collections.Generic;
using SharpDX;
using WoWEditor6.IO.Files.Models;
using WoWEditor6.Scene;

namespace WoWEditor6.IO.Files.Terrain
{
    abstract class MapArea : IDisposable
    {
        public int IndexX { get; protected set; }
        public int IndexY { get; protected set; }
        public string Continent { get; protected set; }
        public bool IsValid { get; protected set; }

        public List<M2Instance> DoodadInstances { get; private set; }

        public AdtVertex[] FullVertices { get; private set; }

        public List<string> TextureNames { get; private set; } 

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public BoundingBox BoundingBox { get; protected set; }
        public BoundingBox ModelBox { get; protected set; }

        protected MapArea()
        {
            IsValid = true;
            DoodadInstances = new List<M2Instance>();
            FullVertices = new AdtVertex[145 * 256];
            TextureNames = new List<string>();
        }

        ~MapArea()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (DoodadInstances != null)
            {
                foreach (var instance in DoodadInstances)
                    WorldFrame.Instance.M2Manager.RemoveInstance(instance.Hash, instance.Uuid);

                DoodadInstances.Clear();
                DoodadInstances = null;
            }

            if (TextureNames != null)
            {
                TextureNames.Clear();
                TextureNames = null;
            }

            FullVertices = null;
            Continent = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract void Save();

        public abstract Graphics.Texture GetTexture(int index);
        public abstract void AsyncLoad();
        public abstract MapChunk GetChunk(int index);

        public abstract bool Intersect(ref Ray ray, out MapChunk chunk, out float distance);

        public abstract bool OnChangeTerrain(Editing.TerrainChangeParameters parameters);
        public abstract void OnUpdateModelPositions(Editing.TerrainChangeParameters parameters);
        public abstract void UpdateNormals();
        public abstract bool OnTextureTerrain(Editing.TextureChangeParameters parameters);
    }
}
