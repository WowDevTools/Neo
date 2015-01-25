using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.IO.Files.Models
{
    class M2Instance
    {
        public int Hash { get; set; }
        public int Uuid { get; set; }
        public BoundingBox BoundingBox { get; set; }
    }
}
