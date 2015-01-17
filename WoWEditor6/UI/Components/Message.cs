using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.UI.Components
{
    enum MessageType
    {
        MouseUp,
        MouseDown,
        MouseMove,
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
