using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using WoWEditor6.IO.Files.Terrain;
using WoWEditor6.IO.Files.Texture;
using WoWEditor6.Scene;
using WoWEditor6.UI.Dialogs;
using WoWEditor6.Utils;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace WoWEditor6.UI.Models
{
    class TexturingViewModel
    {
        private readonly TexturingWidget mWidget;
        private bool mIsValueChangedSurpressed;
        private WeakReference<MapArea> mLastArea;
        private WeakReference<MapChunk> mLastChunk;

        public TexturingWidget Widget { get { return mWidget; } }

        public TexturingViewModel(TexturingWidget widget)
        {
            if (EditorWindowController.Instance != null)
                EditorWindowController.Instance.TexturingModel = this;

            mWidget = widget;
            if (WorldFrame.Instance != null)
                WorldFrame.Instance.OnWorldClicked += OnWorldClick;
        }

        private unsafe void SetSelectedTileTextures(FlowLayoutPanel panel, IEnumerable<string> textures)
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

            new Thread(() =>
            {
                foreach (var pair in loadTasks)
                {
                    var img = pair.Item2;
                    var loadInfo = TextureLoader.LoadToArgbImage(pair.Item1);
                    if (loadInfo == null)
                        continue;

                    var bmp = new Bitmap(loadInfo.Width, loadInfo.Height, PixelFormat.Format32bppArgb);
                    var bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly,
                        PixelFormat.Format32bppArgb);

                    fixed (byte* ptr = loadInfo.Layers[0])
                        UnsafeNativeMethods.CopyMemory((byte*) bmpd.Scan0, ptr, bmp.Width * bmp.Height * 4);

                    bmp.UnlockBits(bmpd);

                    EditorWindowController.Instance.WindowDispatcher.BeginInvoke(new Action(() =>
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

        public void HandleInnerRadiusChanged(float newRadius)
        {
            if (mIsValueChangedSurpressed)
                return;

            mWidget.InnerRadiusSlider.Value = newRadius;
        }

        public void HandleOuterRadiusChanged(float newRadius)
        {
            if (mIsValueChangedSurpressed)
                return;

            mWidget.OuterRadiusSlider.Value = newRadius;
        }

        public void HandleAmoutChanged(float newAmount)
        {
            if (mIsValueChangedSurpressed)
                return;

            mWidget.AmountSlider.Value = newAmount;
        }

        public void SwitchToTexturing()
        {
            Editing.EditManager.Instance.EnableTexturing();
        }

        private void OnWorldClick(IntersectionParams intersectionParams, MouseEventArgs args)
        {
            if (args.Button != MouseButtons.Left)
                return;

            var keys = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(keys);
            if (KeyHelper.IsKeyDown(keys, Keys.ControlKey) || KeyHelper.IsKeyDown(keys, Keys.ShiftKey))
                return;

            if (intersectionParams.ChunkHit == null)
            {
                mLastArea = null;
                return;
            }

            MapArea area;
            if (intersectionParams.ChunkHit.Parent.TryGetTarget(out area) == false)
                return;

            var updateArea = true;
            if (mLastArea != null)
            {
                MapArea lastArea;
                if (mLastArea.TryGetTarget(out lastArea) && lastArea == area)
                    updateArea = false;

            }

            mLastArea = intersectionParams.ChunkHit.Parent;
            if (updateArea)
                SetSelectedTileTextures(mWidget.SelectedTileWrapPanel, area.TextureNames);

            MapChunk lastChunk;
            if (mLastChunk != null && mLastChunk.TryGetTarget(out lastChunk))
            {
                if (lastChunk == intersectionParams.ChunkHit)
                    return;
            }

            mLastChunk = new WeakReference<MapChunk>(intersectionParams.ChunkHit);
            SetSelectedTileTextures(mWidget.SelectedChunkWrapPanel, intersectionParams.ChunkHit.TextureNames);
        }

        private void OnTextureSelected(PictureBox box)
        {
            var texName = box.Tag as string;
            if (string.IsNullOrEmpty(texName))
                return;

            mWidget.TexturePreviewImage.Image = box.Image;
            Editing.TextureChangeManager.Instance.SelectedTexture = texName;
        }

        private void SetEventHandlers(PictureBox box)
        {
            box.Cursor = Cursors.Hand;
            box.MouseEnter += (sender, args) =>
            {
                var panel = box.Parent as Panel;
                if (panel == null)
                    return;

                panel.BackColor = Color.Black;
            };

            box.MouseLeave += (sender, args) =>
            {
                var panel = box.Parent as Panel;
                if (panel == null)
                    return;

                panel.BackColor = Color.Transparent;
            };

            box.Click += (sender, args) => OnTextureSelected(box);
        }
    }
}
