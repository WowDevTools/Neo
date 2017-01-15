using System.Data;

namespace Neo.Storage.Database.WotLk.TrinityCore
{
	internal interface IItemManager
    {
        void LoadItem(DataTable pDataTable);
        Item GetItemByEntry(int pEntryId);
    }
}
