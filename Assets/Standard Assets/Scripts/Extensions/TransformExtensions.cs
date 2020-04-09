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

		public static Transform FindClosestTransform (Transform[] transforms, Transform closestTo)
		{
			int closestOpponentIndex = 0;
			Transform closestTransform = transforms[closestOpponentIndex];
			Transform checkTransform;
			float checkDistance;
			float closestDistance = Vector2.Distance(closestTo.position, closestTransform.position);
			for (int i = 1; i < transforms.Length; i ++)
			{
				checkTransform = transforms[i];
				checkDistance = Vector2.Distance(closestTo.position, checkTransform.position);
				if (checkDistance < closestDistance)
				{
					closestOpponentIndex = i;
					closestTransform = checkTransform;
					closestDistance = checkDistance;
				}
			}
			return closestTransform;
		}
	}
}