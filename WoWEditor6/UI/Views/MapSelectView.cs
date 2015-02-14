using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using WoWEditor6.Scene;
using WoWEditor6.UI.Components;

namespace WoWEditor6.UI.Views
{
    class MapSelectView : IView
    {
        private readonly List<MapSelectQuad> mMapLabels = new List<MapSelectQuad>();
        private readonly Label mTitleLabel;
        private readonly Scrollbar mLabelScroll;
        private readonly Checkbox mAlphabeticCheckbox;
        private readonly EditBox mFilterBox;
        private readonly Label mFilterDesc;
        private Vector2 mSize;
        private string mFilter = string.Empty;

        public MapSelectView()
        {
            mTitleLabel = new Label
            {
                FontSize = 30.0f,
                Position = new Vector2(30.0f, 15.0f),
                Size = new Vector2(float.MaxValue, 40.0f),
                Text = "Please select the map you want to edit"
            };

            mLabelScroll = new Scrollbar
            {
                Vertical = true
            };

            mAlphabeticCheckbox = new Checkbox
            {
                Checked = true,
                Text = "Sort alphabetically",
                Size = 16
            };

            mFilterBox = new EditBox
            {
                Width = 200
            };

            mFilterDesc = new Label
            {
                Color =  Brushes.White,
                FontSize = 16,
                HorizontalAlignment = TextAlignment.Leading,
                VerticalAlignment = ParagraphAlignment.Center,
                Size = new Vector2(float.MaxValue, 18),
                Multiline = false,
                Text = "Filter maps:"
            };

            mFilterBox.TextChanged += OnUpdateFilterText;

            mAlphabeticCheckbox.CheckChanged += (box, check) => OnChangeAlphabeticalOrder(check);
            mLabelScroll.ScrollChanged += OnScroll;
        }

        public void OnRender(RenderTarget target)
        {
            target.FillRectangle(new RectangleF(0, 0, mSize.X, mSize.Y), Brushes.Solid[0xFF333333]);

            target.PushAxisAlignedClip(new RectangleF(0, 100, mSize.X, mSize.Y - 100), AntialiasMode.Aliased);
            foreach (var label in mMapLabels)
                label.OnRender(target);
            target.PopAxisAlignedClip();

            target.DrawLine(new Vector2(0, 100.0f), new Vector2(mSize.X, 100.0f), Brushes.White);

            mTitleLabel.OnRender(target);
            mLabelScroll.OnRender(target);

            mAlphabeticCheckbox.OnRender(target);
            mFilterBox.OnRender(target);
            mFilterDesc.OnRender(target);
        }

        public void OnMessage(Message message)
        {
            if(message.Type == MessageType.MouseWheel)
            {
                var msg = message as MouseMessage;
                if (msg != null)
                    mLabelScroll.OnScroll(msg.Delta * 30);
            }

            for (var i = mMapLabels.Count - 1; i >= 0; --i)
                mMapLabels[i].OnMessage(message);

            mAlphabeticCheckbox.OnMessage(message);
            mFilterBox.OnMessage(message);

            mLabelScroll.OnMessage(message);
        }

        public void OnResize(Vector2 newSize)
        {
            mSize = newSize;
            var numHoriz = (int)Math.Floor((mSize.X - 60.0f) / 103.0f);

            for (var i = 0; i < mMapLabels.Count; ++i)
            {
                var label = mMapLabels[i];
                // ReSharper disable once PossibleLossOfFraction
                label.Position = new Vector2((i % numHoriz) * 103.0f + 30.0f, (i / numHoriz) * 103.0f + 103);
            }

            var numRows = mMapLabels.Count / numHoriz;
            if ((mMapLabels.Count % numHoriz) != 0)
                ++numRows;

            mLabelScroll.TotalSize = numRows * 103.0f + 6.0f;
            mLabelScroll.VisibleSize = mSize.Y - 100;

            mLabelScroll.Position = new Vector2(newSize.X - 15.0f, 102.0f);
            mLabelScroll.Size = newSize.Y - 104.0f;

            mAlphabeticCheckbox.Position = new Vector2(newSize.X - 200, 60);
            mFilterBox.Position = new Vector2(newSize.X - 450, 55);
            mFilterDesc.Position = new Vector2(newSize.X - 450, 30);
        }

        private void OnUpdateFilterText(EditBox box, string text)
        {
            mFilter = text;
            mMapLabels.Clear();

            var numHoriz = (int)Math.Floor((mSize.X - 60.0f) / 103.0f);

            for (int i = 0, j = 0; i < Storage.DbcStorage.Map.NumRows; ++i)
            {
                var row = Storage.DbcStorage.Map.GetRow(i);
                var title = row.GetString(Storage.MapFormatGuess.FieldMapTitle);
                if (string.IsNullOrEmpty(mFilter) == false && title.ToLowerInvariant().Contains(mFilter.ToLowerInvariant()) == false)
                    continue;

                mMapLabels.Add(new MapSelectQuad
                {
                    // ReSharper disable once PossibleLossOfFraction
                    Position = new Vector2((j % numHoriz) * 103.0f + 30.0f, (j / numHoriz) * 103.0f + 103),
                    Size = new Vector2(96.0f, 96.0f),
                    Text = title,
                    Tag = row
                });

                mMapLabels[j].Clicked += MapSelected;
                ++j;
            }

            if (mAlphabeticCheckbox.Checked)
            {
                mMapLabels.Sort((a, b) => String.Compare(a.Text, b.Text, StringComparison.Ordinal));
                for (var i = 0; i < mMapLabels.Count; ++i)
                {
                    var lbl = mMapLabels[i];
                    // ReSharper disable once PossibleLossOfFraction
                    lbl.Position = new Vector2((i % numHoriz) * 103.0f + 30.0f, (i / numHoriz) * 103.0f + 103);
                }
            }

            var numRows = mMapLabels.Count / numHoriz;
            if ((mMapLabels.Count % numHoriz) != 0)
                ++numRows;

            mLabelScroll.TotalSize = numRows * 103.0f + 6.0f;
            mLabelScroll.VisibleSize = mSize.Y - 100;
        }

