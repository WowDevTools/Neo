using System;
using SharpDX;

namespace WoWEditor6.UI.Components
{
    [Flags]
    enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    class MouseMessage : Message
    {
        public Vector2 Position { get; private set; }
        public MouseButton Buttons { get; private set; }
        public int Delta { get; set; }

        public MouseMessage(MessageType type, Vector2 position, MouseButton buttons)
            : base(type)
        {
            Buttons = buttons;
            Position = position;
        }

        public override Message Forward(Vector2 parentPosition)
        {
            return new MouseMessage(Type, new Vector2(Position.X - parentPosition.X, Position.Y - parentPosition.Y), Buttons);
        }
    }
}
