using System;
using SharpDX;
using SharpDX.Direct2D1;

namespace WoWEditor6.UI.Components
{
    class MapSelectQuad : IComponent
    {
        private Vector2 mPosition;
        private Vector2 mSize;
        private RectangleF mRectangle;
        private readonly StaticText mTextDraw;
        private bool mIsHovered;
        private bool mIsClicked;
        private readonly SolidBrush mColor = Brushes.Solid[0xFF777777];
        private readonly SolidBrush mColorHover = Brushes.Solid[0xFFAAAAAA];

        public event Action<MapSelectQuad> Clicked;
        public object Tag { get; set; }

        public Vector2 Position { get { return mPosition; } set { ClientAreaUpdate(value, mSize); } }
        public Vector2 Size { get { return mSize; } set { ClientAreaUpdate(mPosition, value); } }

        public string Text { get { return mTextDraw.Text; } set { mTextDraw.Text = value; } }

        public MapSelectQuad()
        {
            mTextDraw = new StaticText
            {
                FontFamily = "Segoe UI",
                FontSize = 13.0f,
                Weight = SharpDX.DirectWrite.FontWeight.Bold
            };

            Size = new Vector2(96, 96);
        }

        public void OnRender(RenderTarget target)
        {
            var offset = mIsClicked ? new Vector2(1, 1) : new Vector2(0, 0);
            target.FillRectangle(
                !mIsClicked
                    ? mRectangle
                    : new RectangleF(Position.X + offset.X, Position.Y + offset.Y, mSize.X - 2 * offset.X, mSize.Y - 2 * offset.Y),
                mIsHovered ? mColorHover : mColor);

            target.PushAxisAlignedClip(new RectangleF(Position.X + 2, Position.Y + 2, mSize.X - 4, mSize.Y - 4),
                AntialiasMode.Aliased);
            target.DrawTextLayout(new Vector2(Position.X + 2 + offset.X, Position.Y + 2 + offset.Y), mTextDraw, mIsHovered ? Brushes.Black :  Brushes.White);
            target.PopAxisAlignedClip();
        }

        public void OnMessage(Message message)
        {
            var mouseMsg = message as MouseMessage;
            if (mouseMsg == null || message.IsHandled)
            {
                if (message.Type == MessageType.MouseDown || message.Type == MessageType.MouseUp)
                    mIsClicked = false;

                return;
            }

            switch (mouseMsg.Type)
            {
                case MessageType.MouseMove:
                    mIsHovered = mRectangle.Contains(mouseMsg.Position);
                    break;
                case MessageType.MouseDown:
                    mIsClicked = mRectangle.Contains(mouseMsg.Position);
                    break;

                case MessageType.MouseUp:
                    if (mIsClicked && mIsHovered)
                        Clicked?.Invoke(this);

                    mIsClicked = false;
                    break;
            }
        }

        private void ClientAreaUpdate(Vector2 position, Vector2 size)
        {
            mRectangle = new RectangleF(position.X, position.Y, size.X, size.Y);
            mTextDraw.Size = new Size2F(mSize.X - 4, mSize.Y - 4);
            mPosition = position;
            mSize = size;
        }
    }
}
