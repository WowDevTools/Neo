using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    class GameObject : IGameObject
    {
        public int EntryId { get; set; }
        public EnumType Type { get; set; }
        public int DisplayId { get; set; }
        public string Name { get; set; }
        public string IconName { get; set; }
        public string CastBarCaption { get; set; }
        public string Unknown1 { get; set; }
        public int Faction { get; set; }
        public Flags Flags { get; set; }
        public float Size { get; set; }
        public int QuestItem1 { get; set; }
        public int QuestItem2 { get; set; }
        public int QuestItem3 { get; set; }
        public int QuestItem4 { get; set; }
        public int QuestItem5 { get; set; }
        public int QuestItem6 { get; set; }
        public int Data0 { get; set; }
        public int Data1 { get; set; }
        public int Data2 { get; set; }
        public int Data3 { get; set; }
        public int Data4 { get; set; }
        public int Data5 { get; set; }
        public int Data6 { get; set; }
        public int Data7 { get; set; }
        public int Data8 { get; set; }
        public int Data9 { get; set; }
        public int Data10 { get; set; }
        public int Data11 { get; set; }
        public int Data12 { get; set; }
        public int Data13 { get; set; }
        public int Data14 { get; set; }
        public int Data15 { get; set; }
        public int Data16 { get; set; }
        public int Data17 { get; set; }
        public int Data18 { get; set; }
        public int Data19 { get; set; }
        public int Data20 { get; set; }
        public int Data21 { get; set; }
        public int Data22 { get; set; }
        public int Data23 { get; set; }
        public string AiName { get; set; }
        public string ScriptName { get; set; }
        public int VerifiedBuild { get; set; }

        public string GetUpdateSqlQuery()
        {
            throw new NotImplementedException();
        }

        public string GetInsertSqlQuery()
        {
            throw new NotImplementedException();
        }
    }
}
