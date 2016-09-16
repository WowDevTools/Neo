using System.Drawing;

namespace Neo.Utils
{
	public static class InterfaceHelper
	{
		public static Point GetCursorPosition()
		{
			Point curPos = new Point();
			int x;
			int y;
			Gdk.Display.Default.GetPointer(out x, out y);

			curPos.X = x;
			curPos.Y = y;

			return curPos;
		}
	}
}