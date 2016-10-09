using System.Collections.Generic;
using Neo.Scene.Models.M2;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Neo.IO.Files.Models
{
	public class M2RenderPass
    {
        public int StartIndex { get; set; }
        public int IndexCount { get; set; }
        public List<Graphics.Texture> Textures { get; set; }
        public List<int> TextureIndices { get; set; }
        public int TexAnimIndex { get; set; }
        public int TexUnitNumber { get; set; }
        public int ColorAnimIndex { get; set; }
        public int AlphaAnimIndex { get; set; }

        public uint RenderFlag { get; set; }
        public uint BlendMode { get; set; }

        public uint OpCount { get; set; }
        public M2VertexShaderType VertexShaderType { get; set; }
        public M2HullShaderType HullShaderType { get; set; }
        public M2DomainShaderType DomainShaderType { get; set; }
        public M2FragmentShaderType PixelShaderType { get; set; }
    }
}
