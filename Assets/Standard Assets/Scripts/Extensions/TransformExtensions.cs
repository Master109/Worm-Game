using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class TransformExtensions
	{
		public static Transform FindChild (this Transform trs, string childName)
		{
			List<Transform> remainingChildren = new List<Transform>();
			remainingChildren.Add(trs);
			while (remainingChildren.Count > 0)
			{
				foreach (Transform child in remainingChildren[0])
				{
					if (child.name == childName)
						return child;
					remainingChildren.Add(child);
				}
				remainingChildren.RemoveAt(0);
			}
			return null;
		}
		public static Transform[] FindChildren (this Transform trs, string childName)
		{
			List<Transform> remainingChildren = new List<Transform>();
			List<Transform> output = new List<Transform>();
			remainingChildren.Add(trs);
			while (remainingChildren.Count > 0)
			{
				foreach (Transform child in remainingChildren[0])
				{
					if (child.name == childName)
						output.Add(child);
					remainingChildren.Add(child);
				}
				remainingChildren.RemoveAt(0);
			}
			return output.ToArray();
		}

		public static Transform GetClosestTransform_2D (this Transform closestTo, Transform[] transforms)
		{
			while (transforms.Contains(null))
				transforms = transforms.Remove(null);
			if (transforms.Length == 0)
				return null;
			else if (transforms.Length == 1)
				return transforms[0];
			int closestOpponentIndex = 0;
			for (int i = 1; i < transforms.Length; i ++)
			{
				Transform checkOpponent = transforms[i];
				Transform currentClosestOpponent = transforms[closestOpponentIndex];
				if (Vector2.Distance(closestTo.position, checkOpponent.position) < Vector2.Distance(closestTo.position, currentClosestOpponent.position))
					closestOpponentIndex = i;
			}
			return transforms[closestOpponentIndex];
		}

		public static Transform GetClosestTransform_2D (Transform[] transforms, Vector2 position)
		{
			while (transforms.Contains(null))
				transforms = transforms.Remove(null);
			if (transforms.Length == 0)
				return null;
			else if (transforms.Length == 1)
				return transforms[0];
			int closestOpponentIndex = 0;
			for (int i = 1; i < transforms.Length; i ++)
			{
				Transform checkOpponent = transforms[i];
				Transform currentClosestOpponent = transforms[closestOpponentIndex];
				if (Vector2.Distance(position, checkOpponent.position) < Vector2.Distance(position, currentClosestOpponent.position))
					closestOpponentIndex = i;
			}
			return transforms[closestOpponentIndex];
		}

		public static void SetWorldScale (this Transform trs, Vector3 scale)
		{
			trs.localScale = trs.rotation * trs.InverseTransformDirection(scale).Divide(trs.parent.lossyScale);
		}
	}
}