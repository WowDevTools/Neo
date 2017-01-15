using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neo.IO;
using Neo.IO.Files.Terrain;
using Neo.IO.Files.Texture;
using Neo.Scene;
using Neo.UI.Widgets;
using Neo.Utils;
using OpenTK.Input;
using CheckBox = System.Windows.Controls.CheckBox;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Neo.UI.Models
{
	internal class TexturingViewModel
    {
        private readonly TexturingWidget mWidget;
        private bool mIsValueChangedSurpressed;
        private WeakReference<MapArea> mLastArea;
        private WeakReference<MapChunk> mLastChunk;
        private readonly List<string> mRecentTextures = new List<string>();
        private readonly List<string> mFavoriteTextures = new List<string>();
        private readonly List<string> mTilesets = new List<string>();

        public TexturingWidget Widget { get { return this.mWidget; } }

        public TexturingViewModel(TexturingWidget widget)
        {
            if (EditorWindowController.Instance != null)
            {
	            EditorWindowController.Instance.TexturingModel = this;
            }

	        this.mWidget = widget;
            if (WorldFrame.Instance != null)
            {
	            WorldFrame.Instance.OnWorldClicked += OnWorldClick;
            }

	        FileManager.Instance.LoadComplete += OnFilesLoaded;
        }

        private void SetSelectedTileTextures(Control panel, IEnumerable<string> textures)
        {
            panel.Controls.Clear();

            var loadTasks = new List<Tuple<string, PictureBox>>();

            foreach (var tex in textures)
            {
                var pnl = new Panel
                {
                    Width = 100,
                    Height = 100,
                    Margin = new Padding(5, 5, 0, 0)
                };

                var pb = new PictureBox
                {
                    Width = 96,
                    Height = 96,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Location = new Point(2, 2),
                    Tag = tex
                };

                pnl.Controls.Add(pb);

                SetEventHandlers(pb);

                panel.Controls.Add(pnl);

                var texName = tex;
                loadTasks.Add(new Tuple<string, PictureBox>(texName, pb));
            }

            new Thread(async () =>
            {
                foreach (var pair in loadTasks)
                {
                    var img = pair.Item2;
                    var bmp = await CreateBitmap(pair.Item1);

                    await EditorWindowController.Instance.WindowDispatcher.BeginInvoke(new Action(() =>
                    {
                        img.Image = bmp;
                        img.CreateControl();
                    }));
                }
            }).Start();
        }

        public void HandleSelectFromAssets()
        {
            EditorWindowController.Instance.ShowAssetBrowser();
        }

        public void HandleAmountSlider(float value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.TextureChangeManager.Instance.Amount = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleInnerRadiusSlider(float value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.InnerRadius = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleOuterRadiusSlider(float value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.OuterRadius = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandlePenSensivity(float value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.PenSensivity = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleTabletControl(bool value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.IsTabletOn = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleSprayMode(bool value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.IsSprayOn = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleTabletChangeRadius(bool value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.IsTablet_RChange = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleTabletChangeInnerRadius(bool value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.IsTablet_IRChange = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleTabletControlPressure(bool value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.IsTablet_PChange = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleAllowedAmplitude(float value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.Amplitude = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleAllowedInnerAmplitude(float value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.InnerAmplitude = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleParticleSize(float value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.SprayParticleSize = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleParticleAmount(float value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.SprayParticleAmount = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleParticleHardness(float value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.SprayParticleHarndess = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleSpraySolidInnerRadius(bool value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.IsSpraySolidInnerRadius = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleInnerRadiusChanged(float newRadius)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.InnerRadiusSlider.Value = newRadius;
        }

        public void HandleOuterRadiusChanged(float newRadius)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.OuterRadiusSlider.Value = newRadius;
        }

        public void HandleAmoutChanged(float newAmount)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.AmountSlider.Value = newAmount;
        }

        public void HandleOpacityChanged(float newOpacity)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.GradientSlider.Value = newOpacity;
        }

        public void HandlePenSensivityChanged(float newSensivity)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.Tablet_SensivitySlider.Value = newSensivity;
        }

        public void HandleTabletControlChanged(bool newIsTabletChanged)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.TabletControlBox.IsChecked = newIsTabletChanged;
        }

        public void HandleSprayModeChanged(bool newIsSprayOn)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.SprayModeBox.IsChecked = newIsSprayOn;
        }

        public void HandleTabletChangeRadiusChanged(bool newIsTablet_RChanged)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.TabletControlBox_Radius.IsChecked = newIsTablet_RChanged;
        }

        public void HandleTabletChangeInnerRadiusChanged(bool newIsTablet_IRChanged)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.TabletControlBox_InnerRadius.IsChecked = newIsTablet_IRChanged;
        }

        public void HandleTabletControlPressureChanged(bool newIsTablet_PChanged)
        {
            if(this.mIsValueChangedSurpressed)
            {
	            return;
            }
	        this.mWidget.PressureControlBox.IsChecked = newIsTablet_PChanged;
        }

        public void HandleAllowedAmplitudeChanged(float newAmplitude )
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }
	        this.mWidget.Tablet_RadiusSlider.Value = newAmplitude;
        }

        public void HandleAllowedInnerAmplitudeChanged(float newInnerAmplitude)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }
	        this.mWidget.Tablet_InnerRadiusSlider.Value = newInnerAmplitude;
        }

        public void HandleParticleSizeChanged(float newSprayParticleSize)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }
	        this.mWidget.Spray_ParticleSizeSlider.Value = newSprayParticleSize;
        }

        public void HandleParticleAmountChanged(float newSprayParticleAmount)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.Spray_ParticleAmountSlider.Value = newSprayParticleAmount;
        }

        public void HandleParticleHardnessChanged(float newSprayParticleHardness)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.Spray_ParticleHardnessSlider.Value = newSprayParticleHardness;
        }

        public void HandleSpraySolidInnerRadiusChanged(bool newIsSpraySolidInnerRadius)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }
	        this.mWidget.SpraySolidInnerRadiusBox.IsChecked = newIsSpraySolidInnerRadius;
        }

        public void SwitchToTexturing()
        {
            Editing.EditManager.Instance.EnableTexturing();
        }

        public async void UpdateFilters()
        {
            var query = this.mWidget.TextureQueryBox.Text.ToLowerInvariant();
            if (string.IsNullOrEmpty(query) || query.Length < 4)
            {
	            return;
            }

	        var selectedFilters = (from CheckBox cb in this.mWidget.FilterWrapPanel.Children where cb.IsChecked ?? false select ((string)cb.Content).ToLowerInvariant()).ToList();

	        this.mWidget.SearchResultLayout.Controls.Clear();
            var newValues = this.mTilesets.Where(s => s.Contains(query) && (selectedFilters.Count == 0 || selectedFilters.Any(s.Contains)));

            var toAdd = new List<Control>();
            foreach (var tex in newValues)
            {
                var pnl = new Panel
                {
                    Width = 100,
                    Height = 100,
                    Margin = new Padding(5, 5, 0, 0)
                };

                var pb = new PictureBox
                {
                    Width = 96,
                    Height = 96,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Location = new Point(2, 2),
                    Tag = tex,
                    Image = await CreateBitmap(tex)
                };


                pnl.Controls.Add(pb);

                SetEventHandlers(pb);
                toAdd.Add(pnl);
            }

	        this.mWidget.SearchResultLayout.Controls.AddRange(toAdd.ToArray());
        }

        public async void SearchForTexture(string query)
        {
            if (string.IsNullOrEmpty(query) || query.Length < 4)
            {
	            return;
            }

	        var selectedFilters = (from CheckBox cb in this.mWidget.FilterWrapPanel.Children where cb.IsChecked ?? false select ((string) cb.Content).ToLowerInvariant()).ToList();

	        this.mWidget.SearchResultLayout.Controls.Clear();
            query = query.ToLowerInvariant();
            var newValues = this.mTilesets.Where(s => s.Contains(query) && (selectedFilters.Count == 0 || selectedFilters.Any(s.Contains)));

            var toAdd = new List<Control>();
            foreach (var tex in newValues)
            {
                var pnl = new Panel
                {
                    Width = 100,
                    Height = 100,
                    Margin = new Padding(5, 5, 0, 0)
                };

                var pb = new PictureBox
                {
                    Width = 96,
                    Height = 96,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Location = new Point(2, 2),
                    Tag = tex,
                    Image = await CreateBitmap(tex)
                };


                pnl.Controls.Add(pb);

                SetEventHandlers(pb);
                toAdd.Add(pnl);
            }

	        this.mWidget.SearchResultLayout.Controls.AddRange(toAdd.ToArray());
        }

        public async void OnFavoriteButtonClicked()
        {
            var curTexture = this.mWidget.TexturePreviewImage.Tag as string;
            if (string.IsNullOrEmpty(curTexture))
            {
	            return;
            }

	        curTexture = curTexture.ToLowerInvariant();
            var index = this.mFavoriteTextures.IndexOf(curTexture);
            if (index < 0)
            {
                var pnl = new Panel
                {
                    Width = 100,
                    Height = 100,
                    Margin = new Padding(5, 5, 0, 0)
                };

                var pb = new PictureBox
                {
                    Width = 96,
                    Height = 96,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Location = new Point(2, 2),
                    Tag = curTexture,
                    Image = await CreateBitmap(curTexture)
                };


                pnl.Controls.Add(pb);

                SetEventHandlers(pb);
	            this.mFavoriteTextures.Insert(0, curTexture);
	            this.mWidget.FavoriteWrapPanel.Controls.Add(pnl);
	            this.mWidget.FavoriteWrapPanel.Controls.SetChildIndex(pnl, 0);
	            this.mWidget.FavoriteButton.Content = "Remove Favorite";
            }
            else
            {
	            this.mWidget.FavoriteWrapPanel.Controls.RemoveAt(index);
	            this.mFavoriteTextures.RemoveAt(index);
	            this.mWidget.FavoriteButton.Content = "Add Favorite";
            }

            var coll = new StringCollection();
            coll.AddRange(this.mFavoriteTextures.ToArray());
            Properties.Settings.Default.FavoriteTextures = coll;
            Properties.Settings.Default.Save();
        }

        private static unsafe Task<Bitmap> CreateBitmap(string name)
        {
            return Task.Factory.StartNew(() =>
            {
                var loadInfo = TextureLoader.LoadToArgbImage(name);
                if (loadInfo == null)
                {
	                return null;
                }

	            var bmp = new Bitmap(loadInfo.Width, loadInfo.Height, PixelFormat.Format32bppArgb);
                var bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);

                fixed (byte* ptr = loadInfo.Layers[0])
                {
	                UnsafeNativeMethods.CopyMemory((byte*) bmpd.Scan0, ptr, bmp.Width * bmp.Height * 4);
                }

	            bmp.UnlockBits(bmpd);
                return bmp;
            });
        }

        private async void AddRecentTexture(string texture, bool initial = false)
        {
            texture = texture.ToLowerInvariant();
            if (this.mRecentTextures.Contains(texture))
            {
                var index = this.mRecentTextures.IndexOf(texture);
	            this.mRecentTextures.RemoveAt(index);
	            this.mRecentTextures.Add(texture);
                var elem = this.mWidget.RecentWrapPanel.Controls[index];
	            this.mWidget.RecentWrapPanel.Controls.SetChildIndex(elem, 0);
            }
            else
            {
                var pnl = new Panel
                {
                    Width = 100,
                    Height = 100,
                    Margin = new Padding(5, 5, 0, 0)
                };

                var pb = new PictureBox
                {
                    Width = 96,
                    Height = 96,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Location = new Point(2, 2),
                    Tag = texture,
                    Image = await CreateBitmap(texture)
                };

                if (pb.Image == null)
                {
	                pnl.Visible = false;
                }

	            pnl.Controls.Add(pb);

                SetEventHandlers(pb);

	            this.mRecentTextures.Insert(0, texture);
	            this.mWidget.RecentWrapPanel.Controls.Add(pnl);
	            this.mWidget.RecentWrapPanel.Controls.SetChildIndex(pnl, 0);
            }

            if (initial == false)
            {
                var newColl = new StringCollection();
                newColl.AddRange(this.mRecentTextures.ToArray());
                Properties.Settings.Default.RecentTextures = newColl;
                Properties.Settings.Default.Save();
            }
        }

        private async void InitRecentTextures(IEnumerable textures)
        {
            foreach (string tex in textures)
            {
                if (this.mFavoriteTextures.Contains(tex.ToLowerInvariant()))
                {
	                continue;
                }

	            this.mFavoriteTextures.Add(tex.ToLowerInvariant());

                var pnl = new Panel
                {
                    Width = 100,
                    Height = 100,
                    Margin = new Padding(5, 5, 0, 0)
                };

                var pb = new PictureBox
                {
                    Width = 96,
                    Height = 96,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Location = new Point(2, 2),
                    Tag = tex.ToLowerInvariant(),
                    Image = await CreateBitmap(tex)
                };

                pnl.Controls.Add(pb);

                if (pb.Image == null)
                {
	                pnl.Visible = false;
                }

	            SetEventHandlers(pb);
	            this.mWidget.FavoriteWrapPanel.Controls.Add(pnl);
            }
        }

        private void OnFilesLoaded()
        {
            var tilesetRoot = FileManager.Instance.FileListing.RootEntry.Children["tileset"] as DirectoryEntry;
            HandleTilesetDirectory(tilesetRoot, "Tileset");

	        this.mWidget.Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var tex in Properties.Settings.Default.RecentTextures)
                {
	                AddRecentTexture(tex, true);
                }

	            InitRecentTextures(Properties.Settings.Default.FavoriteTextures);
            }));
        }

        private void HandleTilesetDirectory(DirectoryEntry dir, string curDir)
        {
            if (dir == null)
            {
	            return;
            }

	        foreach (var child in dir.Children)
            {
                var entry = child.Value as FileEntry;
                if (entry != null)
                {
                    var name = entry.Name.ToLowerInvariant();
                    if (name.Contains("_s.blp") || name.Contains("_h.blp") || !name.Contains(".blp"))
                    {
	                    continue;
                    }

	                this.mTilesets.Add((curDir + "\\" + entry.Name).ToLowerInvariant());
                }
                else
                {
                    dir = (DirectoryEntry) child.Value;
                    HandleTilesetDirectory(dir, curDir + "\\" + dir.Name);
                }
            }
        }

        private void OnWorldClick(IntersectionParams intersectionParams, MouseEventArgs args)
        {
	        if (!args.Mouse.IsButtonDown(MouseButton.Left))
	        {
		        return;
	        }

	        KeyboardState keyboardState = Keyboard.GetState();
	        if (keyboardState.IsKeyDown(Key.ControlLeft) || keyboardState.IsKeyDown(Key.ShiftLeft))
	        {
		        return;
	        }

            if (intersectionParams.ChunkHit == null)
            {
	            this.mLastArea = null;
                return;
            }

            MapArea area;
	        if (intersectionParams.ChunkHit.Parent.TryGetTarget(out area) == false)
	        {
		        return;
	        }

            var updateArea = true;
            if (this.mLastArea != null)
            {
                MapArea lastArea;
	            if (this.mLastArea.TryGetTarget(out lastArea) && lastArea == area)
	            {
		            updateArea = false;
	            }

            }

	        this.mLastArea = intersectionParams.ChunkHit.Parent;
	        if (updateArea)
	        {
		        SetSelectedTileTextures(this.mWidget.SelectedTileWrapPanel, area.TextureNames);
	        }

            MapChunk lastChunk;
            if (this.mLastChunk != null && this.mLastChunk.TryGetTarget(out lastChunk))
            {
	            if (lastChunk == intersectionParams.ChunkHit)
	            {
		            return;
	            }
            }

	        this.mLastChunk = new WeakReference<MapChunk>(intersectionParams.ChunkHit);
            SetSelectedTileTextures(this.mWidget.SelectedChunkWrapPanel, intersectionParams.ChunkHit.TextureNames);
        }

        private void OnTextureSelected(PictureBox box)
        {
            var texName = box.Tag as string;
	        if (string.IsNullOrEmpty(texName))
	        {
		        return;
	        }

	        this.mWidget.TexturePreviewImage.Image = box.Image;
	        this.mWidget.TexturePreviewImage.Tag = texName;
            Editing.TextureChangeManager.Instance.SelectedTexture = texName;
            AddRecentTexture(texName);

            var nameLow = texName.ToLowerInvariant();
	        this.mWidget.FavoriteButton.Content = this.mFavoriteTextures.Contains(nameLow) ? "Remove Favorite" : "Add Favorite";
	        this.mWidget.FavoriteButton.IsEnabled = true;
        }

        private void SetEventHandlers(PictureBox box)
        {
            box.Cursor = Cursors.Hand;
            box.MouseEnter += (sender, args) =>
            {
                var panel = box.Parent as Panel;
	            if (panel == null)
	            {
		            return;
	            }

                panel.BackColor = Color.Black;
            };

            box.MouseLeave += (sender, args) =>
            {
                var panel = box.Parent as Panel;
	            if (panel == null)
	            {
		            return;
	            }

                panel.BackColor = Color.Transparent;
            };

            box.Click += (sender, args) => OnTextureSelected(box);
        }
    }
}
