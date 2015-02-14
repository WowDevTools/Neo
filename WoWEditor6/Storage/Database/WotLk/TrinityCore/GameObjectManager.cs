using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SharpDX;

namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    class GameObjectManager : IGameObjectManager
    {
        private readonly List<GameObject> mGameObjects = new List<GameObject>();
        private readonly List<SpawnedGameObject> mSpawnedGameObjects = new List<SpawnedGameObject>();
        private readonly List<int> mLoadedMaps = new List<int>();

        public void LoadGameObjects(DataTable pDataTable)
        {
            foreach (DataRow dRow in pDataTable.Rows)
            {
                var gameobject = new GameObject();
                gameobject.EntryId = int.Parse(dRow[0].ToString());
                gameobject.Type = (EnumType)Enum.Parse(typeof(EnumType), dRow[1].ToString());
                gameobject.DisplayId = int.Parse(dRow[2].ToString());
                gameobject.Name = dRow[3].ToString();
                gameobject.IconName = dRow[4].ToString();
                gameobject.CastBarCaption = dRow[5].ToString();
                gameobject.Unknown1 = dRow[6].ToString();
                gameobject.Faction = int.Parse(dRow[7].ToString());
                gameobject.Flags = (Flags)Enum.Parse(typeof(Flags), dRow[8].ToString());
                gameobject.Size = float.Parse(dRow[9].ToString());
                gameobject.QuestItem1 = int.Parse(dRow[10].ToString());
                gameobject.QuestItem2 = int.Parse(dRow[11].ToString());
                gameobject.QuestItem3 = int.Parse(dRow[12].ToString());
                gameobject.QuestItem4 = int.Parse(dRow[13].ToString());
                gameobject.QuestItem5 = int.Parse(dRow[14].ToString());
                gameobject.QuestItem6 = int.Parse(dRow[15].ToString());
                gameobject.Data0 = int.Parse(dRow[16].ToString());
                gameobject.Data1 = int.Parse(dRow[17].ToString());
                gameobject.Data2 = int.Parse(dRow[18].ToString());
                gameobject.Data3 = int.Parse(dRow[19].ToString());
                gameobject.Data4 = int.Parse(dRow[20].ToString());
                gameobject.Data5 = int.Parse(dRow[21].ToString());
                gameobject.Data6 = int.Parse(dRow[22].ToString());
                gameobject.Data7 = int.Parse(dRow[23].ToString());
                gameobject.Data8 = int.Parse(dRow[24].ToString());
                gameobject.Data9 = int.Parse(dRow[25].ToString());
                gameobject.Data10 = int.Parse(dRow[26].ToString());
                gameobject.Data11 = int.Parse(dRow[27].ToString());
                gameobject.Data12 = int.Parse(dRow[28].ToString());
                gameobject.Data13 = int.Parse(dRow[29].ToString());
                gameobject.Data14 = int.Parse(dRow[30].ToString());
                gameobject.Data15 = int.Parse(dRow[31].ToString());
                gameobject.Data16 = int.Parse(dRow[32].ToString());
                gameobject.Data17 = int.Parse(dRow[33].ToString());
                gameobject.Data18 = int.Parse(dRow[34].ToString());
                gameobject.Data19 = int.Parse(dRow[35].ToString());
                gameobject.Data20 = int.Parse(dRow[36].ToString());
                gameobject.Data21 = int.Parse(dRow[37].ToString());
                gameobject.Data22 = int.Parse(dRow[38].ToString());
                gameobject.Data23 = int.Parse(dRow[39].ToString());
                gameobject.AiName = dRow[40].ToString();
                gameobject.ScriptName = dRow[41].ToString();
                gameobject.VerifiedBuild = int.Parse(dRow[42].ToString());
                mGameObjects.Add(gameobject);
            }
        }

        public void LoadSpawnedGameObjects(DataTable pDataTable, int pMapId)
        {
            if (!MapAlreadyLoaded(pMapId))
            {
                foreach (DataRow dRow in pDataTable.Rows)
                {
                    var gameobject = new SpawnedGameObject();
                    var position = new Vector3();
                    gameobject.SpawnGuid = int.Parse(dRow[0].ToString());
                    gameobject.GameObject = GetGameObjectByEntry(int.Parse(dRow[1].ToString()));
                    gameobject.Map = int.Parse(dRow[2].ToString());
                    gameobject.ZoneId = int.Parse(dRow[3].ToString());
                    gameobject.AreaId = int.Parse(dRow[4].ToString());
                    gameobject.SpawnMask = (goSpawnMask)Enum.Parse(typeof(goSpawnMask), dRow[5].ToString());
                    gameobject.PhaseMask = int.Parse(dRow[6].ToString());
                    position.X = float.Parse(dRow[7].ToString());
                    position.Y = float.Parse(dRow[8].ToString());
                    position.Z = float.Parse(dRow[9].ToString());
                    gameobject.Position = position;
                    gameobject.Orientation = float.Parse(dRow[10].ToString());
                    gameobject.Rotation0 = float.Parse(dRow[11].ToString());
                    gameobject.Rotation1 = float.Parse(dRow[12].ToString());
                    gameobject.Rotation2 = float.Parse(dRow[13].ToString());
                    gameobject.Rotation3 = float.Parse(dRow[14].ToString());
                    gameobject.SpawnTimeSecs = int.Parse(dRow[15].ToString());
                    gameobject.AnimProgress = int.Parse(dRow[16].ToString());
                    gameobject.State = int.Parse(dRow[17].ToString());
                    mSpawnedGameObjects.Add(gameobject);
                }
                mLoadedMaps.Add(pMapId);
            }
        }

        public GameObject GetGameObjectByEntry(int pEntryId)
        {
            return mGameObjects.First(gameobject => gameobject.EntryId == pEntryId);
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
