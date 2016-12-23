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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Neo.UI.Models;

namespace Neo.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for ImportFileDialog.xaml
    /// </summary>
    public partial class ImportFileDialog
    {
        public ImportFileDialog(ImportFileViewModel model)
        {
            DataContext = model;
            InitializeComponent();
        }

        private void BrowseFile_Click(object sender, RoutedEventArgs e)
        {
            var model = DataContext as ImportFileViewModel;
            if (model == null)
            {
	            return;
            }

	        model.BrowseForFile();
        }

        private void ImportTexture_Button(object sender, RoutedEventArgs e)
        {
            var model = DataContext as ImportFileViewModel;
            if (model == null)
            {
	            return;
            }

	        model.HandleFileImport();
        }

        private void Handle_TargetSelectedButton(object sender, RoutedEventArgs e)
        {
            var model = DataContext as ImportFileViewModel;
            if (model == null)
            {
	            return;
            }

	        model.HandleFileImportSettings();
        }
    }
}
