using System;
using System.Data;
using System.Windows.Forms;

namespace Neo.Storage.Database
{
	internal class MySqlConnector : Singleton<MySqlConnector>, IMySqlConnector
    {

        private readonly MySql.Data.MySqlClient.MySqlConnection mMySqlConn = new MySql.Data.MySqlClient.MySqlConnection();

        public string MySqlServer { get; set; }
        public string MySqlUser { get; set; }
        public string MySqlPassword { get; set; }
        public string MySqlDatabase { get; set; }

        public MySqlConnector()
        {

        }

        public void OpenConnection()
        {
            if (string.IsNullOrEmpty(this.MySqlServer) || string.IsNullOrEmpty(this.MySqlUser) || string.IsNullOrEmpty(this.MySqlDatabase))
            {
	            throw new ArgumentException();
            }

	        if (this.mMySqlConn.State != ConnectionState.Open)
            {
	            this.mMySqlConn.ConnectionString = string.Format("server={0};uid={1};pwd={2};database={3};", this.MySqlServer, this.MySqlUser, this.MySqlPassword, this.MySqlDatabase);

                try
                {
	                this.mMySqlConn.Open();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
	                Console.WriteLine(ex.Message);
                }
            }
            else
            {
	            Console.WriteLine("The connection is already open.");
            }

            if (this.mMySqlConn.State != ConnectionState.Open)
            {
	            throw new TimeoutException("Can't connect to the server.");
            }
        }

        public void Configuration(string pMySqlServer, string pMySqlUser, string pMySqlPassword, string pMySqlDatabase)
        {
            if (string.IsNullOrEmpty(pMySqlServer) || string.IsNullOrEmpty(pMySqlUser) || string.IsNullOrEmpty(pMySqlDatabase))
            {
	            throw new ArgumentException();
            }

	        this.MySqlServer = pMySqlServer;
	        this.MySqlUser = pMySqlUser;
	        this.MySqlPassword = pMySqlPassword;
	        this.MySqlDatabase = pMySqlDatabase;
        }

        public bool CheckConnection()
        {
            if (this.mMySqlConn.State == ConnectionState.Open)
            {
	            return true;
            }
	        return false;
        }

        public void CloseConnection()
        {
            if (this.mMySqlConn.State == ConnectionState.Open)
            {
	            this.mMySqlConn.Close();
            }
        }

        public DataTable QueryToDataTable(string pQuery)
        {
            if(this.mMySqlConn.State == ConnectionState.Open)
            {
                DataTable retVal = new DataTable();

                MySql.Data.MySqlClient.MySqlDataAdapter mySqlDataAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter(pQuery, this.mMySqlConn);
                mySqlDataAdapter.Fill(retVal);
                mySqlDataAdapter.Dispose();

                return retVal;
            }
            throw new TimeoutException("Can't connect to the server.");
        }

        public bool Query(string pQuery)
        {
            if(this.mMySqlConn.State == ConnectionState.Open)
            {
                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(pQuery, this.mMySqlConn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
	                Console.WriteLine(ex.Message);
                }
                return true;
            }
            return false;
        }
    }
}
