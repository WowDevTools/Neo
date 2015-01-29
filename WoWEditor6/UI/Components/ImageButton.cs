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
        private static Brush gBorder;
        private static Brush gHover;
        private static Brush gClick;

        private bool mIsHovered;
        private bool mIsClicked;
        private readonly BitmapImage mImage;

        private Vector2 mSize;
        private Vector2 mPosition;
        private RectangleF mImageRect;
        private RectangleF mTargetRect;

        public Vector2 Position { get { return mPosition; } set { ClientAreaChanged(value, mSize); } }
        public Vector2 Size { get { return mSize; } set { ClientAreaChanged(mPosition, value); } }

        public event Action<ImageButton> OnClick;
        public event Action<ImageButton> MouseEnter;
        public event Action<ImageButton> MouseLeave;

        public System.Drawing.Image Image
        {
            set
            {
                mImage.UpdateData(value);
            }
        }

        public ImageButton(Action<ImageButton> onClick = null)
        {
            if (onClick != null)
                OnClick += onClick;

            mImage = new BitmapImage(1, 1, new BitmapProperties()
            {
                DpiX = 96.0f,
                DpiY = 96.0f,
                PixelFormat =
                    new PixelFormat {AlphaMode = AlphaMode.Premultiplied, Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm}
            });
        }

        public void OnRender(RenderTarget target)
        {
            if (gBorder == null)
                InitBrushes();

            if(mIsHovered || mIsClicked)
            {
                if (mIsClicked)
                    target.FillRectangle(mTargetRect, gClick);
                else if (mIsHovered)
                    target.FillRectangle(mTargetRect, gHover);

                target.DrawRectangle(mTargetRect, gBorder);
            }

            target.DrawBitmap(mImage.GetBitmap(), mImageRect, 1.0f, BitmapInterpolationMode.Linear);
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
                    var isHover = mIsHovered;
                    mIsHovered = mTargetRect.Contains(msg.Position);
                    if (mIsHovered) msg.IsHandled = true;
                    if (mIsHovered && !isHover) MouseEnter?.Invoke(this);
                    if (mIsHovered == false && isHover) MouseLeave?.Invoke(this);
                    break;

                case MessageType.MouseDown:
                    mIsClicked = mTargetRect.Contains(msg.Position);
                    if (mIsClicked) msg.IsHandled = true;
                    break;

                case MessageType.MouseUp:
                    if(mIsClicked && mTargetRect.Contains(msg.Position))
                        OnClick?.Invoke(this);

                    if (mTargetRect.Contains(msg.Position)) msg.IsHandled = true;
                    mIsClicked = false;
                    break;
            }
        }

        private void ClientAreaChanged(Vector2 position, Vector2 size)
        {
            mSize = size;
            mPosition = position;

            mTargetRect = new RectangleF(position.X, position.Y, size.X, size.Y);
            mImageRect = new RectangleF(position.X + 4, position.Y + 4, size.X - 8, size.Y - 8);
        }

        private static void InitBrushes()
        {
            gBorder = Brushes.Solid[Color.GhostWhite];
            gHover = Brushes.Solid[0xCC666666];
            gClick = Brushes.Solid[0xCC888888];
        }
    }
}