        private void OnChangeAlphabeticalOrder(bool enabled)
        {
            mMapLabels.Clear();

            var numHoriz = (int)Math.Floor((mSize.X - 60.0f) / 103.0f);

            for (int i = 0, j = 0; i < Storage.DbcStorage.Map.NumRows; ++i)
            {
                var row = Storage.DbcStorage.Map.GetRow(i);
                var title = row.GetString(Storage.MapFormatGuess.FieldMapTitle);
                if (string.IsNullOrEmpty(mFilter) == false && title.ToLowerInvariant().Contains(mFilter.ToLowerInvariant()) == false)
                    continue;

                mMapLabels.Add(new MapSelectQuad
                {
                    // ReSharper disable once PossibleLossOfFraction
                    Position = new Vector2((j % numHoriz) * 103.0f + 30.0f, (j / numHoriz) * 103.0f + 103),
                    Size = new Vector2(96.0f, 96.0f),
                    Text = title,
                    Tag = row
                });

                mMapLabels[j].Clicked += MapSelected;
                ++j;
            }

            if (enabled)
            {
                mMapLabels.Sort((a, b) => String.Compare(a.Text, b.Text, StringComparison.Ordinal));
                for (var i = 0; i < mMapLabels.Count; ++i)
                {
                    var lbl = mMapLabels[i];
                    // ReSharper disable once PossibleLossOfFraction
                    lbl.Position = new Vector2((i % numHoriz) * 103.0f + 30.0f, (i / numHoriz) * 103.0f + 103);
                }
            }

            var numRows = mMapLabels.Count / numHoriz;
            if ((mMapLabels.Count % numHoriz) != 0)
                ++numRows;

            mLabelScroll.TotalSize = numRows * 103.0f + 6.0f;
            mLabelScroll.VisibleSize = mSize.Y - 100;
        }

        public void OnShow()
        {
            if (mMapLabels.Count > 0)
                return;

            Storage.DbcStorage.Initialize();

            var numHoriz = (int) Math.Floor((mSize.X - 60.0f) / 103.0f);

            for (int i = 0, j = 0; i < Storage.DbcStorage.Map.NumRows; ++i)
            {
                var row = Storage.DbcStorage.Map.GetRow(i);
                var title = row.GetString(Storage.MapFormatGuess.FieldMapTitle);
                if (string.IsNullOrEmpty(mFilter) == false && title.ToLowerInvariant().Contains(mFilter.ToLowerInvariant()) == false)
                    continue;

                mMapLabels.Add(new MapSelectQuad
                {
                    // ReSharper disable once PossibleLossOfFraction
                    Position = new Vector2((j % numHoriz) * 103.0f + 30.0f, (j / numHoriz) * 103.0f + 103),
                    Size = new Vector2(96.0f, 96.0f),
                    Text = title,
                    Tag = row
                });

                mMapLabels[j].Clicked += MapSelected;
                ++j;
            }

            mMapLabels.Sort((a, b) => String.Compare(a.Text, b.Text, StringComparison.Ordinal));
            for (var i = 0; i < mMapLabels.Count; ++i)
            {
                var lbl = mMapLabels[i];
                // ReSharper disable once PossibleLossOfFraction
                lbl.Position = new Vector2((i % numHoriz)*103.0f + 30.0f, (i/numHoriz) * 103.0f + 103);
            }

            var numRows = mMapLabels.Count / numHoriz;
            if ((mMapLabels.Count % numHoriz) != 0)
                ++numRows;

            mLabelScroll.TotalSize = numRows * 103.0f + 6.0f;
            mLabelScroll.VisibleSize = mSize.Y - 100;
        }

        private void OnScroll(float offset)
        {
            var numHoriz = (int)Math.Floor((mSize.X - 60.0f) / 103.0f);

            for (var i = 0; i < mMapLabels.Count; ++i)
            {
                var label = mMapLabels[i];
                // ReSharper disable once PossibleLossOfFraction
                label.Position = new Vector2((i % numHoriz) * 103.0f + 30.0f, (i / numHoriz) * 103.0f + 103 - offset);
            }
        }

        private void MapSelected(MapSelectQuad quad)
        {
            var row = quad.Tag as IO.Files.DbcRecord;
            if (row == null)
                return;

            var esView = InterfaceManager.Instance.GetViewForState<EntrySelectView>(AppState.EntrySelect);
            if (esView != null)
                esView.SetSelectedMap(row);
            WorldFrame.Instance.State = AppState.EntrySelect;
        }
    }
}
