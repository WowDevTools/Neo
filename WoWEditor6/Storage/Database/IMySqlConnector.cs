using System;
using System.Data;
using MySql.Data;

namespace WoWEditor6.Storage.Database
{
    interface IMySqlConnector
    {
        private MySql.Data.MySqlClient.MySqlConnection m_MySqlConn;
        private bool mIsConfigured;
        public string MySqlServer { get; set; }
        public string MySqlUser { get; set; }
        public string MySqlPassword { get; set; }
        public string MySqlDatabase { get; set; }
        public void OpenConnection();
        public void Configuration(string pMySqlServer, string pMySqlUser, string pMySqlPassword, string pMySqlDatabase);
        public void CloseConnection();
        public DataTable QueryToDataTable(string pQuery);
    }
}
