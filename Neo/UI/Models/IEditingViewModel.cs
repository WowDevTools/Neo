using System.Collections;
using System.Windows;
using Neo.UI.Dialogs;
using System.Windows.Forms;
using SharpDX;
using Neo.Editing;
using Neo.UI.Widget;

namespace Neo.UI.Models
{
    class IEditingViewModel
    {
        private readonly IEditingWidget mWidget;

        public IEditingWidget Widget { get { return mWidget; } }

        public IEditingViewModel(IEditingWidget widget)
        {
            if (EditorWindowController.Instance != null)
                EditorWindowController.Instance.IEditingModel = this;

            mWidget = widget;
        }

        public void SwitchWidgets(int widget)
        {
            switch (widget)
            {
                case 0:
                    mWidget.ChunkEditingWidget.Visibility = Visibility.Hidden;
                    mWidget.TexturingWidget.Visibility = Visibility.Hidden;
                    mWidget.TerrainSettingsWidget.Visibility = Visibility.Hidden;
                    mWidget.ShadingWidget.Visibility = Visibility.Hidden;
                    mWidget.ModelSpawnWidget.Visibility = Visibility.Hidden;
                    break;

                case 1:
                    mWidget.ChunkEditingWidget.Visibility = Visibility.Hidden;
                    mWidget.TexturingWidget.Visibility = Visibility.Hidden;
                    mWidget.TerrainSettingsWidget.Visibility = Visibility.Visible;
                    mWidget.ShadingWidget.Visibility = Visibility.Hidden;
                    mWidget.ModelSpawnWidget.Visibility = Visibility.Hidden;
                    EditManager.Instance.EnableSculpting();
                    break;

                case 3:
                    mWidget.ChunkEditingWidget.Visibility = Visibility.Hidden;
                    mWidget.TexturingWidget.Visibility = Visibility.Visible;
                    mWidget.TerrainSettingsWidget.Visibility = Visibility.Hidden;
                    mWidget.ShadingWidget.Visibility = Visibility.Hidden;
                    mWidget.ModelSpawnWidget.Visibility = Visibility.Hidden;
                    EditManager.Instance.EnableTexturing();
                    break;

                case 4:
                    mWidget.ChunkEditingWidget.Visibility = Visibility.Hidden;
                    mWidget.TexturingWidget.Visibility = Visibility.Hidden;
                    mWidget.TerrainSettingsWidget.Visibility = Visibility.Hidden;
                    mWidget.ModelSpawnWidget.Visibility = Visibility.Hidden;
                    mWidget.ShadingWidget.Visibility = Visibility.Visible;
                    EditManager.Instance.EnableSculpting();
                    EditManager.Instance.EnableShading();
                    break;

                case 5:
                    mWidget.ChunkEditingWidget.Visibility = Visibility.Hidden;
                    mWidget.TexturingWidget.Visibility = Visibility.Hidden;
                    mWidget.TerrainSettingsWidget.Visibility = Visibility.Hidden;
                    mWidget.ShadingWidget.Visibility = Visibility.Hidden;
                    mWidget.ModelSpawnWidget.Visibility = Visibility.Visible;
                    break;

                case 6:
                    mWidget.TexturingWidget.Visibility = Visibility.Hidden;
                    mWidget.TerrainSettingsWidget.Visibility = Visibility.Hidden;
                    mWidget.ShadingWidget.Visibility = Visibility.Hidden;
                    mWidget.ModelSpawnWidget.Visibility = Visibility.Hidden;
                    mWidget.ChunkEditingWidget.Visibility = Visibility.Visible;
                    EditManager.Instance.EnableChunkEditing();
                    break;
            }

        }


    }
}
