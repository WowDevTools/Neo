using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.IO.Files.Models
{
    internal class M2SubMeshInfo
    {
        public int StartIndex { get; set; }
        public int NumIndices { get; set; }
        public BoundingSphere BoundingSphere;
    }
}
