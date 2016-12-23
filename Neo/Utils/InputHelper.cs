using OpenTK.Input;

namespace Neo.Utils
{
	internal static class InputHelper
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

	    public static bool IsKeyDown(Key key)
	    {
		    KeyboardState keyboardState = Keyboard.GetState();
		    return keyboardState.IsKeyDown(key);
	    }

	    public static bool IsButtonDown(MouseButton button)
	    {
		    MouseState mouseState = Mouse.GetState();
		    return mouseState.IsButtonDown(button);
	    }
    }
}
