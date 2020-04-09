using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class ColorExtensions
	{
		public static Color SetAlpha (this Color c, float a)
		{
			return new Color(c.r, c.g, c.b, a);
		}
		
		public static Color AddAlpha (this Color c, float a)
		{
			return SetAlpha (c, c.a + a);
		}
		
		public static Color MultiplyAlpha (this Color c, float a)
		{
			return SetAlpha (c, c.a * a);
		}
		
		public static Color DivideAlpha (this Color c, float a)
		{
			return SetAlpha (c, c.a / a);
		}
		
		public static byte[] ToBytes (this Color color)
		{
			byte[] bytes = new byte[4];
			bytes[0] = (byte) (255 * color.r - 128);
			return bytes;
		}

		public static Color RandomColor ()
		{
			return new Color(Random.value, Random.value, Random.value);
		}

		public static Color RandomColorBetween (Color a, Color b)
		{
			return Color.Lerp(a, b, Random.value);
		}

		public static Color RandomColorBetween (params KeyValuePair<Color, Color>[] colors)
		{
			KeyValuePair<Color, Color> keyValuePair;
			int keyValuePairIndex = Random.Range(0, colors.Length);
			keyValuePair = colors[keyValuePairIndex];
			return RandomColorBetween(keyValuePair.Key, keyValuePair.Value);
		}

		public static bool IsColorWithinRangeOfColor (Color fromColor, Color toColor, float range, bool equalValueIsWithin = true)
		{
			float difference = GetAverageDifferenceAbs(toColor, fromColor);
			if (equalValueIsWithin)
				return difference <= range;
			else
				return difference < range;
		}

		public static float GetAverageDifference (Color fromColor, Color toColor)
		{
			float r = toColor.r - fromColor.r;
			float g = toColor.g - fromColor.g;
			float b = toColor.b - fromColor.b;
			return (r + g + b) / 3;
		}

		public static float GetAverageDifferenceAbs (Color fromColor, Color toColor)
		{
			float r = Mathf.Abs(toColor.r - fromColor.r);
			float g = Mathf.Abs(toColor.g - fromColor.g);
			float b = Mathf.Abs(toColor.b - fromColor.b);
			return (r + g + b) / 3;
		}

		public static bool IsColorOutsideRangeFromColors (Color c, float range, Color[] colors, bool equalValueIsWithin = true)
		{
			foreach (Color color in colors)
			{
				if (IsColorWithinRangeOfColor(c, color, range, equalValueIsWithin))
					return false;
			}
			return true;
		}

		public static bool IsColorOutsideRangeFromColors (Color c, float range, params Color[] colors)
		{
			foreach (Color color in colors)
			{
				if (IsColorWithinRangeOfColor(c, color, range))
					return false;
			}
			return true;
		}
	}
}