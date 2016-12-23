using System.Windows;
using System.Windows.Controls;
using Neo.Editing;
using Neo.Scene.Terrain;
using System;
using System.Linq;
using System.Collections.Generic;
using Neo.UI.Models;

namespace Neo.UI.Widgets
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
	        this.lstArea.Items.Clear();

            foreach (var at in Storage.DbcStorage.AreaTable.GetAllRows<IO.Files.Terrain.Wotlk.AreaTable>())
            {
	            this.lstArea.Items.Add(new KeyValuePair<int, string>(at.Id, at.Areaname_Lang.Locale));
            }

	        this.lstArea.DisplayMemberPath = "Value";
	        this.lstArea.SelectedValuePath = "Key";
	        this.lstArea.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Value", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void OnSelectedAreaId(int areaid)
        {
            if (this.lstArea.Items.Count == 0)
            {
	            return;
            }

	        this.lstArea.SelectedValue = areaid;
	        this.lstArea.ScrollIntoView(this.lstArea.SelectedItem);
        }

        private void OnHoveredAreaChange(int areaid)
        {
            if (this.lstArea.Items.Count == 0)
            {
	            return;
            }

	        var area = this.lstArea.Items.Cast<KeyValuePair<int, string>>().FirstOrDefault(x => x.Key == areaid);
	        this.txtAreaName.Text =  $"Current Area: {(area.Key > 0 ? area.Value : "Unknown")}";
        }


        private void chkChunkLines_Change(object sender, RoutedEventArgs e)
        {
            var model = DataContext as ChunkEditingViewModel;
            if (model == null)
            {
	            return;
            }

	        model.HandleChunkLinesChange(((CheckBox)e.Source).IsChecked.Value);
        }

        private void chkAreaColour_Change(object sender, RoutedEventArgs e)
        {
            var model = DataContext as ChunkEditingViewModel;
            if (model == null)
            {
	            return;
            }

	        model.HandleAreaColourChange(((CheckBox)e.Source).IsChecked.Value);
        }

        private void lstArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lstArea.SelectedItem == null)
            {
	            return;
            }

	        var model = DataContext as ChunkEditingViewModel;
            if (model == null)
            {
	            return;
            }

	        model.HandleAreaSelectionChange(((KeyValuePair<int, string>) this.lstArea.SelectedItem).Key);
        }

        private void txtSearchArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Filter the area listbox
	        this.lstArea.Items.Filter = new Predicate<object>((item) =>
            {
                return ((KeyValuePair<int, string>)item).Value.IndexOf(this.txtSearchArea.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
            });
        }

        private void rdoMode_Checked(object sender, RoutedEventArgs e)
        {
            var model = DataContext as ChunkEditingViewModel;
            if (model == null || !(e.Source as RadioButton).IsChecked.Value)
            {
	            return;
            }

	        var rdo = sender as RadioButton;
            if (rdo.Name == "rdoPaintChunk")
            {
	            model.SetChunkEditState(ChunkEditMode.AreaPaint);
            }
            else
            {
	            model.SetChunkEditState(ChunkEditMode.AreaSelect);
            }
        }

        private void tabChunkMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var model = DataContext as ChunkEditingViewModel;
            if (model == null)
            {
	            return;
            }

	        var tabControl = e.Source as TabControl;
            if (tabControl == null)
            {
	            return;
            }

	        switch ((tabControl.SelectedItem as TabItem).Name)
            {
                case "tabChunkPaint":
                    model.SetChunkEditState(this.rdoPaintChunk.IsChecked.Value ? ChunkEditMode.AreaPaint : ChunkEditMode.AreaSelect); //Enforce correct mode
                    break;
                case "tabFlags":
                    model.SetChunkEditState(ChunkEditMode.Flags);
                    break;
                case "tabHoles":
                    model.SetChunkEditState(ChunkEditMode.Hole);
                    break;
            }
        }

        private void rdoHoleParamsChecked(object sender, RoutedEventArgs e)
        {
            var model = DataContext as ChunkEditingViewModel;
            if (model == null || this.rdoSmallHole == null || this.rdoCreateHole == null)
            {
	            return;
            }

	        model.HandleHoleParamsChange(this.rdoSmallHole.IsChecked.Value, this.rdoCreateHole.IsChecked.Value);
        }

    }
}
