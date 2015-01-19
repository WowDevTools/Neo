using SharpDX;

namespace WoWEditor6.Scene.Terrain
{
    class MapManager
    {
        private Vector2 mEntryPoint;

        public string Continent { get; private set; }
        public int MapId { get; private set; }
        public IO.Files.Terrain.WoD.WdtFile CurrentWdt { get; private set; }
        public bool HasNewBlend { get; private set; }

        public void Initialize()
        {

        }

        public void EnterWorld(Vector2 entryPoint, int mapId, string continent)
        {
            mEntryPoint = entryPoint;
            MapId = mapId;
            Continent = continent;
            WorldFrame.Instance.State = AppState.LoadingScreen;

            CurrentWdt = new IO.Files.Terrain.WoD.WdtFile();
            CurrentWdt.Load(continent);
            HasNewBlend = (CurrentWdt.Flags & 0x84) != 0;
        }
    }
}
