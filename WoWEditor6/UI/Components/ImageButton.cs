using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;

namespace WoWEditor6.UI.Components
{
    class ImageButton : IComponent
    {
        private static TextureBitmap gHighlightImage;
        private static TextureBitmap gClickImage;

        private bool mIsHovered;
        private bool mIsClicked;
        private TextureBitmap mImage;

        private Vector2 mSize;
        private Vector2 mPosition;
        private RectangleF mImageRect;

        public Vector2 Position { get { return mPosition; } set { ClientAreaChanged(value, mSize); } }
        public Vector2 Size { get { return mSize; } set { ClientAreaChanged(mPosition, value); } }

        public string Image
        {
            set
            {
                lock (this)
                {
                    mImage?.Dispose();
                    mImage = new TextureBitmap();
                    mImage.LoadFromFile(value);
                }
            }
        }

        public void OnRender(RenderTarget target)
        {
            if (gClickImage == null)
                InitBrushes();

            lock(this)
            {
                
                var loaded = mImage?.IsLoaded ?? false;
                loaded = loaded && (gHighlightImage?.IsLoaded ?? false) && (gClickImage?.IsLoaded ?? false);
                if (!loaded)
                    return;

                target.DrawBitmap(mImage, mImageRect, 1.0f, BitmapInterpolationMode.Linear);
                if (mIsClicked)
                    target.DrawBitmap(gClickImage, mImageRect, 1.0f, BitmapInterpolationMode.Linear);
                else if (mIsHovered)
                    target.DrawBitmap(gHighlightImage, mImageRect, 1.0f, BitmapInterpolationMode.Linear);
            }
        }

        public void OnMessage(Message message)
        {
            if (message.IsHandled)
                return;

            var msg = message as MouseMessage;
            if (msg == null)
                return;

            switch(msg.Type)
            {
                case MessageType.MouseMove:
                    mIsHovered = mImageRect.Contains(msg.Position);
                    if (mIsHovered) msg.IsHandled = true;
                    break;

                case MessageType.MouseDown:
                    mIsClicked = mImageRect.Contains(msg.Position);
                    if (mIsClicked) msg.IsHandled = true;
                    break;

                case MessageType.MouseUp:
                    if(mIsClicked && mImageRect.Contains(msg.Position))
                    {

                    }
                    if (mImageRect.Contains(msg.Position)) msg.IsHandled = true;
                    mIsClicked = false;
                    break;
            }
        }

        private void ClientAreaChanged(Vector2 position, Vector2 size)
        {
            mSize = size;
            mPosition = position;

            mImageRect = new RectangleF(position.X, position.Y, size.X, size.Y);
        }

        private void InitBrushes()
        {
            gHighlightImage = new TextureBitmap();
            gHighlightImage.OnBeforeLoad += (img, data) =>
            {
                for (var i = 0; i < data.Length; i += 4)
                    data[i + 3] = 0;
            };
            gHighlightImage.LoadFromFile(@"Interface\Buttons\ButtonHilight-Square.blp");

            gClickImage = new TextureBitmap();
            gClickImage.OnBeforeLoad += (img, data) =>
            {
                for (var i = 0; i < data.Length; i += 4)
                    data[i + 3] = 0;
            };

            gClickImage.LoadFromFile(@"Interface\Buttons\CheckButtonHilight.blp");
        }
    }
}
