using System;
using SharpDX;
using SharpDX.Direct2D1;

namespace Neo.UI.Components
{
    class Button : IComponent
    {
        private static SolidBrush gBackground;
        private static SolidBrush gBackgroundHover;
        private static SolidBrush gBackgroundClick;
        private static SolidBrush gBorder;
        private static SolidBrush gCaptionColor;

        private Vector2 mPosition;
        private Vector2 mSize;
        private RectangleF mClientRectangle;

        private bool mIsHovered;
        private bool mIsClicked;
        private readonly StaticText mTextDraw;

        public event Action<Button> OnClick;

        public string Text { get { return mTextDraw.Text; } set { mTextDraw.Text = value; } }

        public Vector2 Position
        {
            get { return mPosition; }
            set
            {
                mPosition = value;
                ClientAreaChanged(mPosition, mSize);
            }
        }

        public Vector2 Size
        {
            get { return mSize; }
            set
            {
                mSize = value;
                ClientAreaChanged(mPosition, mSize);
            }
        }

        public Button()
        {
            mTextDraw = new StaticText()
            {
                Weight = SharpDX.DirectWrite.FontWeight.Bold,
                FontSize = 14.0f,
                VerticalAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center,
                HorizontalAlignment = SharpDX.DirectWrite.TextAlignment.Center
            };

            Size = new Vector2(115, 25);
        }

        public void OnRender(RenderTarget target)
        {
            var brush = gBackground;
            if (mIsClicked)
                brush = gBackgroundClick;
            else if (mIsHovered)
                brush = gBackgroundHover;

            target.FillRectangle(mClientRectangle, gBorder);
            target.FillRectangle(new RectangleF(Position.X + 1, Position.Y + 1, Size.X - 2, Size.Y - 2), brush);
            target.DrawTextLayout(new Vector2(Position.X + 2, Position.Y + 2), mTextDraw, gCaptionColor, DrawTextOptions.Clip);
        }

        public void OnMessage(Message message)
        {
            var msg = message as MouseMessage;
            if (msg == null)
                return;

            switch(msg.Type)
            {
                case MessageType.MouseMove:
                    if (msg.IsHandled == false)
                    {
                        mIsHovered = mClientRectangle.Contains(msg.Position);
                        if (mIsHovered)
                            msg.IsHandled = true;
                    }
                    break;

                case MessageType.MouseDown:
                    {
                        if(msg.IsHandled == false)
                        {
                            mIsClicked = mClientRectangle.Contains(msg.Position);
                            if (mIsClicked)
                                msg.IsHandled = true;
                        }
                    }
                    break;

                case MessageType.MouseUp:
                    {
                        if(msg.IsHandled == false)
                        {
                            if (mClientRectangle.Contains(msg.Position) && mIsClicked)
                            {
                                if (OnClick != null)
                                    OnClick(this);
                            }
                        }

                        mIsClicked = false;
                    }
                    break;
            }
        }

        public static void Initialize()
        {
            gBackground = Brushes.Solid[0xFF803F00];
            gBorder = Brushes.Solid[0xFFBBBBBB];
            gBackgroundHover = Brushes.Solid[0xFFB35900];
            gBackgroundClick = Brushes.Solid[0xFFFF7F00];
            gCaptionColor = Brushes.Solid[System.Drawing.Color.GhostWhite];
        }

        private void ClientAreaChanged(Vector2 position, Vector2 size)
        {
            mClientRectangle = new RectangleF(position.X, position.Y, size.X, size.Y);
            mTextDraw.Size = new Size2F(size.X - 4, size.Y - 4);
        }

    }
}
