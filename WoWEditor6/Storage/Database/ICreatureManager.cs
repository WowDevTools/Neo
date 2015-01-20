using System;
using System.Data;
using System.Collections.Generic;
using SharpDX;

namespace WoWEditor6.Storage.Database
{
    interface ICreatureManager
    {
        void LoadCreatures(DataTable pDataTable);
        void LoadSpawnedCreatures(DataTable pDataTable, int pMapID);
        Creature GetCreatureByEntry(int pEntryID);
        bool MapAlreadyLoaded(int pMapID);
        List<SpawnedCreature> GetSpawnedCreaturesInRadius(Vector3 pPosition, double pRadius);

    }
}
