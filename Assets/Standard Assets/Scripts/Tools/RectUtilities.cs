using UnityEngine;
using System;

public static class RectUtilities
{
	[Serializable]
	public class RectOffset
	{
		public float left;
		public float down;
		public float right;
		public float up;

		public RectOffset (float left, float down, float right, float up)
		{
			this.left = left;
			this.down = down;
			this.right = right;
			this.up = up;
		}

		public virtual Rect Apply (Rect rect)
		{
			return Rect.MinMaxRect(rect.xMin + left, rect.yMin + down, rect.xMax + right, rect.yMax + up);
		}

		public virtual Vector2 GetPositionOffset ()
		{
			return new Vector2(right - left, down - up);
		}
	}
}