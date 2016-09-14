using System.Windows.Forms;

namespace WoWEditor6.Utils
{
    static class KeyHelper
    {
        public static bool AreKeysDown(byte[] keyState, params Keys[] keys)
        {
            if (keys.Length == 0)
                return false;

            foreach (var key in keys)
            {
                if ((int)key >= keyState.Length)
                    return false;

                if (((keyState[(int)key]) & 0x80) == 0)
                    return false;
            }

            return true;
        }

        public static bool IsKeyDown(byte[] keyState, Keys key)
        {
            return ((keyState[(int)key]) & 0x80) != 0;
        }
    }
}
