using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WoWEditor6.UI.Components
{
    class KeyboardMessage : Message
    {
        public char Character { get; set; }
        public Keys KeyCode { get; set; }

        public KeyboardMessage(MessageType type, char character, Keys key)
            : base(type)
        {
            Character = character;
            KeyCode = key;
        }

        public static char GetCharacter(KeyEventArgs args)
        {
            var state = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(state);
            var buff = new StringBuilder(1);
            var numRet = UnsafeNativeMethods.ToUnicode(args.KeyValue, (int) args.KeyData, state, buff, 1, 0);
            return numRet <= 0 ? '\0' : buff[0];
        }
    }
}
