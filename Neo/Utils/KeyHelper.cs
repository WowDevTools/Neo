using OpenTK.Input;

namespace Neo.Utils
{
    static class KeyHelper
    {
        public static bool AreKeysDown(params Key[] keys)
        {
	        KeyboardState keyboardState = Keyboard.GetState();
	        if (keys.Length == 0)
	        {
		        return false;
	        }

            foreach (var key in keys)
            {
	            if (keyboardState.IsKeyDown(key))
	            {
		            return false;
	            }
            }

            return true;
        }
    }
}
