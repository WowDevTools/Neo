using SharpDX;

namespace WoWEditor6.UI.Components
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
        public MessageType Type { get; }
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
