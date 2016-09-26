using System.Windows;
using System.Windows.Controls;
using WoWEditor6.Editing;
using WoWEditor6.Scene.Terrain;
using System;
using System.Linq;
using System.Collections.Generic;
using WoWEditor6.UI.Models;

namespace WoWEditor6.UI.Widgets
{
    /// <summary>
    /// Interaction logic for ChunkEditingWidget.xaml
    /// </summary>
    public partial class ChunkEditingWidget : UserControl
    {
        public ChunkEditingWidget()
        {
            DataContext = new ChunkEditingViewModel(this);
            InitializeComponent();
            ChunkEditManager.Instance.SelectedAreaIdChange += OnSelectedAreaId;
            ChunkEditManager.Instance.HoveredAreaChange += OnHoveredAreaChange;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAreaListBox();
        }

        private void LoadAreaListBox()
        {
            lstArea.Items.Clear();

            foreach (var at in Storage.DbcStorage.AreaTable.GetAllRows<IO.Files.Terrain.Wotlk.AreaTable>())
                lstArea.Items.Add(new KeyValuePair<int, string>(at.Id, at.Areaname_Lang.Locale));

            lstArea.DisplayMemberPath = "Value";
            lstArea.SelectedValuePath = "Key";
            lstArea.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Value", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void OnSelectedAreaId(int areaid)
        {
            if (lstArea.Items.Count == 0) return;

            lstArea.SelectedValue = areaid;
            lstArea.ScrollIntoView(lstArea.SelectedItem);
        }

        private void OnHoveredAreaChange(int areaid)
        {
            if (lstArea.Items.Count == 0) return;

            var area = lstArea.Items.Cast<KeyValuePair<int, string>>().FirstOrDefault(x => x.Key == areaid);
            txtAreaName.Text =  $"Current Area: {(area.Key > 0 ? area.Value : "Unknown")}";
        }


        private void chkChunkLines_Change(object sender, RoutedEventArgs e)
        {
            var model = DataContext as ChunkEditingViewModel;
            if (model == null)
                return;

            model.HandleChunkLinesChange(((CheckBox)e.Source).IsChecked.Value);
        }

        private void chkAreaColour_Change(object sender, RoutedEventArgs e)
        {
            var model = DataContext as ChunkEditingViewModel;
            if (model == null)
                return;

            model.HandleAreaColourChange(((CheckBox)e.Source).IsChecked.Value);
        }

        private void lstArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstArea.SelectedItem == null)
                return;

            var model = DataContext as ChunkEditingViewModel;
            if (model == null)
                return;

            model.HandleAreaSelectionChange(((KeyValuePair<int, string>)lstArea.SelectedItem).Key);
        }

        private void txtSearchArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Filter the area listbox
            lstArea.Items.Filter = new Predicate<object>((item) =>
            {
                return ((KeyValuePair<int, string>)item).Value.IndexOf(txtSearchArea.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
            });
        }

        private void rdoMode_Checked(object sender, RoutedEventArgs e)
        {
            var model = DataContext as ChunkEditingViewModel;
            if (model == null || !(e.Source as RadioButton).IsChecked.Value)
                return;

            var rdo = sender as RadioButton;
            if (rdo.Name == "rdoPaintChunk")
                model.SetChunkEditState(ChunkEditMode.AreaPaint);
            else
                model.SetChunkEditState(ChunkEditMode.AreaSelect);
        }

        private void tabChunkMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var model = DataContext as ChunkEditingViewModel;
            if (model == null)
                return;

            var tabControl = e.Source as TabControl;
            if (tabControl == null)
                return;

            switch ((tabControl.SelectedItem as TabItem).Name)
            {
                case "tabChunkPaint":
                    model.SetChunkEditState(rdoPaintChunk.IsChecked.Value ? ChunkEditMode.AreaPaint : ChunkEditMode.AreaSelect); //Enforce correct mode
                    break;
            }
        }
    }
}
