
namespace Neo.IO.Files.Models
{
	public class WmoMaterial
    {
        private readonly int mTexture1;
        private readonly int mTexture2;
        private readonly int mTexture3;

        public Graphics.Texture[] Textures { get; private set; }
        public int ShaderType { get; private set; }
        public int BlendMode { get; private set; }
        public uint Flags1 { get; private set; }
        public uint MaterialFlags { get; private set; }

        public WmoMaterial(WmoRoot root, int shader, int texture1, int texture2, int texture3, int blendMode, uint flags, uint materialFlags)
        {
	        this.Textures = new Graphics.Texture[0];
	        this.MaterialFlags = materialFlags;
	        this.mTexture1 = texture1;
	        this.mTexture2 = texture2;
	        this.mTexture3 = texture3;
	        this.ShaderType = shader;
	        this.BlendMode = blendMode;
	        this.Flags1 = flags;
            LoadTextures(root);
        }

        private void LoadTextures(WmoRoot root)
        {
            switch (this.ShaderType)
            {
                case 11:
                case 12:
                case 7:
	                this.Textures = new[]
                    {
                        root.GetTexture(this.mTexture1),
                        root.GetTexture(this.mTexture2),
                        root.GetTexture(this.mTexture3)
                    };
                    break;

                case 5:
                case 6:
                case 8:
                case 9:
                case 13:
                case 15:
                case 3:
	                this.Textures = new[]
                    {
                        root.GetTexture(this.mTexture1),
                        root.GetTexture(this.mTexture2),
                    };
                    break;

                default:
	                this.Textures = new[]
                    {
                        root.GetTexture(this.mTexture1),
                    };
                    break;
            }
        }
    }
}
