using System.Data;
using System.Collections.Generic;
using SharpDX;

namespace Neo.Storage.Database.WotLk.TrinityCore
{
    interface ICreatureManager
    {
        void LoadCreatures(DataTable pDataTable);
        void LoadSpawnedCreatures(DataTable pDataTable, int pMapId);
        Creature GetCreatureByEntry(int pEntryId);
        List<SpawnedCreature> GetSpawnedCreaturesInRadius(Vector3 pPosition, double pRadius);

    }
}
