using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SharpDX;

namespace Neo.Storage.Database.WotLk.TrinityCore
{
    interface IGameObjectManager
    {
        void LoadGameObjects(DataTable pDataTable);
        void LoadSpawnedGameObjects(DataTable pDataTable, int pMapId);
        GameObject GetGameObjectByEntry(int pEntryId);
        List<SpawnedGameObject> GetSpawnedGameObjectsInRadius(Vector3 pPosition, double pRadius);
    }
}
