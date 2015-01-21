using System.Data;
using System.Collections.Generic;
using SharpDX;

namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    interface ICreatureManager
    {
        void LoadCreatures(DataTable pDataTable);
        void LoadSpawnedCreatures(DataTable pDataTable, int pMapId);
        Creature GetCreatureByEntry(int pEntryId);
        bool MapAlreadyLoaded(int pMapId);
        List<SpawnedCreature> GetSpawnedCreaturesInRadius(Vector3 pPosition, double pRadius);

    }
}
