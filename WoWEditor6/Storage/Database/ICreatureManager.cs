using System;
using System.Data;
using System.Collections.Generic;
using SharpDX;

namespace WoWEditor6.Storage.Database
{
    interface ICreatureManager
    {
        private List<Creature> mCreatures = new List<Creature>();
        private List<SpawnedCreature> mSpawnedCreatures = new List<SpawnedCreature>();
        private List<int> mLoadedMaps = new List<int>();
        public void LoadCreatures(DataTable pDataTable);
        public void LoadSpawnedCreatures(DataTable pDataTable, int pMapID);
        public Creature GetCreatureByEntry(int pEntryID);
        public bool MapAlreadyLoaded(int pMapID);
        public List<SpawnedCreature> GetSpawnedCreaturesInRadius(Vector3 pPosition, double pRadius);

    }
}
