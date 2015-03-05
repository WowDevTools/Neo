using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using WoWEditor6.IO.Files.Models;
using WoWEditor6.Scene;
using WoWEditor6.Scene.Texture;

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

        protected List<Graphics.Texture> mTextures = new List<Graphics.Texture>(); 

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

        public abstract void AddDoodadInstance(int uuid, string modelName, BoundingBox box, Vector3 position, Vector3 rotation, float scale);

        public int GetFreeM2Uuid()
        {
            if (DoodadInstances.Count >= (1 << 20) - 1)
                return -1;

            var upper = IndexY * 64 + IndexX;
            var lower = DoodadInstances.Count + 1;
            while (DoodadInstances.Any(i => i.Uuid == ((upper << 20) | lower)))
            {
                ++lower;
                if (lower >= (1 << 20) - 1)
                    return -1;
            }

            return (upper << 20) | lower;
        }

        public bool IsUuidAvailable(int uuid)
        {
            return DoodadInstances.All(d => d.Uuid != uuid);
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

            if (mTextures != null)
            {
                mTextures.Clear();
                mTextures = null;
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

        public abstract void AsyncLoad();
        public abstract MapChunk GetChunk(int index);

        public abstract bool Intersect(ref Ray ray, out MapChunk chunk, out float distance);

        public abstract bool OnChangeTerrain(Editing.TerrainChangeParameters parameters);
        public abstract void OnUpdateModelPositions(Editing.TerrainChangeParameters parameters);
        public abstract void UpdateNormals();
        public abstract bool OnTextureTerrain(Editing.TextureChangeParameters parameters);

        public int GetOrAddTexture(string textureName)
        {
            for (var i = 0; i < TextureNames.Count; ++i)
            {
                if (string.Equals(TextureNames[i], textureName, StringComparison.InvariantCultureIgnoreCase))
                    return i;
            }

            TextureNames.Add(textureName);
            mTextures.Add(TextureManager.Instance.GetTexture(textureName));
            return TextureNames.Count - 1;
        }

        public string GetTextureName(int index)
        {
            if (index >= TextureNames.Count)
                throw new IndexOutOfRangeException();

            return TextureNames[index];
        }

        public Graphics.Texture GetTexture(int index)
        {
            if (index >= mTextures.Count)
                throw new IndexOutOfRangeException();

            return mTextures[index];
        }
    }
}
