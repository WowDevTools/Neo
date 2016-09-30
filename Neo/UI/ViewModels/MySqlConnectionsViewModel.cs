using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Neo.Storage.Database;
using Neo.Storage.Database.WotLk.TrinityCore;
using Neo.UI.Models;
using Neo.UI.Services;

namespace Neo.UI.ViewModels
{
    class MySqlConnectionsViewModel : BindableBase
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
            LoginCommand = new DelegateCommand(Login);
            SaveConnectionCommand = new DelegateCommand(SaveConnection);
            NewConnectionCommand = new DelegateCommand(NewConnection);
            DeleteConnectionCommand = new DelegateCommand(DeleteConnection);
            SelectionChangedCommand = new DelegateCommand(SelectionChanged);
            CancelCommand = new DelegateCommand(CancelNewConnection);

            NotificationRequest = new InteractionRequest<INotification>();
            SaveAsRequest = new InteractionRequest<SaveAsNotification>();

            ConnectionsModel = new MySqlConnectionsModel
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
            get { return mConnections; }
            set { SetProperty(ref mConnections, value); }
        }

        private List<string> GetConnection()
        {
            return new List<string>
            {
                ConnectionsModel.Address,
                ConnectionsModel.Username,
                ConnectionsModel.Password,
                ConnectionsModel.Database,
                SaveAs,
                ConnectionsModel.Default.ToString()
            };
        }

        private void InitializeConnections()
        {
            var xml = new XmlService();

            foreach (var connection in XmlService.GetIdAttributes())
            {
                ConnectionsModel.Connections.Add(connection);
            }
        }

        private bool IsNull()
        {
            return ConnectionsModel.Address == null || ConnectionsModel.Username == null || ConnectionsModel.Password == null || ConnectionsModel.Database == null;
        }

        private void RaiseErrorNotification(string errorMsg, string titleMsg = "Error")
        {
            NotificationRequest.Raise(
                new Notification { Content = errorMsg, Title = titleMsg }
                );
        }

        private void RaiseNewConnection()
        {
            var notification = new SaveAsNotification {Title = "New Connection"};

            SaveAsRequest.Raise(notification,
                returned =>
                {
                    if (returned == null || !returned.Confirmed || returned.SaveAs == null) return;
                    SaveAs = returned.SaveAs;
                    if (ConnectionsModel.Connections.Contains(SaveAs))
                    {
                        RaiseErrorNotification("A connection with the same name already exists.");
                        return;
                    }
                    ConnectionsModel.Connections.Add(SaveAs);
                    ConnectionsModel.DeleteIsEnabled = false;
                    ConnectionsModel.SaveIsEnabled = true;
                    ConnectionsModel.NewIsEnabled = false;
                    ConnectionsModel.Visibility = "Visible";
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
                    MySqlConnector.Instance.Configuration(ConnectionsModel.Address, ConnectionsModel.Username,
                        ConnectionsModel.Password, ConnectionsModel.Database);
                    MySqlConnector.Instance.OpenConnection();
                    var dt = MySqlConnector.Instance.QueryToDataTable("SELECT * FROM creature_template");
                    CreatureManager.Instance.LoadCreatures(dt);
                    dt = MySqlConnector.Instance.QueryToDataTable("SELECT * FROM gameobject_template");
                    GameObjectManager.Instance.LoadGameObjects(dt);
                    dt = MySqlConnector.Instance.QueryToDataTable("SELECT * FROM item_template");
                    ItemManager.Instance.LoadItem(dt);

                    ConnectionsModel.LoginIsEnabled = false;
                    ConnectionsModel.LoginContent = "Succesful!";
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

            ConnectionsModel.SaveIsEnabled = false;
            ConnectionsModel.DeleteIsEnabled = true;
            ConnectionsModel.NewIsEnabled = true;
            ConnectionsModel.Visibility = "Hidden";
        }

        private void DeleteConnection()
        {
            var xml = new XmlService();

            if (ConnectionsModel.SelectedItem == null) return;
            XmlService.DeleteConnection(ConnectionsModel.SelectedItem);
            ConnectionsModel.Connections.Remove(ConnectionsModel.SelectedItem);
        }

        private void CancelNewConnection()
        {
            ConnectionsModel.Connections.Remove(SaveAs);
            ConnectionsModel.NewIsEnabled = true;
            ConnectionsModel.SaveIsEnabled = false;
            ConnectionsModel.DeleteIsEnabled = true;
            ConnectionsModel.Visibility = "Hidden";
        }

        private void SelectionChanged()
        {
            var xml = new XmlService();
            if (ConnectionsModel.SelectedItem == null || ConnectionsModel.SaveIsEnabled) return;
            var list = XmlService.ReadConnection(ConnectionsModel.SelectedItem);

            ConnectionsModel.Address = list[0];
            ConnectionsModel.Username = list[1];
            ConnectionsModel.Password = list[2];
            ConnectionsModel.Database = list[3];
        }
    }
}
