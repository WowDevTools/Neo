using System;
using System.Collections.Generic;

namespace Neo.IO.Files.Models
{
	public abstract class WmoRoot : IDisposable
    {
        public string FileName { get; protected set; }

        public abstract Graphics.Texture GetTexture(int index);
        public abstract WmoMaterial GetMaterial(int index);
        public abstract bool Load(string fileName);

        public BoundingBox BoundingBox { get; protected set; }

        public IList<WmoGroup> Groups { get; protected set; }

        public abstract void Dispose();
    }
}
