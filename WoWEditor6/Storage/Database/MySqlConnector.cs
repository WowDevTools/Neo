using System;
using System.Data;
using MySql.Data;

namespace WoWEditor6.Storage.Database
{
    class MySqlConnector : Singleton<MySqlConnector>, IMySqlConnector
    {

        private MySql.Data.MySqlClient.MySqlConnection m_MySqlConn = new MySql.Data.MySqlClient.MySqlConnection();
        private bool m_IsConfigured;

        public string MySqlServer { get; set; }
        public string MySqlUser { get; set; }
        public string MySqlPassword { get; set; }
        public string MySqlDatabase { get; set; }

        public MySqlConnector()
        {
            m_IsConfigured = false;
        }

        public void OpenConnection()
        {
            if (string.IsNullOrEmpty(MySqlServer) || string.IsNullOrEmpty(MySqlUser) || string.IsNullOrEmpty(MySqlPassword) || string.IsNullOrEmpty(MySqlDatabase))
                throw new ArgumentException();

            if (!(m_MySqlConn.State == System.Data.ConnectionState.Open))
                m_MySqlConn.ConnectionString = string.Format("server={0};uid={1};pwd={2};database={3};", MySqlServer, MySqlUser, MySqlPassword, MySqlDatabase);

            m_MySqlConn.Open();

            if (!(m_MySqlConn.State == System.Data.ConnectionState.Open))
                throw new TimeoutException("Can't connect to the server.");
        }

        public void Configuration(string pMySqlServer, string pMySqlUser, string pMySqlPassword, string pMySqlDatabase)
        {
            if (string.IsNullOrEmpty(pMySqlServer) || string.IsNullOrEmpty(pMySqlUser) || string.IsNullOrEmpty(pMySqlPassword) || string.IsNullOrEmpty(pMySqlDatabase))
                throw new ArgumentException();

            MySqlServer = pMySqlServer;
            MySqlUser = pMySqlUser;
            MySqlPassword = pMySqlPassword;
            MySqlDatabase = pMySqlDatabase;
            m_IsConfigured = true;
        }

        public void CloseConnection()
        {
            if (m_MySqlConn.State == ConnectionState.Open)
                m_MySqlConn.Close();
        }

        public DataTable QueryToDataTable(string pQuery)
        {
            if(m_MySqlConn.State == System.Data.ConnectionState.Open)
            {
                DataTable retVal = new DataTable();

                MySql.Data.MySqlClient.MySqlDataAdapter mySqlDataAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter(pQuery, m_MySqlConn);
                mySqlDataAdapter.Fill(retVal);
                mySqlDataAdapter.Dispose();

                return retVal;
            }
            throw new TimeoutException("Can't connect to the server.");
        }
    }
}
