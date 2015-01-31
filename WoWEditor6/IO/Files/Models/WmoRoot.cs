using System;
using System.Collections.Generic;
using SharpDX;

namespace WoWEditor6.IO.Files.Models
{
    abstract class WmoRoot : IDisposable
    {
        public abstract Graphics.Texture GetTexture(int index);
        public abstract WmoMaterial GetMaterial(int index);
        public abstract bool Load(string fileName);

        public BoundingBox BoundingBox { get; protected set; }

        public IList<WmoGroup> Groups { get; protected set; }

        public abstract void Dispose();
    }
}
