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
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            MySqlConnector.Instance.Configuration(tbAddress.Text, tbUser.Text, tbPassword.Text, tbDatabase.Text);
            MySqlConnector.Instance.OpenConnection();

            var dt = MySqlConnector.Instance.QueryToDataTable("SELECT * FROM creature_template");
            CreatureManager.Instance.LoadCreatures(dt);
            sw.Stop();
            System.Windows.MessageBox.Show("Seconds:" + sw.Elapsed.Seconds.ToString());
        }
    }
}
