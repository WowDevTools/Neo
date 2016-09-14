using System;

namespace WoWEditor6.IO.Files.Terrain
{
    class AdtFactory
    {
        public static AdtFactory Instance { get; private set; }

        public FileDataVersion Version { get; set; }

        static AdtFactory()
        {
            Instance = new AdtFactory();
        }

        public MapArea CreateArea(string continent, int ix, int iy)
        {
            switch (Version)
            {
                case FileDataVersion.Warlords:
                    return new WoD.MapArea(continent, ix, iy);

                case FileDataVersion.Lichking:
                    return new Wotlk.MapArea(continent, ix, iy);

                default:
                    throw new NotImplementedException("Version not yet supported");
            }
        }
    }
}
