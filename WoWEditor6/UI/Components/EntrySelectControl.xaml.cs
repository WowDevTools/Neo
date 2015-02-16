using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WoWEditor6.UI.Components
{
    /// <summary>
    /// Interaction logic for EntrySelectControl.xaml
    /// </summary>
    public partial class EntrySelectControl
    {
        public EntrySelectControl()
        {
            InitializeComponent();

            var group = new TransformGroup();
            group.Children.Add(new ScaleTransform());
            group.Children.Add(new TranslateTransform());
            WdlPreviewImage.RenderTransform = group;
        }

        public void MapSelected(int mapId)
        {
            WdlPreviewImage.Source = WpfImageSource.FromBgra(17*64, 17*64, GetWdlColors(mapId));
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var g = (sender as Grid);
            if (g == null)
                return;

            var height = g.RowDefinitions[1].ActualHeight;
            var width = g.ColumnDefinitions[1].ActualWidth;
            if (height < width)
                width = height;
            else if (width < height)
                height = width;

            WdlPreviewImage.Width = width;
            WdlPreviewImage.Height = height;
        }

        private void WdlPreview_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(WdlPreviewImage);
            var facx = pos.X / WdlPreviewImage.ActualWidth;
            var facy = pos.Y / WdlPreviewImage.ActualHeight;

            var adtx = (int) (facx * 64);
            var adty = (int) (facy * 64);

            AdtPreviewLabel.Text = string.Format("ADT: {0}/{1}", adtx, adty);
        }

        private static uint[] GetWdlColors(int mapId)
        {
            var colors = new uint[17*64*17*64];
            var record = Storage.DbcStorage.Map.GetRowById(mapId);
            if (record == null)
                return colors;

            var mapName = record.GetString(Storage.MapFormatGuess.FieldMapName);
            var wdlFile = new IO.Files.Terrain.WdlFile();
            wdlFile.Load(mapName);
            for (var i = 0; i < 64; ++i)
            {
                for (var j = 0; j < 64; ++j)
                {
                    if (wdlFile.HasEntry(j, i) == false)
                        continue;

                    var entry = wdlFile.GetEntry(j, i);
                    LoadEntry(entry, colors, ref i, ref j);
                }
            }

            return colors;
        }

        private static unsafe void LoadEntry(IO.Files.Terrain.MareEntry entry, uint[] textureData, ref int i, ref int j)
        {
            for (var k = 0; k < 17; ++k)
            {
                for (var l = 0; l < 17; ++l)
                {
                    uint r;
                    uint g;
                    uint b;
                    var h = entry.outer[k * 17 + l];
                    if (h > 2000)
                    {
                        r = g = b = 255;
                    }
                    else if (h > 1000)
                    {
                        var am = (h - 1000) / 1000.0f;
                        r = (uint)(0.75f + am * 0.25f * 255);
                        g = (uint)(0.5f * am * 255);
                        b = (uint)(0.75f + am * 0.5f * 255);
                    }
                    else if (h > 600)
                    {
                        var am = (h - 600) / 400.0f;
                        r = (uint)(0.75 + am * 0.25f * 255);
                        g = (uint)(0.5f * am * 255);
                        b = (uint)(am * 255);
                    }
                    else if (h > 300)
                    {
                        var am = (h - 300) / 300.0f;
                        r = (uint)(255 - am * 255);
                        g = 1;
                        b = 0;
                    }
                    else if (h > 0)
                    {
                        var am = h / 300.0f;
                        r = (uint)(0.75 * am * 255);
                        g = (uint)(255 - (0.5f * am * 255));
                        b = 0;
                    }
                    else if (h > -100)
                    {
                        var am = (h + 100.0f) / 100.0f;
                        r = (uint)(0.0f);
                        g = (uint)(am * 127);
                        b = 200;
                    }
                    else
                    {
                        r = g = 0;
                        b = 0x2F;
                    }

                    if (k == 0 || l == 0)
                        r = g = b = 0;

                    textureData[(i * 17 + k) * (64 * 17) + j * 17 + l] = 0xFF000000 | (r << 16) | (g << 8) | (b << 0);
                }
            }
        }
    }
}
