using System.Collections;
using System.Windows;
using Neo.UI.Dialogs;
using System.Windows.Forms;
using SharpDX;
using Neo.Editing;
using Neo.UI.Widget;

namespace Neo.UI.Models
{
	internal class IEditingViewModel
    {
        private readonly IEditingWidget mWidget;

        public IEditingWidget Widget { get { return this.mWidget; } }

        public IEditingViewModel(IEditingWidget widget)
        {
            if (EditorWindowController.Instance != null)
            {
	            EditorWindowController.Instance.IEditingModel = this;
            }

	        this.mWidget = widget;
        }

        public void SwitchWidgets(int widget)
        {
            switch (widget)
            {
                case 0:
	                this.mWidget.ChunkEditingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.TexturingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.TerrainSettingsWidget.Visibility = Visibility.Hidden;
	                this.mWidget.ShadingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.ModelSpawnWidget.Visibility = Visibility.Hidden;
                    break;

                case 1:
	                this.mWidget.ChunkEditingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.TexturingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.TerrainSettingsWidget.Visibility = Visibility.Visible;
	                this.mWidget.ShadingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.ModelSpawnWidget.Visibility = Visibility.Hidden;
                    EditManager.Instance.EnableSculpting();
                    break;

                case 3:
	                this.mWidget.ChunkEditingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.TexturingWidget.Visibility = Visibility.Visible;
	                this.mWidget.TerrainSettingsWidget.Visibility = Visibility.Hidden;
	                this.mWidget.ShadingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.ModelSpawnWidget.Visibility = Visibility.Hidden;
                    EditManager.Instance.EnableTexturing();
                    break;

                case 4:
	                this.mWidget.ChunkEditingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.TexturingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.TerrainSettingsWidget.Visibility = Visibility.Hidden;
	                this.mWidget.ModelSpawnWidget.Visibility = Visibility.Hidden;
	                this.mWidget.ShadingWidget.Visibility = Visibility.Visible;
                    EditManager.Instance.EnableSculpting();
                    EditManager.Instance.EnableShading();
                    break;

                case 5:
	                this.mWidget.ChunkEditingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.TexturingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.TerrainSettingsWidget.Visibility = Visibility.Hidden;
	                this.mWidget.ShadingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.ModelSpawnWidget.Visibility = Visibility.Visible;
                    break;

                case 6:
	                this.mWidget.TexturingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.TerrainSettingsWidget.Visibility = Visibility.Hidden;
	                this.mWidget.ShadingWidget.Visibility = Visibility.Hidden;
	                this.mWidget.ModelSpawnWidget.Visibility = Visibility.Hidden;
	                this.mWidget.ChunkEditingWidget.Visibility = Visibility.Visible;
                    EditManager.Instance.EnableChunkEditing();
                    break;
            }

        }


    }
}
