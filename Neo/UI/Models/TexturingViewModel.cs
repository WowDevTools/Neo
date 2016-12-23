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
    class TexturingViewModel
    {
        private readonly TexturingWidget mWidget;
        private bool mIsValueChangedSurpressed;
        private WeakReference<MapArea> mLastArea;
        private WeakReference<MapChunk> mLastChunk;
        private readonly List<string> mRecentTextures = new List<string>();
        private readonly List<string> mFavoriteTextures = new List<string>();
        private readonly List<string> mTilesets = new List<string>();

        public TexturingWidget Widget { get { return mWidget; } }

        public TexturingViewModel(TexturingWidget widget)
        {
            if (EditorWindowController.Instance != null)
            {
	            EditorWindowController.Instance.TexturingModel = this;
            }

	        mWidget = widget;
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
            mIsValueChangedSurpressed = true;
            Editing.TextureChangeManager.Instance.Amount = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleInnerRadiusSlider(float value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.InnerRadius = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleOuterRadiusSlider(float value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.OuterRadius = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandlePenSensivity(float value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.PenSensivity = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleTabletControl(bool value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.IsTabletOn = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleSprayMode(bool value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.IsSprayOn = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleTabletChangeRadius(bool value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.IsTablet_RChange = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleTabletChangeInnerRadius(bool value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.IsTablet_IRChange = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleTabletControlPressure(bool value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.IsTablet_PChange = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleAllowedAmplitude(float value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.Amplitude = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleAllowedInnerAmplitude(float value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.InnerAmplitude = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleParticleSize(float value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.SprayParticleSize = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleParticleAmount(float value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.SprayParticleAmount = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleParticleHardness(float value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.SprayParticleHarndess = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleSpraySolidInnerRadius(bool value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.IsSpraySolidInnerRadius = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleInnerRadiusChanged(float newRadius)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.InnerRadiusSlider.Value = newRadius;
        }

        public void HandleOuterRadiusChanged(float newRadius)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.OuterRadiusSlider.Value = newRadius;
        }

        public void HandleAmoutChanged(float newAmount)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.AmountSlider.Value = newAmount;
        }

        public void HandleOpacityChanged(float newOpacity)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.GradientSlider.Value = newOpacity;
        }

        public void HandlePenSensivityChanged(float newSensivity)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.Tablet_SensivitySlider.Value = newSensivity;
        }

        public void HandleTabletControlChanged(bool newIsTabletChanged)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.TabletControlBox.IsChecked = newIsTabletChanged;
        }

        public void HandleSprayModeChanged(bool newIsSprayOn)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.SprayModeBox.IsChecked = newIsSprayOn;
        }

        public void HandleTabletChangeRadiusChanged(bool newIsTablet_RChanged)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.TabletControlBox_Radius.IsChecked = newIsTablet_RChanged;
        }

        public void HandleTabletChangeInnerRadiusChanged(bool newIsTablet_IRChanged)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.TabletControlBox_InnerRadius.IsChecked = newIsTablet_IRChanged;
        }

        public void HandleTabletControlPressureChanged(bool newIsTablet_PChanged)
        {
            if(mIsValueChangedSurpressed)
            {
	            return;
            }
	        mWidget.PressureControlBox.IsChecked = newIsTablet_PChanged;
        }

        public void HandleAllowedAmplitudeChanged(float newAmplitude )
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }
	        mWidget.Tablet_RadiusSlider.Value = newAmplitude;
        }

        public void HandleAllowedInnerAmplitudeChanged(float newInnerAmplitude)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }
	        mWidget.Tablet_InnerRadiusSlider.Value = newInnerAmplitude;
        }

        public void HandleParticleSizeChanged(float newSprayParticleSize)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }
	        mWidget.Spray_ParticleSizeSlider.Value = newSprayParticleSize;
        }

        public void HandleParticleAmountChanged(float newSprayParticleAmount)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.Spray_ParticleAmountSlider.Value = newSprayParticleAmount;
        }

        public void HandleParticleHardnessChanged(float newSprayParticleHardness)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.Spray_ParticleHardnessSlider.Value = newSprayParticleHardness;
        }

        public void HandleSpraySolidInnerRadiusChanged(bool newIsSpraySolidInnerRadius)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }
	        mWidget.SpraySolidInnerRadiusBox.IsChecked = newIsSpraySolidInnerRadius;
        }

        public void SwitchToTexturing()
        {
            Editing.EditManager.Instance.EnableTexturing();
        }

        public async void UpdateFilters()
        {
            var query = mWidget.TextureQueryBox.Text.ToLowerInvariant();
            if (string.IsNullOrEmpty(query) || query.Length < 4)
            {
	            return;
            }

	        var selectedFilters = (from CheckBox cb in mWidget.FilterWrapPanel.Children where cb.IsChecked ?? false select ((string)cb.Content).ToLowerInvariant()).ToList();

            mWidget.SearchResultLayout.Controls.Clear();
            var newValues =
                mTilesets.Where(s => s.Contains(query) && (selectedFilters.Count == 0 || selectedFilters.Any(s.Contains)));

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

            mWidget.SearchResultLayout.Controls.AddRange(toAdd.ToArray());
        }

        public async void SearchForTexture(string query)
        {
            if (string.IsNullOrEmpty(query) || query.Length < 4)
            {
	            return;
            }

	        var selectedFilters = (from CheckBox cb in mWidget.FilterWrapPanel.Children where cb.IsChecked ?? false select ((string) cb.Content).ToLowerInvariant()).ToList();

            mWidget.SearchResultLayout.Controls.Clear();
            query = query.ToLowerInvariant();
            var newValues =
                mTilesets.Where(s => s.Contains(query) && (selectedFilters.Count == 0 || selectedFilters.Any(s.Contains)));

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

            mWidget.SearchResultLayout.Controls.AddRange(toAdd.ToArray());
        }

        public async void OnFavoriteButtonClicked()
        {
            var curTexture = mWidget.TexturePreviewImage.Tag as string;
            if (string.IsNullOrEmpty(curTexture))
            {
	            return;
            }

	        curTexture = curTexture.ToLowerInvariant();
            var index = mFavoriteTextures.IndexOf(curTexture);
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
                mFavoriteTextures.Insert(0, curTexture);
                mWidget.FavoriteWrapPanel.Controls.Add(pnl);
                mWidget.FavoriteWrapPanel.Controls.SetChildIndex(pnl, 0);
                mWidget.FavoriteButton.Content = "Remove Favorite";
            }
            else
            {
                mWidget.FavoriteWrapPanel.Controls.RemoveAt(index);
                mFavoriteTextures.RemoveAt(index);
                mWidget.FavoriteButton.Content = "Add Favorite";
            }

            var coll = new StringCollection();
            coll.AddRange(mFavoriteTextures.ToArray());
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
            if (mRecentTextures.Contains(texture))
            {
                var index = mRecentTextures.IndexOf(texture);
                mRecentTextures.RemoveAt(index);
                mRecentTextures.Add(texture);
                var elem = mWidget.RecentWrapPanel.Controls[index];
                mWidget.RecentWrapPanel.Controls.SetChildIndex(elem, 0);
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

                mRecentTextures.Insert(0, texture);
                mWidget.RecentWrapPanel.Controls.Add(pnl);
                mWidget.RecentWrapPanel.Controls.SetChildIndex(pnl, 0);
            }

            if (initial == false)
            {
                var newColl = new StringCollection();
                newColl.AddRange(mRecentTextures.ToArray());
                Properties.Settings.Default.RecentTextures = newColl;
                Properties.Settings.Default.Save();
            }
        }

        private async void InitRecentTextures(IEnumerable textures)
        {
            foreach (string tex in textures)
            {
                if (mFavoriteTextures.Contains(tex.ToLowerInvariant()))
                {
	                continue;
                }

	            mFavoriteTextures.Add(tex.ToLowerInvariant());

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
                mWidget.FavoriteWrapPanel.Controls.Add(pnl);
            }
        }

        private void OnFilesLoaded()
        {
            var tilesetRoot = FileManager.Instance.FileListing.RootEntry.Children["tileset"] as DirectoryEntry;
            HandleTilesetDirectory(tilesetRoot, "Tileset");

            mWidget.Dispatcher.BeginInvoke(new Action(() =>
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

	                mTilesets.Add((curDir + "\\" + entry.Name).ToLowerInvariant());
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
                mLastArea = null;
                return;
            }

            MapArea area;
	        if (intersectionParams.ChunkHit.Parent.TryGetTarget(out area) == false)
	        {
		        return;
	        }

            var updateArea = true;
            if (mLastArea != null)
            {
                MapArea lastArea;
	            if (mLastArea.TryGetTarget(out lastArea) && lastArea == area)
	            {
		            updateArea = false;
	            }

            }

            mLastArea = intersectionParams.ChunkHit.Parent;
	        if (updateArea)
	        {
		        SetSelectedTileTextures(mWidget.SelectedTileWrapPanel, area.TextureNames);
	        }

            MapChunk lastChunk;
            if (mLastChunk != null && mLastChunk.TryGetTarget(out lastChunk))
            {
	            if (lastChunk == intersectionParams.ChunkHit)
	            {
		            return;
	            }
            }

            mLastChunk = new WeakReference<MapChunk>(intersectionParams.ChunkHit);
            SetSelectedTileTextures(mWidget.SelectedChunkWrapPanel, intersectionParams.ChunkHit.TextureNames);
        }

        private void OnTextureSelected(PictureBox box)
        {
            var texName = box.Tag as string;
	        if (string.IsNullOrEmpty(texName))
	        {
		        return;
	        }

            mWidget.TexturePreviewImage.Image = box.Image;
            mWidget.TexturePreviewImage.Tag = texName;
            Editing.TextureChangeManager.Instance.SelectedTexture = texName;
            AddRecentTexture(texName);

            var nameLow = texName.ToLowerInvariant();
            mWidget.FavoriteButton.Content = mFavoriteTextures.Contains(nameLow) ? "Remove Favorite" : "Add Favorite";
            mWidget.FavoriteButton.IsEnabled = true;
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
