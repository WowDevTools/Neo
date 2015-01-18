using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;
using WoWEditor6.Scene;
using WoWEditor6.Scene.Terrain;
using WoWEditor6.UI.Components;

namespace WoWEditor6.UI.Views
{
    class LoadingScreenView : IView
    {
        private Vector2 mSize;
        private TextureBitmap mLoadingImage;
        private RectangleF mTargetRectangle;

        public void OnRender(RenderTarget target)
        {
            target.FillRectangle(new RectangleF(0, 0, mSize.X, mSize.Y), Brushes.Solid[0xFF333333]);
            if (mLoadingImage == null || mLoadingImage.IsLoaded == false)
                return;

            target.DrawBitmap(mLoadingImage, mTargetRectangle, 1.0f, BitmapInterpolationMode.Linear);
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
                    if (string.IsNullOrEmpty(path) == false)
                        loadScreenPath = path;
                }
            }

            mLoadingImage?.Dispose();
            mLoadingImage = new TextureBitmap();
            mLoadingImage.LoadComplete += ImageLoaded;
            mLoadingImage.LoadFromFile(loadScreenPath);
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
            mTargetRectangle = new RectangleF(ofsx, ofsy, newWidth, newHeight);
        }
    }
}
