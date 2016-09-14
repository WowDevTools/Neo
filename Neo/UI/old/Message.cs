using SharpDX;

namespace Neo.UI.Components
{
    enum MessageType
    {
        MouseUp,
        MouseDown,
        MouseMove,
        MouseWheel,
        KeyDown,
        KeyUp
    }

    class Message
    {
        public MessageType Type { get; private set; }
        public bool IsHandled { get; set; }

        protected Message(MessageType type)
        {
            Type = type;
        }

        public virtual Message Forward(Vector2 parentPosition)
        {
            return this;
        }
    }
}
