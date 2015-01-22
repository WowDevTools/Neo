using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.IO.Files.Models
{
    abstract class WmoRoot
    {
        public abstract Graphics.Texture GetTexture(int index);
        public abstract WmoMaterial GetMaterial(int index);
        public abstract bool Load(string fileName);

        public BoundingBox BoundingBox { get; protected set; }

        public IList<WmoGroup> Groups { get; protected set; }
    }
}
