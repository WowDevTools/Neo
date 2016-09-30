using System;
using System.Drawing;

namespace Neo.Utils
{
	public static class RandomExtensions
	{
		public static Color NextColor(this Random random)
		{
			byte red = (byte)random.Next(0, 255);
			byte green = (byte)random.Next(0, 255);
			byte blue = (byte)random.Next(0, 255);

			return Color.FromArgb(red, green, blue);
		}
	}
}