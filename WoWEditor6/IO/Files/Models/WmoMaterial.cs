using System.Collections.Generic;

namespace WoWEditor6.IO.Files.Models
{
    class WmoMaterial
    {
        private readonly int mTexture1;
        private readonly int mTexture2;
        private readonly int mTexture3;

        public List<Graphics.Texture> Textures { get; } = new List<Graphics.Texture>();
        public int ShaderType { get; }
        public int BlendMode { get; }
        public uint Flags1 { get; }

        public WmoMaterial(WmoRoot root, int shader, int texture1, int texture2, int texture3, int blendMode, uint flags)
        {
            mTexture1 = texture1;
            mTexture2 = texture2;
            mTexture3 = texture3;
            ShaderType = shader;
            BlendMode = blendMode;
            Flags1 = flags;
            LoadTextures(root);
        }

        private void LoadTextures(WmoRoot root)
        {
            switch (ShaderType)
            {
                case 11:
                case 12:
                case 7:
                    Textures.Add(root.GetTexture(mTexture1));
                    Textures.Add(root.GetTexture(mTexture2));
                    Textures.Add(root.GetTexture(mTexture3));
                    break;

                case 5:
                case 6:
                case 8:
                case 9:
                case 13:
                case 15:
                case 3:
                    Textures.Add(root.GetTexture(mTexture1));
                    Textures.Add(root.GetTexture(mTexture2));
                    break;

                default:
                    Textures.Add(root.GetTexture(mTexture1));
                    break;
            }
        }
    }
}
