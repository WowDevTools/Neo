using System;
using System.Collections;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct2D1;

namespace Neo.UI.Components
{
    enum ToolbarOrientation
    {
        Horizontal,
        Vertical
    }

    class Toolbar : IComponent
    {
        public interface IToolbarButtonCollection : IEnumerable<ImageButton>
        {
            void Add(ImageButton button);
            void Remove(ImageButton button);
            void AddRange(IEnumerable<ImageButton> buttons);
        }

        class ToolbarButtonCollection : IToolbarButtonCollection
        {
            private readonly List<ImageButton> mButtons = new List<ImageButton>();

            public IEnumerator<ImageButton> GetEnumerator()
            {
                // ReSharper disable once InconsistentlySynchronizedField
                return mButtons.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(ImageButton button)
            {
                lock(this)
                {
                    mButtons.Add(button);
                    if (OnItemsChanged != null)
                        OnItemsChanged();
                }
            }

            public void Remove(ImageButton button)
            {
                lock(this)
                {
                    if (mButtons.Remove(button))
                    {
                        if (OnItemsChanged != null)
                            OnItemsChanged();
                    }
                }
            }

            public void AddRange(IEnumerable<ImageButton> buttons)
            {
                lock(this)
                {
                    mButtons.AddRange(buttons);
                    if (OnItemsChanged != null)
                            OnItemsChanged();
                }
            }

            public event Action OnItemsChanged;
        }

        private Vector2 mPosition;
        private Vector2 mSize;
        private RectangleF mTargetRectangle;
        private ToolbarOrientation mOrientation;

        public IToolbarButtonCollection Buttons { get; private set; }

        public Vector2 Position { get { return mPosition; } set { ClientAreaChanged(value, mSize); } }
        public Vector2 Size { get { return mSize; } set { ClientAreaChanged(mPosition, value); } }
        public Vector2 BorderOffsets { private get; set; }

        public ToolbarOrientation Orientation { get { return mOrientation; } set { OrientationChanged(value); } }

        public Toolbar()
        {
            Buttons = new ToolbarButtonCollection();
            var tbc = Buttons as ToolbarButtonCollection;
            // ReSharper disable once PossibleNullReferenceException
            tbc.OnItemsChanged += ButtonsChanged;
            Orientation = ToolbarOrientation.Horizontal;
        }

        public void OnRender(RenderTarget target)
        {
            var borderStart = mOrientation == ToolbarOrientation.Horizontal
                ? new Vector2(mPosition.X + BorderOffsets.X, mPosition.Y + mSize.Y)
                : new Vector2(mPosition.X + mSize.X, mPosition.Y + BorderOffsets.X);

            var borderEnd = mOrientation == ToolbarOrientation.Horizontal
                ? new Vector2(mPosition.X + mSize.X - BorderOffsets.Y, mPosition.Y + mSize.Y)
                : new Vector2(mPosition.X + mSize.X, mPosition.Y + mSize.Y - BorderOffsets.Y);

            target.FillRectangle(mTargetRectangle, Brushes.Solid[0xCC333333]);
            target.DrawLine(borderStart, borderEnd, Brushes.White);

            lock(Buttons)
            {
                foreach (var button in Buttons)
                    button.OnRender(target);
            }
        }

        public void OnMessage(Message message)
        {
            lock(Buttons)
            {
                foreach (var button in Buttons)
                    button.OnMessage(message);
            }
        }

        private void ButtonsChanged()
        {
            var size = mOrientation == ToolbarOrientation.Horizontal ? (mSize.Y - 10) : (mSize.X - 10);

            var curPos = 10.0f;

            foreach(var button in Buttons)
            {
                button.Size = new Vector2(size, size);
                button.Position = mOrientation == ToolbarOrientation.Horizontal
                    ? new Vector2(mPosition.X + curPos, mPosition.Y + 5)
                    : new Vector2(mSize.X + 5, curPos);

                curPos += size + 10;
            }
        }

        private void ClientAreaChanged(Vector2 position, Vector2 size)
        {
            mPosition = position;
            mSize = size;

            mTargetRectangle = new RectangleF(position.X, position.Y, size.X, size.Y);
            lock (Buttons)
                ButtonsChanged();
        }

        private void OrientationChanged(ToolbarOrientation orientation)
        {
            mOrientation = orientation;

            lock (Buttons)
                ButtonsChanged();
        }
    }
}
