using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct2D1;

namespace WoWEditor6.UI.Components
{
    class StatusBar : IComponent
    {
        private Vector2 mPosition;
        private Vector2 mSize;
        private RectangleF mTargetRectangle;

        public List<IComponent> Items { get; } = new List<IComponent>();

        public Vector2 Position { get { return mPosition; } set { ClientAreaChanged(value, mSize); } }
        public Vector2 Size { get { return mSize; } set { ClientAreaChanged(mPosition, value); } }
        public Vector2 BorderOffsets { private get; set; }

        public void OnRender(RenderTarget target)
        {
            var borderStart = new Vector2(mPosition.X + BorderOffsets.X, mPosition.Y);
            var borderEnd = new Vector2(mPosition.X + mSize.X - BorderOffsets.Y, mPosition.Y);

            target.FillRectangle(mTargetRectangle, Brushes.Solid[0xCC333333]);
            target.DrawLine(borderStart, borderEnd, Brushes.White);

            var ot = target.Transform;

            target.Transform *= Matrix3x2.Translation(Position.X, Position.Y);
            foreach (var item in Items)
                item.OnRender(target);

            target.Transform = ot;
        }

        public void OnMessage(Message message)
        {
            foreach (var item in Items)
                item.OnMessage(message);
        }

        private void ClientAreaChanged(Vector2 position, Vector2 size)
        {
            mPosition = position;
            mSize = size;

            mTargetRectangle = new RectangleF(position.X, position.Y, size.X, size.Y);
        }
    }
}
