using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    public class GameObject : IGameObject
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
            return "UPDATE gameobject_template SET type = '" + this.Type + "', displayId = '" + this.DisplayId + "', name = '" + this.Name + "', IconName = '" + this.IconName + "', castBarCaption = '" + this.CastBarCaption + "', unk1 = '" + this.Unknown1 + "', faction = '" + this.Faction + "', flags = '" + this.Flags + "', size = '" + this.Size + "', questItem1 = '" + this.QuestItem1 + "', questItem2 = '" + this.QuestItem2 + "', questItem3 = '" + this.QuestItem3 + "', questItem4 = '" + this.QuestItem4 + "', questItem5 = '" + this.QuestItem5 + "', questItem6 = '" + this.QuestItem6 + "', data0 = '" + this.Data0 + "', data1 = '" + this.Data1 + "', data2 = '" + this.Data2 + "', data3 = '" + this.Data3 + "', data4 = '" + this.Data4 + "', data5 = '" + this.Data5 + "', data6 = '" + this.Data6 + "', data7 = '" + this.Data7 + "', data8 = '" + this.Data8 + "', data9 = '" + this.Data9 + "', data10 = '" + this.Data10 + "', data11 = '" + this.Data11 + "', data12 = '" + this.Data12 + "', data13 = '" + this.Data13 + "', data14 = '" + this.Data14 + "', data15 = '" + this.Data15 + "', data16 = '" + this.Data16 + "', data17 = '" + this.Data17 + "', data18 = '" + this.Data18 + "', data19 = '" + this.Data19 + "', data20 = '" + this.Data20 + "', data21 = '" + this.Data21 + "', data22 = '" + this.Data22 + "', data23 = '" + this.Data23 + "', AIName = '" + this.AiName + "', ScriptName = '" + this.ScriptName + "', VerifiedBuild = '" + this.VerifiedBuild + "' WHERE entry = '" + this.EntryId + "';";
        }

        public string GetInsertSqlQuery()
        {
            return "INSERT INTO creature_template VALUES ('" + this.EntryId + "', '"  + this.Type +  "', '" + this.DisplayId + "', '" + this.Name + "', '" + this.IconName + "', '" + this.CastBarCaption + "', '" + this.Unknown1 + "', '" + this.Faction + "', '" + this.Flags + "', '" + this.Size + "', '" + this.QuestItem1 + "', '" + this.QuestItem2 + "', '" + this.QuestItem3 + "', '" + this.QuestItem4 + "', '" + this.QuestItem5 + "', '" + this.QuestItem6 + "', '" + this.Data0 + "', '" + this.Data1 + "', '" + this.Data2 + "', '" + this.Data3 + "', '" + this.Data4 + "', '" + this.Data5 + "', '" + this.Data6 + "', '" + this.Data7 + "', '" + this.Data8 + "', '" + this.Data9 + "', '" + this.Data10 + "', '" + this.Data11 + "', '" + this.Data12 + "', '" + this.Data13 + "', '" + this.Data14 + "', '" + this.Data15 + "', '" + this.Data16 + "', '" + this.Data17 + "', '" + this.Data18 + "', '" + this.Data19 + "', '" + this.Data20 + "', '" + this.Data21 + "', '" + this.Data22 + "', '" + this.Data23 + "', '" + this.AiName + "', '" + this.ScriptName + "', '" + this.ScriptName + "', '" + this.VerifiedBuild + "');";
        }
    }
}
