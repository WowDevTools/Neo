using System;
using SharpDX;
using SharpDX.Direct2D1;

namespace WoWEditor6.UI.Components
{
    class Checkbox : IComponent
    {
        private static SolidBrush gBorderBrush;
        private static SolidBrush gBackgroundBrush;
        private static SolidBrush gBackgroundHoverBrush;
        private static SolidBrush gBackgroundClickedBrush;
        private static SolidBrush gClickBrush;

        private RectangleF mTargetRect;
        private Vector2 mPosition;
        private float mSize;
        private bool mIsHovered;
        private bool mIsPressed;
        private readonly StaticText mTextDraw;

        public Vector2 Position { get { return mPosition; } set { ClientAreaChanged(value, mSize); } }
        public float Size { get { return mSize; } set { ClientAreaChanged(mPosition, value); } }

        public string Text { get { return mTextDraw.Text; } set { mTextDraw.Text = value; } }
        public bool Checked { get; set; }
        public event Action<Checkbox, bool> CheckChanged;

        public Checkbox()
        {
            mTextDraw = new StaticText
            {
                Weight = SharpDX.DirectWrite.FontWeight.Bold,
                VerticalAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center
            };

            Text = "";
        }

        public void OnRender(RenderTarget target)
        {
            if(gBorderBrush == null)
            {
                gBackgroundBrush = Brushes.Solid[0xCC555555];
                gBackgroundHoverBrush = Brushes.Solid[0xCC888888];
                gClickBrush = Brushes.Solid[0xFFFF7F00];
                gBackgroundClickedBrush = Brushes.Solid[0xCCBBBBBB];
                gBorderBrush = Brushes.White;
            }

            target.DrawTextLayout(new Vector2(Position.X + Size + 7, Position.Y - 2), mTextDraw, Brushes.White);

            var brush = gBackgroundBrush;
            if (mIsPressed)
                brush = gBackgroundClickedBrush;
            else if (mIsHovered)
                brush = gBackgroundHoverBrush;

            target.FillRectangle(mTargetRect, brush);
            target.DrawRectangle(mTargetRect, gBorderBrush);
            if (!Checked) return;

            target.DrawLine(Position + new Vector2(3, 3), Position + new Vector2(mSize - 3, mSize - 3), gClickBrush,
                mSize / 4.0f);
            target.DrawLine(new Vector2(Position.X + 3, Position.Y + mSize - 3),
                new Vector2(Position.X + mSize - 3, Position.Y + 3), gClickBrush, mSize / 4.0f);
        }

        public void OnMessage(Message message)
        {
            var msg = message as MouseMessage;
            if (msg == null)
                return;

            if (msg.IsHandled)
                return;
            
            switch(message.Type)
            {
                case MessageType.MouseMove:
                    mIsHovered = IsHovered(msg.Position);
                    break;

                case MessageType.MouseDown:
                    if (msg.Buttons == MouseButton.Left)
                        mIsPressed = IsHovered(msg.Position);
                    break;

                case MessageType.MouseUp:
                    {
                        if (msg.Buttons == MouseButton.Left && IsHovered(msg.Position) && mIsPressed)
                        {
                            Checked = !Checked;
                            if (CheckChanged != null)
                                CheckChanged(this, Checked);
                        }

                        mIsPressed = false;
                    }
                    break;
            }
        }

        private void ClientAreaChanged(Vector2 position, float size)
        {
            size = Math.Max(size, 16.0f);
            mPosition = position;
            mSize = size;

            mTargetRect = new RectangleF(position.X, position.Y, size, size);
            mTextDraw.FontSize = size;
            mTextDraw.Size = new Size2F(float.MaxValue, size);
        }

        private bool IsHovered(Vector2 position)
        {
            return mTargetRect.Contains(position) || new RectangleF(mPosition.X + mSize, mPosition.Y, mTextDraw.Width + 7, Size).Contains(position);
        }
    }
}
