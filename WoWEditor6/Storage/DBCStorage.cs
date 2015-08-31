using System;
using WoWEditor6.IO;
using WoWEditor6.IO.Files;
using WoWEditor6.IO.Files.Sky;

namespace WoWEditor6.Storage
{
    static class DbcStorage
    {
        public static IDataStorageFile Map { get; private set; }
        public static IDataStorageFile LoadingScreen { get; private set; }
        public static IDataStorageFile Light { get; private set; }
        public static IDataStorageFile LightData { get;private set;  }
        public static IDataStorageFile LightParams { get;private set;  }
        public static IDataStorageFile ZoneLight { get;private set;  }
        public static IDataStorageFile ZoneLightPoint { get;private set;  }
        public static IDataStorageFile LightIntBand { get;private set;  }
        public static IDataStorageFile LightFloatBand { get;private set;  }
        public static IDataStorageFile CreatureDisplayInfo { get; private set; }
        public static IDataStorageFile CreatureModelData { get; private set; }
        public static IDataStorageFile FileData { get; private set; }
        public static IDataStorageFile GroundEffectTexture { get; private set; }
        public static IDataStorageFile GroundEffectDoodad { get; private set; }

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
            CreatureDisplayInfo = new DbcFile();
            CreatureModelData = new DbcFile();
            FileData = new DbcFile();
            GroundEffectDoodad = new DbcFile();
            GroundEffectTexture = new DbcFile();
        }

        public static void Initialize()
        {
            Map.Load(@"DBFilesClient\Map.dbc");
            LoadingScreen.Load(@"DBFilesClient\LoadingScreens.dbc");
            Light.Load(@"DBFilesClient\Light.dbc");
            try
            {
                CreatureDisplayInfo.Load(@"DBFilesClient\CreatureDisplayInfo.dbc");
            }
            catch (Exception)
            {
                if (FileManager.Instance.Version < FileDataVersion.Warlords)
                    throw;

                CreatureDisplayInfo = new DB2File();
                CreatureDisplayInfo.Load(@"DBFilesClient\CreatureDisplayInfo.db2");
            }

            CreatureModelData.Load(@"DBFilesClient\CreatureModelData.dbc");

            if (FileManager.Instance.Version <= FileDataVersion.Mists)
                InitLightsMop();

            if(FileManager.Instance.Version == FileDataVersion.Lichking)
            {
                LightIntBand.Load(@"DBFilesClient\LightIntBand.dbc");
                LightFloatBand.Load(@"DBFilesClient\LightFloatBand.dbc");
            }

            if(FileManager.Instance.Version <= FileDataVersion.Warlords)
                FileData.Load(@"DBFilesClient\FileData.dbc");

            GroundEffectDoodad.Load(@"DBFilesClient\GroundEffectDoodad.dbc");
            GroundEffectTexture.Load(@"DBFilesClient\GroundEffectTexture.dbc");

            MapFormatGuess.Initialize();
            SkyManager.Instance.Initialize();
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
