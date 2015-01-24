using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO.Files.Models
{
    class M2RenderPass
    {
        public int StartIndex { get; set; }
        public int IndexCount { get; set; }
        public List<Graphics.Texture> Textures { get; set; }
        public uint RenderFlags { get; set; }
        public int TexAnimIndex { get; set; }
        public int TexUnitNumber { get; set; }
        public int ColorAnimIndex { get; set; }
        public int AlphaAnimIndex { get; set; }

        public uint BlendMode => (RenderFlags >> 16);
        public uint RenderFlag => (RenderFlags & 0xFFFF);
    }
}
