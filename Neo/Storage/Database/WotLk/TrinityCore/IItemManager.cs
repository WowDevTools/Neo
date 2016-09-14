using System.Data;

namespace Neo.Storage.Database.WotLk.TrinityCore
{
    interface IItemManager
    {
        void LoadItem(DataTable pDataTable);
        Item GetItemByEntry(int pEntryId);
    }
}
