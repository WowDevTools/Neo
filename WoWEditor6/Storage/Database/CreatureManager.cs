using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace WoWEditor6.Storage.Database
{
    class CreatureManager : Singleton<CreatureManager>, ICreatureManager
    {
        private List<Creature> m_Creatures = new List<Creature>();
        private List<SpawnedCreature> m_SpawnedCreatures = new List<SpawnedCreature>();
        private List<int> m_LoadedMaps = new List<int>();

        public void LoadCreatures(DataTable pDataTable)
        {
            foreach (DataRow dRow in pDataTable.Rows)
            {
                Creature creature = new Creature();
                foreach (DataColumn dColumn in pDataTable.Columns)
                    switch (dColumn.ColumnName)
                    {
                        case "entry":
                            creature.EntryID = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "name":
                            creature.Name = dRow[dColumn].ToString();
                            break;
                        case "scale":
                            creature.Scale = float.Parse(dRow[dColumn].ToString());
                            break;
                        case "modelid1":
                            creature.ModelID1 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "modelid2":
                            creature.ModelID2 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "modelid3":
                            creature.ModelID3 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "modelid4":
                            creature.ModelID4 = int.Parse(dRow[dColumn].ToString());
                            break;
                    }
                m_Creatures.Add(creature);
            }
        }

        public void LoadSpawnedCreatures(DataTable pDataTable, int pMapID)
        {
            if (!MapAlreadyLoaded(pMapID))
            {
                foreach (DataRow dRow in pDataTable.Rows)
                {
                    SpawnedCreature creature = new SpawnedCreature();
                    foreach (DataColumn dColumn in pDataTable.Columns)
                        switch (dColumn.ColumnName)
                        {
                            case "guid":
                                creature.SpawnGUID = int.Parse(dRow[dColumn].ToString());
                                break;
                            case "id":
                                creature.Creature = GetCreatureByEntry(int.Parse(dRow[dColumn].ToString()));
                                break;
                            case "map":
                                creature.Map = int.Parse(dRow[dColumn].ToString());
                                break;
                            case "spawnMask":
                                creature.SpawnMask = (SpawnMask)Enum.Parse(typeof(SpawnMask), dRow[dColumn].ToString());
                                break;
                            case "phaseMask":
                                creature.PhaseMask = int.Parse(dRow[dColumn].ToString());
                                break;
                            case "modelid":
                                creature.ModellID = int.Parse(dRow[dColumn].ToString());
                                break;
                            case "position_x":
                                creature.X = float.Parse(dRow[dColumn].ToString());
                                break;
                            case "position_y":
                                creature.Y = float.Parse(dRow[dColumn].ToString());
                                break;
                            case "position_z":
                                creature.Z = float.Parse(dRow[dColumn].ToString());
                                break;
                            case "orientation":
                                creature.Orientation = float.Parse(dRow[dColumn].ToString());
                                break;
                        }
                    m_SpawnedCreatures.Add(creature);
                }
                m_LoadedMaps.Add(pMapID);
            }
        }

        public Creature GetCreatureByEntry(int pEntryID)
        {
            Creature retVal = m_Creatures.First(creature => creature.EntryID == pEntryID);
            return retVal;
        }

        public bool MapAlreadyLoaded(int pMapID)
        {
            try
            {
                int mapLoaded = m_LoadedMaps.First(map => map == pMapID);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
