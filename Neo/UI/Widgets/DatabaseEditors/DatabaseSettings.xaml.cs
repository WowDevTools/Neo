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
using WoWEditor6.UI.ViewModels;

namespace WoWEditor6.UI.Widgets
{
    public partial class DatabaseSettings
    {
        public DatabaseSettings()
        {
            InitializeComponent();
            DataContext = new MySqlConnectionsViewModel();
        }
    }
}
