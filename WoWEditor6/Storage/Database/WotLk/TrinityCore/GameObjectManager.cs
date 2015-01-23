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
            throw new NotImplementedException();
        }
        public void LoadSpawnedGameObjects(DataTable pDataTable, int pMapId)
        {
            throw new NotImplementedException();
        }
        public List<SpawnedGameObject> GetSpawnedCreaturesInRadius(Vector3 pPosition, double pRadius)
        {
            throw new NotImplementedException();
        }
        public GameObject GetCreatureByEntry(int pEntryId)
        {
            throw new NotImplementedException();
        }
        GameObject GetGameObjectByEntry(int pEntryId)
        {
            throw new NotImplementedException();
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
