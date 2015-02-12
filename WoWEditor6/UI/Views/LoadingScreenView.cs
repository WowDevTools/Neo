using System;
using SharpDX;
using SharpDX.Direct2D1;
using WoWEditor6.Scene;
using WoWEditor6.UI.Components;

namespace WoWEditor6.UI.Views
{
    class LoadingScreenView : IView
    {
        private Vector2 mSize;
        private TextureBitmap mLoadingImage;
        private TextureBitmap mLoadingBarBackground;
        private TextureBitmap mLoadingBarFill;
        private RectangleF mTargetRectangle;
        private float mProgress;

        public void OnRender(RenderTarget target)
        {
            target.FillRectangle(new RectangleF(0, 0, mSize.X, mSize.Y), Brushes.Solid[0xFF333333]);
            if (mLoadingImage == null || mLoadingImage.IsLoaded == false)
                return;

            target.DrawBitmap(mLoadingImage, mTargetRectangle, 1.0f, BitmapInterpolationMode.Linear);

            if (mLoadingBarBackground == null || !mLoadingBarBackground.IsLoaded) return;

            var startPosY = mTargetRectangle.Y + mTargetRectangle.Height * 0.8f + 13;
            var startPosX = mTargetRectangle.X + (mTargetRectangle.Width - mLoadingBarBackground.Width * 1.2f + 80) / 2.0f;
            target.DrawBitmap(mLoadingBarFill,
                new RectangleF(startPosX, startPosY, (mLoadingBarBackground.Width * 1.2f - 80.0f) * mProgress, 40),
                1.0f, BitmapInterpolationMode.Linear);

            startPosY = mTargetRectangle.Y + mTargetRectangle.Height * 0.8f;
            startPosX = mTargetRectangle.X + (mTargetRectangle.Width  - mLoadingBarBackground.Width * 1.2f) / 2.0f;
            target.DrawBitmap(mLoadingBarBackground,
                new RectangleF(startPosX, startPosY, mLoadingBarBackground.Width * 1.2f, mLoadingBarBackground.Height),
                1.0f, BitmapInterpolationMode.Linear);
        }

        public void OnMessage(Message message)
        {

        }

        public void OnResize(Vector2 newSize)
        {
            mSize = newSize;
            if (mLoadingImage != null && mLoadingImage.IsLoaded)
                ImageLoaded(mLoadingImage);

        }

        public void UpdateProgress(float progress)
        {
            mProgress = progress;
        }

        public void OnShow()
        {
            var mapId = WorldFrame.Instance.MapManager.MapId;
            var mapRow = Storage.DbcStorage.Map.GetRowById(mapId);
            if(mapRow == null)
            {
                WorldFrame.Instance.State = AppState.MapSelect;
                return;
            }

            var loadScreenPath = "Interface\\Glues\\loading.blp";
            var loadEntry = mapRow.GetInt32(Storage.MapFormatGuess.FieldMapLoadingScreen);
            if(loadEntry != 0)
            {
                var loadRow = Storage.DbcStorage.LoadingScreen.GetRowById(loadEntry);
                if(loadRow != null)
                {
                    var path = loadRow.GetString(Storage.MapFormatGuess.FieldLoadingScreenPath);
                    if (loadRow.GetInt32(Storage.MapFormatGuess.FieldLoadingScreenHasWidescreen) == 1)
                    {
                        path = path.Replace(".BLP", "WIDE.BLP");
                    }
                    if (string.IsNullOrEmpty(path) == false)
                        loadScreenPath = path;
                }
            }

            if (mLoadingImage != null)
                mLoadingImage.Dispose();
            mLoadingImage = new TextureBitmap();
            mLoadingImage.LoadComplete += ImageLoaded;
            mLoadingImage.LoadFromFile(loadScreenPath);

            if(mLoadingBarBackground == null)
            {
                mLoadingBarBackground = new TextureBitmap();
                mLoadingBarBackground.LoadFromFile(@"Interface\Glues\LoadingBar\Loading-BarBorder.blp");
                mLoadingBarFill = new TextureBitmap();
                mLoadingBarFill.LoadFromFile(@"Interface\Glues\LoadingBar\Loading-BarFill.blp");
            }
        }

        private void ImageLoaded(TextureBitmap bmp)
        {
            var facx = mSize.X / bmp.Width;
            var facy = mSize.Y / bmp.Height;
            var fac = Math.Min(facx, facy);
            var newWidth = fac * bmp.Width;
            var newHeight = fac * bmp.Height;
            var ofsx = (mSize.X - newWidth) / 2.0f;
            var ofsy = (mSize.Y - newHeight) / 2.0f;
            mTargetRectangle = new RectangleF(0, 0, mSize.X, mSize.Y);
        }
    }
}
