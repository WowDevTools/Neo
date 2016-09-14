using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SharpDX;
using Neo.IO;
using Neo.IO.Files.Terrain;
using Neo.Storage;

namespace Neo.UI.Components
{
    /// <summary>
    /// Interaction logic for EntrySelectControl.xaml
    /// </summary>
    public partial class EntrySelectControl
    {
        private bool mIsClicked;
        private int mSelectedMap;

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
            mSelectedMap = mapId;
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

        private void WdlPreview_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mIsClicked = true;
            WdlPreviewImage.CaptureMouse();
        }

        private void WdlPreview_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(WdlPreviewImage);

            if (position.X > 0 && position.Y > 0 && position.X < WdlPreviewImage.ActualWidth &&
                position.Y < WdlPreviewImage.ActualHeight && mIsClicked)
            {
                var facx = position.X / WdlPreviewImage.ActualWidth;
                var facy = position.Y / WdlPreviewImage.ActualHeight;

                var entryx = facx * 64 * Metrics.TileSize;
                var entryy = facy * 64 * Metrics.TileSize;

                OnEnterWorld((float) entryx, (float) entryy);
            }


            mIsClicked = false;
            WdlPreviewImage.ReleaseMouseCapture();
        }

        private void OnEnterWorld(float x, float y)
        {
            var mapRow = DbcStorage.Map.GetRowById(mSelectedMap);
            if (mapRow == null)
                return;

            var widescreen = false;
            var loadScreenPath = "Interface\\Glues\\loading.blp";
            var loadEntry = mapRow.GetInt32(MapFormatGuess.FieldMapLoadingScreen);
            if (loadEntry != 0)
            {
                var loadRow = DbcStorage.LoadingScreen.GetRowById(loadEntry);
                if (loadRow != null)
                {
                    var path = loadRow.GetString(MapFormatGuess.FieldLoadingScreenPath);

                    if (string.IsNullOrEmpty(path) == false)
                    {
                        if (MapFormatGuess.FieldLoadingScreenHasWidescreen >= 0 && loadRow.GetInt32(MapFormatGuess.FieldLoadingScreenHasWidescreen) == 1)
                        {
                            var widePath = path.ToUpperInvariant().Replace(".BLP", "WIDE.BLP");
                            if (FileManager.Instance.Provider.Exists(widePath))
                            {
                                path = widePath;
                                widescreen = true;
                            }
                        }

                        loadScreenPath = path;
                    }
                }
            }

            if (string.IsNullOrEmpty(loadScreenPath)) return;
            var wnd = DataContext as EditorWindow;
            if (wnd == null) return;
            Visibility = Visibility.Collapsed;
            wnd.LoadingScreenView.Visibility = Visibility.Visible;
            wnd.LoadingScreenView.OnLoadStarted(mSelectedMap, loadScreenPath, widescreen, new Vector2(x, y));
        }

        private static uint[] GetWdlColors(int mapId)
        {
            var colors = new uint[17*64*17*64];
            var record = DbcStorage.Map.GetRowById(mapId);
            if (record == null)
                return colors;

            var mapName = record.GetString(MapFormatGuess.FieldMapName);
            var wdlFile = new WdlFile();
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

        private static unsafe void LoadEntry(MareEntry entry, uint[] textureData, ref int i, ref int j)
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            EditorWindowController.Instance.ShowMapOverview();
        }
    }
}
