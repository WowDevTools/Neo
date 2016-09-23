
namespace Neo.Graphics
{
    static class DefaultTextures
    {
        public static Texture Color { get; private set; }
        public static Texture Specular { get; private set; }

        public static void Initialize()
        {
            if (Color != null)
                return;

            Color = new Texture();
            Specular = new Texture();
            //Specular.UpdateMemory(1, 1, Format.R8_UNorm, new byte[] {255}, 1);
        }
    }
}
