using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WoWEditor6.Storage.Database;
using WoWEditor6.Storage.Database.WotLk.TrinityCore;

namespace WoWEditor6.UI.Widgets
{
    /// <summary>
    /// Interaction logic for DatabaseSettings.xaml
    /// </summary>
    public partial class DatabaseSettings : UserControl
    {
        public DatabaseSettings()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnector.Instance.Configuration(tbAddress.Text, tbUser.Text, tbPassword.Text, tbDatabase.Text);
            MySqlConnector.Instance.OpenConnection();
            var dt = MySqlConnector.Instance.QueryToDataTable("SELECT * FROM creature_template");
            CreatureManager.Instance.LoadCreatures(dt);
            dt = MySqlConnector.Instance.QueryToDataTable("SELECT * FROM gameobject_template");
            GameObjectManager.Instance.LoadGameObjects(dt);
            btnLogin.IsEnabled = false;
            btnLogin.Content = "Succesful!";

            /*
            Example code for loading spawned creatures and gameobjects for single maps

            var mapid = 0;
            dt = MySqlConnector.Instance.QueryToDataTable("SELECT * FROM creature WHERE map = '" + mapid + "'");
            CreatureManager.Instance.LoadSpawnedCreatures(dt, mapid);
            dt = MySqlConnector.Instance.QueryToDataTable("SELECT * FROM gameobject WHERE map = '" + mapid + "'");
            GameObjectManager.Instance.LoadSpawnedGameObjects(dt, mapid);*/

        }
    }
}
