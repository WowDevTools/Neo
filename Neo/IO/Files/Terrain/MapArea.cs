using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neo.Graphics;
using Neo.IO.Files.Models;
using Neo.Scene;
using Neo.Scene.Texture;
using OpenTK;
using SlimTK;
using Warcraft.Core;

namespace Neo.IO.Files.Terrain
{
	public abstract class MapArea : IDisposable
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
        private List<Graphics.Texture> mSpecularTextures = new List<Graphics.Texture>();

        protected MapArea()
        {
	        this.IsValid = true;
	        this.DoodadInstances = new List<M2Instance>();
	        this.FullVertices = new AdtVertex[145 * 256];
	        this.TextureNames = new List<string>();
        }

        ~MapArea()
        {
            Dispose(false);
        }

        public abstract void AddDoodadInstance(int uuid, string modelName, BoundingBox boundingBox, Vector3 position, Vector3 rotation, float scale);

        public int GetFreeM2Uuid()
        {
            if (this.DoodadInstances.Count >= (1 << 20) - 1)
            {
	            return -1;
            }

	        var upper = this.IndexY * 64 + this.IndexX;
            var lower = this.DoodadInstances.Count + 1;
            while (this.DoodadInstances.Any(i => i.Uuid == ((upper << 20) | lower)))
            {
                ++lower;
                if (lower >= (1 << 20) - 1)
                {
	                return -1;
                }
            }

            return (upper << 20) | lower;
        }

        public bool IsUuidAvailable(int uuid)
        {
            return this.DoodadInstances.All(d => d.Uuid != uuid);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.DoodadInstances != null)
            {
                foreach (var instance in this.DoodadInstances)
                {
	                WorldFrame.Instance.M2Manager.RemoveInstance(instance.Hash, instance.Uuid);
                }

	            this.DoodadInstances.Clear();
	            this.DoodadInstances = null;
            }

            if (this.TextureNames != null)
            {
	            this.TextureNames.Clear();
	            this.TextureNames = null;
            }

            if (this.mTextures != null)
            {
	            this.mTextures.Clear();
	            this.mTextures = null;
            }

	        this.FullVertices = null;
	        this.Continent = null;
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
            for (var i = 0; i < this.TextureNames.Count; ++i)
            {
                if (string.Equals(this.TextureNames[i], textureName, StringComparison.InvariantCultureIgnoreCase))
                {
	                return i;
                }
            }

	        this.TextureNames.Add(textureName);

            var specTex = Path.ChangeExtension(textureName, null) + "_s.blp";
            if (FileManager.Instance.Provider.Exists(specTex) == false)
            {
	            this.mSpecularTextures.Add(DefaultTextures.Specular);
            }
            else
            {
	            this.mSpecularTextures.Add(TextureManager.Instance.GetTexture(specTex));
            }

	        this.mTextures.Add(TextureManager.Instance.GetTexture(textureName));
            return this.TextureNames.Count - 1;
        }

        public string GetTextureName(int index)
        {
            if (index >= this.TextureNames.Count)
            {
	            throw new IndexOutOfRangeException();
            }

	        return this.TextureNames[index];
        }

        public Graphics.Texture GetTexture(int index)
        {
            if (index >= this.mTextures.Count)
            {
	            throw new IndexOutOfRangeException();
            }

	        return this.mTextures[index];
        }

        public Graphics.Texture GetSpecularTexture(int index)
        {
            if(index >= this.mSpecularTextures.Count)
            {
	            throw new IndexOutOfRangeException();
            }

	        return this.mSpecularTextures[index];
        }

        protected void LoadSpecularTextures()
        {
	        this.mSpecularTextures = new List<Graphics.Texture>();
            foreach (var tex in this.TextureNames)
            {
                var specTex = Path.ChangeExtension(tex, null) + "_s.blp";
                if (FileManager.Instance.Provider.Exists(specTex) == false)
                {
	                this.mSpecularTextures.Add(DefaultTextures.Specular);
                }
                else
                {
	                this.mSpecularTextures.Add(TextureManager.Instance.GetTexture(specTex));
                }
            }
        }

        public bool IsSpecularTextureLoaded(int index)
        {
            var tex = GetSpecularTexture(index);
            return tex != DefaultTextures.Specular;
        }

        public abstract void SetChanged();
    }
}
