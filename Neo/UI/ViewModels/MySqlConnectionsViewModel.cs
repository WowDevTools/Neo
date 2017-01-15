using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Neo.Storage.Database;
using Neo.Storage.Database.WotLk.TrinityCore;
using Neo.UI.Models;
using Neo.UI.Services;

namespace Neo.UI.ViewModels
{
	internal class MySqlConnectionsViewModel : BindableBase
    {
        public ICommand LoginCommand { get; private set; }
        public ICommand SaveConnectionCommand { get; private set; }
        public ICommand NewConnectionCommand { get; private set; }
        public ICommand DeleteConnectionCommand { get; private set; }
        public ICommand SelectionChangedCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public InteractionRequest<INotification> NotificationRequest { get; private set; }
        public InteractionRequest<SaveAsNotification> SaveAsRequest { get; private set; }

        private string SaveAs { get; set; }

        public MySqlConnectionsViewModel()
        {
	        this.LoginCommand = new DelegateCommand(Login);
	        this.SaveConnectionCommand = new DelegateCommand(SaveConnection);
	        this.NewConnectionCommand = new DelegateCommand(NewConnection);
	        this.DeleteConnectionCommand = new DelegateCommand(DeleteConnection);
	        this.SelectionChangedCommand = new DelegateCommand(SelectionChanged);
	        this.CancelCommand = new DelegateCommand(CancelNewConnection);

	        this.NotificationRequest = new InteractionRequest<INotification>();
	        this.SaveAsRequest = new InteractionRequest<SaveAsNotification>();

	        this.ConnectionsModel = new MySqlConnectionsModel
            {
                LoginContent = "Login",
                LoginIsEnabled = true,
                NewIsEnabled = true,
                DeleteIsEnabled = true,
                Visibility = "Hidden",
                Connections = new ObservableCollection<string>()
            };

            InitializeConnections();
        }

        private MySqlConnectionsModel mConnections;

        public MySqlConnectionsModel ConnectionsModel
        {
            get { return this.mConnections; }
            set { SetProperty(ref this.mConnections, value); }
        }

        private List<string> GetConnection()
        {
            return new List<string>
            {
	            this.ConnectionsModel.Address,
	            this.ConnectionsModel.Username,
	            this.ConnectionsModel.Password,
	            this.ConnectionsModel.Database,
	            this.SaveAs,
	            this.ConnectionsModel.Default.ToString()
            };
        }

        private void InitializeConnections()
        {
            var xml = new XmlService();

            foreach (var connection in XmlService.GetIdAttributes())
            {
	            this.ConnectionsModel.Connections.Add(connection);
            }
        }

        private bool IsNull()
        {
            return this.ConnectionsModel.Address == null || this.ConnectionsModel.Username == null || this.ConnectionsModel.Password == null || this.ConnectionsModel.Database == null;
        }

        private void RaiseErrorNotification(string errorMsg, string titleMsg = "Error")
        {
	        this.NotificationRequest.Raise(
                new Notification { Content = errorMsg, Title = titleMsg }
                );
        }

        private void RaiseNewConnection()
        {
            var notification = new SaveAsNotification {Title = "New Connection"};

	        this.SaveAsRequest.Raise(notification,
                returned =>
                {
                    if (returned == null || !returned.Confirmed || returned.SaveAs == null)
                    {
	                    return;
                    }
	                this.SaveAs = returned.SaveAs;
                    if (this.ConnectionsModel.Connections.Contains(this.SaveAs))
                    {
                        RaiseErrorNotification("A connection with the same name already exists.");
                        return;
                    }
	                this.ConnectionsModel.Connections.Add(this.SaveAs);
	                this.ConnectionsModel.DeleteIsEnabled = false;
	                this.ConnectionsModel.SaveIsEnabled = true;
	                this.ConnectionsModel.NewIsEnabled = false;
	                this.ConnectionsModel.Visibility = "Visible";
                });
        }

        private void Login()
        {
            if (IsNull())
            {
                RaiseErrorNotification("Not all fields are filled in.");
            }
            else
            {
                try
                {
                    MySqlConnector.Instance.Configuration(this.ConnectionsModel.Address, this.ConnectionsModel.Username, this.ConnectionsModel.Password, this.ConnectionsModel.Database);
                    MySqlConnector.Instance.OpenConnection();
                    var dt = MySqlConnector.Instance.QueryToDataTable("SELECT * FROM creature_template");
                    CreatureManager.Instance.LoadCreatures(dt);
                    dt = MySqlConnector.Instance.QueryToDataTable("SELECT * FROM gameobject_template");
                    GameObjectManager.Instance.LoadGameObjects(dt);
                    dt = MySqlConnector.Instance.QueryToDataTable("SELECT * FROM item_template");
                    ItemManager.Instance.LoadItem(dt);

	                this.ConnectionsModel.LoginIsEnabled = false;
	                this.ConnectionsModel.LoginContent = "Succesful!";
                }
                catch (System.Exception)
                {
                    MessageBox.Show("An error has occurred when trying to connect to the database.\nPlease verify the database information.");
                }
            }

            /*
            Example code for loading spawned creatures and gameobjects for single maps

            var mapid = 0;
            dt = MySqlConnector.Instance.QueryToDataTable("SELECT * FROM creature WHERE map = '" + mapid + "'");
            CreatureManager.Instance.LoadSpawnedCreatures(dt, mapid);
            dt = MySqlConnector.Instance.QueryToDataTable("SELECT * FROM gameobject WHERE map = '" + mapid + "'");
            GameObjectManager.Instance.LoadSpawnedGameObjects(dt, mapid);
             */
        }

        private void NewConnection()
        {
            RaiseNewConnection();
        }

        private void SaveConnection()
        {
            if (IsNull())
            {
                RaiseErrorNotification("Not all fields are filled in.");
                return;
            }
            var xml = new XmlService();
            XmlService.SaveConnection(GetConnection());

	        this.ConnectionsModel.SaveIsEnabled = false;
	        this.ConnectionsModel.DeleteIsEnabled = true;
	        this.ConnectionsModel.NewIsEnabled = true;
	        this.ConnectionsModel.Visibility = "Hidden";
        }

        private void DeleteConnection()
        {
            var xml = new XmlService();

            if (this.ConnectionsModel.SelectedItem == null)
            {
	            return;
            }
	        XmlService.DeleteConnection(this.ConnectionsModel.SelectedItem);
	        this.ConnectionsModel.Connections.Remove(this.ConnectionsModel.SelectedItem);
        }

        private void CancelNewConnection()
        {
	        this.ConnectionsModel.Connections.Remove(this.SaveAs);
	        this.ConnectionsModel.NewIsEnabled = true;
	        this.ConnectionsModel.SaveIsEnabled = false;
	        this.ConnectionsModel.DeleteIsEnabled = true;
	        this.ConnectionsModel.Visibility = "Hidden";
        }

        private void SelectionChanged()
        {
            var xml = new XmlService();
            if (this.ConnectionsModel.SelectedItem == null || this.ConnectionsModel.SaveIsEnabled)
            {
	            return;
            }
	        var list = XmlService.ReadConnection(this.ConnectionsModel.SelectedItem);

	        this.ConnectionsModel.Address = list[0];
	        this.ConnectionsModel.Username = list[1];
	        this.ConnectionsModel.Password = list[2];
	        this.ConnectionsModel.Database = list[3];
        }
    }
}
