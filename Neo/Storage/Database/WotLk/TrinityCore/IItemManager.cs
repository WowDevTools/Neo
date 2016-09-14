using System.Data;

namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    interface IItemManager
    {
        void LoadItem(DataTable pDataTable);
        Item GetItemByEntry(int pEntryId);
    }
}
