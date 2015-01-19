using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.Storage.Database
{
    class Creature : ICreature
    {
        public int EntryID { get; set; }
        public int ModelID1 { get; set; }
        public int ModelID2 { get; set; }
        public int ModelID3 { get; set; }
        public int ModelID4 { get; set; }
        public string Name { get; set; }
        public float Scale { get; set; }
    }
}
