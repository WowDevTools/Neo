
namespace Neo.IO.Files.Terrain
{
	public class TerrainQueryManager
    {
        private IMapAreaQueryImplementation mMapAreaImplementation;

        public static TerrainQueryManager Instance { get; private set; }

        static TerrainQueryManager()
        {
            Instance = new TerrainQueryManager();
        }

        private TerrainQueryManager()
        {
            FileManager.Instance.LoadComplete += Initialize;
        }

        public void Execute(MapAreaQuery query)
        {
	        this.mMapAreaImplementation.Execute(query);
        }

        private void Initialize()
        {
            switch (FileManager.Instance.Version)
            {
                case FileDataVersion.Lichking:
	                this.mMapAreaImplementation = new Wotlk.MapQueryImpl();
                    break;
            }
        }
    }
}
