using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using OpenTK;

namespace Neo.Storage.Database.WotLk.TrinityCore
{
	internal class GameObjectManager : Singleton<GameObjectManager>, IGameObjectManager
    {
        private readonly List<GameObject> mGameObjects = new List<GameObject>();
        private readonly List<SpawnedGameObject> mSpawnedGameObjects = new List<SpawnedGameObject>();
        private readonly List<int> mLoadedMaps = new List<int>();

        public void LoadGameObjects(DataTable pDataTable)
        {
            //These line avoid sql error when the computer by-default separator is the comma ( like in belgium )
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            foreach (DataRow dRow in pDataTable.Rows)
            {
                var gameobject = new GameObject()
                {
                    EntryId = int.Parse(dRow[0].ToString()),
                    Type = (EnumType)Enum.Parse(typeof(EnumType), dRow[1].ToString()),
                    DisplayId = int.Parse(dRow[2].ToString()),
                    Name = dRow[3].ToString(),
                    IconName = dRow[4].ToString(),
                    CastBarCaption = dRow[5].ToString(),
                    Unknown1 = dRow[6].ToString(),
                    Faction = int.Parse(dRow[7].ToString()),
                    Flags = uint.Parse(dRow[8].ToString()),
                    Size = float.Parse(dRow[9].ToString()),
                    Data0 = int.Parse(dRow[10].ToString()),
                    Data1 = int.Parse(dRow[11].ToString()),
                    Data2 = int.Parse(dRow[12].ToString()),
                    Data3 = int.Parse(dRow[13].ToString()),
                    Data4 = int.Parse(dRow[14].ToString()),
                    Data5 = int.Parse(dRow[15].ToString()),
                    Data6 = int.Parse(dRow[16].ToString()),
                    Data7 = int.Parse(dRow[17].ToString()),
                    Data8 = int.Parse(dRow[18].ToString()),
                    Data9 = int.Parse(dRow[19].ToString()),
                    Data10 = int.Parse(dRow[20].ToString()),
                    Data11 = int.Parse(dRow[21].ToString()),
                    Data12 = int.Parse(dRow[22].ToString()),
                    Data13 = int.Parse(dRow[23].ToString()),
                    Data14 = int.Parse(dRow[24].ToString()),
                    Data15 = int.Parse(dRow[25].ToString()),
                    Data16 = int.Parse(dRow[26].ToString()),
                    Data17 = int.Parse(dRow[27].ToString()),
                    Data18 = int.Parse(dRow[28].ToString()),
                    Data19 = int.Parse(dRow[29].ToString()),
                    Data20 = int.Parse(dRow[30].ToString()),
                    Data21 = int.Parse(dRow[31].ToString()),
                    Data22 = int.Parse(dRow[32].ToString()),
                    Data23 = int.Parse(dRow[33].ToString()),
                    AiName = dRow[34].ToString(),
                    ScriptName = dRow[35].ToString(),
                    VerifiedBuild = int.Parse(dRow[36].ToString())
                };
                mGameObjects.Add(gameobject);
            }
        }

        public void LoadSpawnedGameObjects(DataTable pDataTable, int pMapId)
        {
            if (!MapAlreadyLoaded(pMapId))
            {
                foreach (DataRow dRow in pDataTable.Rows)
                {
                    var position = new Vector3()
                    {
                        X = float.Parse(dRow[9].ToString()),
                        Y = float.Parse(dRow[10].ToString()),
                        Z = float.Parse(dRow[11].ToString())
                    };
                    var gameobject = new SpawnedGameObject()
                    {
                        SpawnGuid = int.Parse(dRow[0].ToString()),
                        GameObject = GetGameObjectByEntry(int.Parse(dRow[1].ToString())),
                        Map = int.Parse(dRow[2].ToString()),
                        ZoneId = int.Parse(dRow[3].ToString()),
                        AreaId = int.Parse(dRow[4].ToString()),
                        SpawnMask = (goSpawnMask)Enum.Parse(typeof(goSpawnMask), dRow[5].ToString()),
                        PhaseMask = int.Parse(dRow[6].ToString()),
                        Position = position,
                        Orientation = float.Parse(dRow[10].ToString()),
                        Rotation0 = float.Parse(dRow[11].ToString()),
                        Rotation1 = float.Parse(dRow[12].ToString()),
                        Rotation2 = float.Parse(dRow[13].ToString()),
                        Rotation3 = float.Parse(dRow[14].ToString()),
                        SpawnTimeSecs = int.Parse(dRow[15].ToString()),
                        AnimProgress = int.Parse(dRow[16].ToString()),
                        State = int.Parse(dRow[17].ToString())
                    };
                    mSpawnedGameObjects.Add(gameobject);
                }
                mLoadedMaps.Add(pMapId);
            }
        }

        public GameObject GetGameObjectByEntry(int pEntryId)
        {
            return mGameObjects.FirstOrDefault(gameobject => gameobject.EntryId == pEntryId);
        }

        public void addGameObject(GameObject gameObject)
        {
            mGameObjects.Add(gameObject);
        }

        private bool MapAlreadyLoaded(int pMapId)
        {
            try
            {
                var mapLoaded = mLoadedMaps.First(map => map == pMapId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<SpawnedGameObject> GetSpawnedGameObjectsInRadius(Vector3 pPosition, double pRadius)
        {
            var camPosXPlus = pPosition.X + pRadius;
            var camPosXMinus = pPosition.X - pRadius;
            var camPosYPlus = pPosition.Y + pRadius;
            var camPosYMinus = pPosition.Y - pRadius;

            var retVal = (from SpawnedGameObject gameobject in mSpawnedGameObjects
                where ((gameobject.Position.X <= camPosXPlus && gameobject.Position.X >= camPosXMinus) && (gameobject.Position.Y <= camPosYPlus && gameobject.Position.Y >= camPosYMinus))
                select gameobject).ToList<SpawnedGameObject>();

            return retVal;
        }
    }
}
