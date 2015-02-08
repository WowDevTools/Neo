using System;
using SharpDX;
using SharpDX.Direct2D1;

namespace WoWEditor6.UI.Components
{
    class Scrollbar : IComponent
    {
        private Vector2 mPosition;
        private float mSize;
        private float mScrollOffset;
        private bool mIsKnobDown;
        private bool mIsKnobHovered;
        private Vector2 mKnobOffset;

        public float TotalSize { get; set; }
        public float VisibleSize { get; set; }

        public float Thickness { get; set; }
        public bool Vertical { get; set; }

        public Vector2 Position { get { return mPosition; } set { mPosition = value; } }
        public float Size { get { return mSize; } set { mSize = value; } }

        public event Action<float> ScrollChanged;

        public Scrollbar()
        {
            Thickness = 10.0f;
            Vertical = true;
        }

        public void OnRender(RenderTarget target)
        {
            var color = Brushes.Solid[0xFFAAAAAA];
            if (mIsKnobDown)
                color = Brushes.White;
            else if (mIsKnobHovered)
                color = Brushes.Solid[0xFFDDDDDD];

            var fact = VisibleSize / TotalSize;
            var scrollStart = (mScrollOffset / TotalSize) * Size;

            target.FillRectangle(
                new RectangleF(Position.X + (Vertical ? 0 : scrollStart), Position.Y  + (Vertical ? scrollStart : 0), Vertical ? Thickness : (Size * fact),
                    Vertical ? (Size * fact) : Thickness), color);
        }

        public void OnScroll(int delta)
        {
            mScrollOffset += delta;
            if (mScrollOffset < 0)
                mScrollOffset = 0;
            else if (mScrollOffset + VisibleSize > TotalSize)
                mScrollOffset = TotalSize - VisibleSize;

            if (ScrollChanged != null)
                ScrollChanged(mScrollOffset);
        }

        public void OnMessage(Message message)
        {
            var mouseMsg = message as MouseMessage;
            if (mouseMsg == null)
                return;

            if (mouseMsg.IsHandled)
                return;

            switch(mouseMsg.Type)
            {
                case MessageType.MouseDown:
                    HandleMouseDown(mouseMsg);
                    break;

                case MessageType.MouseUp:
                    mIsKnobDown = false;
                    break;

                case MessageType.MouseMove:
                    HandleMouseMove(mouseMsg);
                    break;
            }
        }

        private void HandleMouseMove(MouseMessage msg)
        {
            var fact = VisibleSize / TotalSize;
            var scrollStart = (mScrollOffset / TotalSize) * Size;
            var knobRect = new RectangleF(Position.X + (Vertical ? 0 : scrollStart),
                Position.Y + (Vertical ? scrollStart : 0), Vertical ? Thickness : (Size * fact),
                Vertical ? (Size * fact) : Thickness);

            mIsKnobHovered = knobRect.Contains(msg.Position);

            if (mIsKnobDown == false)
                return;

            var knoby = msg.Position.Y - mKnobOffset.Y - Position.Y;
            if (knoby < 0)
                knoby = 0;

            scrollStart = knoby;
            scrollStart /= Size;
            scrollStart *= TotalSize;
            mScrollOffset = scrollStart;
            if (mScrollOffset + VisibleSize > TotalSize)
                mScrollOffset = TotalSize - VisibleSize;

            if (ScrollChanged != null)
                ScrollChanged(mScrollOffset);
        }

        private void HandleMouseDown(MouseMessage msg)
        {
            var fact = VisibleSize / TotalSize;
            var scrollStart = (mScrollOffset / TotalSize) * Size;
            var knobRect = new RectangleF(Position.X + (Vertical ? 0 : scrollStart),
                Position.Y + (Vertical ? scrollStart : 0), Vertical ? Thickness : (Size * fact),
                Vertical ? (Size * fact) : Thickness);

            mIsKnobDown = knobRect.Contains(msg.Position);
            mKnobOffset = new Vector2(msg.Position.X - knobRect.X, msg.Position.Y - knobRect.Y);
        }
    }
}
