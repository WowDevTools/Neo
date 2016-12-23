using System.Collections.Generic;
using System.Data;
using OpenTK;

namespace Neo.Storage.Database.WotLk.TrinityCore
{
	internal interface IGameObjectManager
    {
        void LoadGameObjects(DataTable pDataTable);
        void LoadSpawnedGameObjects(DataTable pDataTable, int pMapId);
        GameObject GetGameObjectByEntry(int pEntryId);
        List<SpawnedGameObject> GetSpawnedGameObjectsInRadius(Vector3 pPosition, double pRadius);
    }
}
