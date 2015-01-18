using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace WoWEditor6.UI.Components
{
    class Label : IComponent
    {
        private readonly StaticText mTextDraw;
        private Vector2 mSize;
        private bool mMultiline;

        public Vector2 Position { get; set; }
        public Vector2 Size { get { return mSize; } set { SizeChanged(value); } }
        public string Text { get { return mTextDraw.Text; } set { mTextDraw.Text = value; } }
        public SolidBrush Color { get; set; } = Brushes.Solid[0xFFFFFFFF];
        public float FontSize { get { return mTextDraw.FontSize; } set { mTextDraw.FontSize = value; } }
        public bool Multiline { get { return mMultiline; } set { MultilineChanged(value); } }
        public TextAlignment HorizontalAlignment
        {
            get { return mTextDraw.HorizontalAlignment; }
            set { mTextDraw.HorizontalAlignment = value; }
        }

        public ParagraphAlignment VerticalAlignment
        {
            get { return mTextDraw.VerticalAlignment; }
            set { mTextDraw.VerticalAlignment = value; }
        }

        public float TextHeight => ((TextLayout) mTextDraw).Metrics.Height;

        public Label()
        {
            mTextDraw = new StaticText
            {
                FontFamily = "Segoe UI",
                FontSize = 14.0f,
                HorizontalAlignment = TextAlignment.Leading,
                Size = new Size2F(float.MaxValue, 100.0f),
                Text = "",
                VerticalAlignment = ParagraphAlignment.Near,
                Weight = FontWeight.Bold
            };
        }

        public void OnRender(RenderTarget target)
        {
            target.PushAxisAlignedClip(new RectangleF(Position.X, Position.Y, Size.X, Size.Y), AntialiasMode.Aliased);

            target.DrawTextLayout(Position, mTextDraw, Color, mMultiline ? DrawTextOptions.None : DrawTextOptions.Clip);

            target.PopAxisAlignedClip();
        }

        public void OnMessage(Message message)
        {

        }

        private void SizeChanged(Vector2 size)
        {
            mTextDraw.Size = mMultiline ? new Size2F(size.X, size.Y) : new Size2F(float.MaxValue, size.Y);
            mSize = size;
        }

        private void MultilineChanged(bool value)
        {
            mTextDraw.Size = value ? new Size2F(mSize.X, mSize.Y) : new Size2F(float.MaxValue, mSize.Y);
            mMultiline = value;
        }
    }
}
