using System.Data;

namespace Neo.Storage.Database
{
    interface IMySqlConnector
    {
        string MySqlServer { get; set; }
        string MySqlUser { get; set; }
        string MySqlPassword { get; set; }
        string MySqlDatabase { get; set; }
        void OpenConnection();
        void Configuration(string pMySqlServer, string pMySqlUser, string pMySqlPassword, string pMySqlDatabase);
        bool CheckConnection();
        void CloseConnection();
        DataTable QueryToDataTable(string pQuery);
    }
}
