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
using Neo.Storage.Database;
using Neo.UI.ViewModels;

namespace Neo.UI.Widgets
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
