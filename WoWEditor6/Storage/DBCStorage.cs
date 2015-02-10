using WoWEditor6.IO.Files;

namespace WoWEditor6.Storage
{
    static class DbcStorage
    {
        public static DbcFile Map { get; private set; }
        public static DbcFile LoadingScreen { get; private set; }
        public static DbcFile Light { get; private set; }
        public static DbcFile LightData { get;private set;  }
        public static DbcFile LightParams { get;private set;  }
        public static DbcFile ZoneLight { get;private set;  }
        public static DbcFile ZoneLightPoint { get;private set;  }
        public static DbcFile LightIntBand { get;private set;  }
        public static DbcFile LightFloatBand { get;private set;  }

        static DbcStorage()
        {
            Map = new DbcFile();
            LoadingScreen = new DbcFile();
            Light = new DbcFile();
            LightData = new DbcFile();
            LightParams = new DbcFile();
            ZoneLight = new DbcFile();
            ZoneLightPoint = new DbcFile();
            LightIntBand = new DbcFile();
            LightFloatBand = new DbcFile();
        }

        public static void Initialize()
        {
            Map.Load(@"DBFilesClient\Map.dbc");
            LoadingScreen.Load(@"DBFilesClient\LoadingScreens.dbc");
            Light.Load(@"DBFilesClient\Light.dbc");

            if (IO.FileManager.Instance.Version <= IO.FileDataVersion.Mists)
                InitLightsMop();

            if(IO.FileManager.Instance.Version == IO.FileDataVersion.Lichking)
            {
                LightIntBand.Load(@"DBFilesClient\LightIntBand.dbc");
                LightFloatBand.Load(@"DBFilesClient\LightFloatBand.dbc");
            }

            MapFormatGuess.Initialize();
            IO.Files.Sky.SkyManager.Instance.Initialize();
        }

        private static void InitLightsMop()
        {
            LightData.Load(@"DBFilesClient\LightData.dbc");
            LightParams.Load(@"DBFilesClient\LightParams.dbc");
            ZoneLight.Load(@"DBFilesClient\ZoneLight.dbc");
            ZoneLightPoint.Load(@"DBFilesClient\ZoneLightPoint.dbc");
        }
    }
}
