using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class RectExtensions
	{
		public static Rect NULL = new Rect(MathfExtensions.NULL_FLOAT, MathfExtensions.NULL_FLOAT, MathfExtensions.NULL_FLOAT, MathfExtensions.NULL_FLOAT);

		public static Rect Move (this Rect rect, Vector2 movement)
		{
			rect.position += movement;
			return rect;
		}

		public static Rect SwapXAndY (this Rect rect)
		{
			return Rect.MinMaxRect(rect.min.y, rect.min.x, rect.max.y, rect.max.x);
		}
		
		public static bool IsEncapsulating (this Rect r1, Rect r2, bool equalRectsRetunsTrue)
		{
			if (equalRectsRetunsTrue)
			{
				bool minIsOk = r1.min.x <= r2.min.x && r1.min.y <= r2.min.y;
				bool maxIsOk = r1.max.x >= r2.max.x && r1.max.y >= r2.max.y;
				return minIsOk && maxIsOk;
			}
			else
			{
				bool minIsOk = r1.min.x < r2.min.x && r1.min.y < r2.min.y;
				bool maxIsOk = r1.max.x > r2.max.x && r1.max.y > r2.max.y;
				return minIsOk && maxIsOk;
			}
		}
		
		// public static bool IsIntersecting (this Rect r1, Rect r2)
		// {
		// 	// return r1.Contains(r2.min) || r1.Contains(r2.max) || r1.Contains(new Vector2(r2.min.x, r2.max.y)) || r1.Contains(new Vector2(r2.max.x, r2.min.y)) || r2.Contains(r1.min) || r2.Contains(r1.max) || r2.Contains(new Vector2(r1.min.x, r1.max.y)) || r2.Contains(new Vector2(r1.max.x, r1.min.y));
		// 	return r1.Overlaps(r2);
		// }
		
		public static bool IsExtendingOutside (this Rect r1, Rect r2, bool equalRectsRetunsTrue)
		{
			if (equalRectsRetunsTrue)
			{
				bool minIsOk = r1.min.x <= r2.min.x || r1.min.y <= r2.min.y;
				bool maxIsOk = r1.max.x >= r2.max.x || r1.max.y >= r2.max.y;
				return minIsOk || maxIsOk;
			}
			else
			{
				bool minIsOk = r1.min.x < r2.min.x || r1.min.y < r2.min.y;
				bool maxIsOk = r1.max.x > r2.max.x || r1.max.y > r2.max.y;
				return minIsOk || maxIsOk;
			}
		}
		
		public static Rect ToRect (this Bounds bounds)
		{
			return Rect.MinMaxRect(bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y);
		}

		public static Rect Combine (params Rect[] rectsArray)
		{
			Rect output = rectsArray[0];
			for (int i = 1; i < rectsArray.Length; i ++)
			{
				if (rectsArray[i].min.x < output.min.x)
					output.min = new Vector2(rectsArray[i].min.x, output.min.y);
				if (rectsArray[i].min.y < output.min.y)
					output.min = new Vector2(output.min.x, rectsArray[i].min.y);
				if (rectsArray[i].max.x > output.max.x)
					output.max = new Vector2(rectsArray[i].max.x, output.max.y);
				if (rectsArray[i].max.y > output.max.y)
					output.max = new Vector2(output.max.x, rectsArray[i].max.y);
			}
			return output;
		}

		public static Rect Expand (this Rect rect, Vector2 amount)
		{
			Vector2 center = rect.center;
			rect.size += amount;
			rect.center = center;
			return rect;
		}
		
		public static Rect Set (this Rect rect, RectInt rectInt)
		{
			rect.center = rectInt.center;
			rect.size = rectInt.size;
			return rect;
		}

		public static Vector2 ClosestPoint (this Rect rect, Vector2 point)
		{
			return point.ClampComponents(rect.min, rect.max);
		}

		public static Vector2 ToNormalizedPosition (this Rect rect, Vector2 point)
		{
			return Rect.PointToNormalized(rect, point);
		}

		public static Vector2 FromNormalizedPosition (this Rect rect, Vector2 normalizedPoint)
		{
			return Rect.NormalizedToPoint(rect, normalizedPoint);
		}

		public static Rect NormalizeSizeBy (this Rect rect, Vector2 normalize)
		{
			Rect output = rect;
			output = output.Expand(rect.size.Multiply(normalize));
			output.center = rect.center;
			return output;
		}

		public static Vector2 ToNormalizedPosition (this RectInt rect, Vector2Int point)
		{
			return Vector2.one.Divide(rect.size.ToVec2()).Multiply(point.ToVec2() - rect.min.ToVec2());
		}

		public static Rect SetToPositiveSize (this Rect rect)
		{
			Rect output = rect;
			output.size = new Vector2(Mathf.Abs(output.size.x), Mathf.Abs(output.size.y));
			output.center = rect.center;
			return output;
		}

		public static Rect FromPoints (params Vector2[] points)
		{
			Vector2 point = points[0];
			Rect output = Rect.MinMaxRect(point.x, point.y, point.x, point.y);
			for (int i = 1; i < points.Length; i ++)
			{
				point = points[i];
				if (point.x < output.min.x)
					output.min = new Vector2(point.x, output.min.y);
				if (point.y < output.min.y)
					output.min = new Vector2(output.min.x, point.y);
				if (point.x > output.max.x)
					output.max = new Vector2(point.x, output.max.y);
				if (point.y > output.max.y)
					output.max = new Vector2(output.max.x, point.y);
			}
			return output;
		}
	}
}