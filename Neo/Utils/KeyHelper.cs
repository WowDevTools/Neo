using Gdk;

namespace Neo.Utils
{
    static class KeyHelper
    {
        public static bool AreKeysDown(byte[] keyState, params Key[] keys)
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

        public static bool IsKeyDown(byte[] keyState, Key key)
        {
            return ((keyState[(int)key]) & 0x80) != 0;
        }
    }
}
