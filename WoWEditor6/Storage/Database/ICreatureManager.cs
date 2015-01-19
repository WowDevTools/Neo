using System;
using System.Data;
using System.Collections.Generic;

namespace WoWEditor6.Storage.Database
{
    interface ICreatureManager
    {
        private List<Creature> m_Creatures = new List<Creature>();
        private List<SpawnedCreature> m_SpawnedCreatures = new List<SpawnedCreature>();
        private List<int> m_LoadedMaps = new List<int>();
        public void LoadCreatures(DataTable pDataTable);
        public void LoadSpawnedCreatures(DataTable pDataTable, int pMapID);
        public Creature GetCreatureByEntry(int pEntryID);
        public bool MapAlreadyLoaded(int pMapID);
    }
}
