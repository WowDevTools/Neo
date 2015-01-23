using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SharpDX;

namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    interface IGameObjectManager
    {
        void LoadGameObjects(DataTable pDataTable);
        void LoadSpawnedGameObjects(DataTable pDataTable, int pMapId);
        GameObject GetCreatureByEntry(int pEntryId);
        List<SpawnedGameObject> GetSpawnedCreaturesInRadius(Vector3 pPosition, double pRadius);
    }
}
