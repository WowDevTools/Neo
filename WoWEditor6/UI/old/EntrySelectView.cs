using SharpDX;
using SharpDX.Direct2D1;
using WoWEditor6.Scene;
using WoWEditor6.UI.Components;

namespace WoWEditor6.UI.Views
{
    class EntrySelectView : IView
    {
        private IO.Files.DbcRecord mMapRecord;
        private readonly WdlControl mWdlControl = new WdlControl();
        private Vector2 mSize;
        private readonly Button mBackButton;

        public EntrySelectView()
        {
            mBackButton = new Button
            {
                Size = new Vector2(75, 25),
                Text = "Back"
            };

            mBackButton.OnClick += OnBackButtonClick;
            mWdlControl.LocationSelected += OnLocationSelected;
        }

        public void SetSelectedMap(IO.Files.DbcRecord record)
        {
            mMapRecord = record;
        }

        public void OnRender(RenderTarget target)
        {
            target.FillRectangle(new RectangleF(0, 0, mSize.X, mSize.Y), Brushes.Solid[0xFF333333]);
            mWdlControl.OnRender(target);
            mBackButton.OnRender(target);
        }

        public void OnMessage(Message message)
        {
            mBackButton.OnMessage(message);
            mWdlControl.OnMessage(message);
        }

        public void OnResize(Vector2 newSize)
        {
            mSize = newSize;
            mWdlControl.Size = new Vector2(newSize.X - 40, newSize.Y - 70);
            mBackButton.Position = new Vector2(newSize.X - 20 - mBackButton.Size.X, newSize.Y - 20 - mBackButton.Size.Y);
        }

        public void OnShow()
        {
            if (mMapRecord == null)
                return;

            mWdlControl.UpdateMap(mMapRecord.GetString(Storage.MapFormatGuess.FieldMapName));
            mWdlControl.Position = new Vector2(20, 50);
        }

        private static void OnBackButtonClick(Button button)
        {
            WorldFrame.Instance.State = AppState.MapSelect;
        }

        private void OnLocationSelected(Vector2 location)
        {
            var mapId = mMapRecord.GetInt32(0);
            WorldFrame.Instance.MapManager.EnterWorld(location, mapId);
        }
    }
}
