using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using WoWEditor6.Scene.Models.M2;

namespace WoWEditor6.IO.Files.Models
{
    class M2Instance
    {
        public int Hash { get; set; }
        public int Uuid { get; set; }
        public BoundingBox BoundingBox { get; set; }
        public M2RenderInstance RenderInstance { get; set; }
    }
}
