using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Worms;
using Extensions;

[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class _RectTransform : MonoBehaviour
{
	public RectTransform rectTrs;
	public bool setAnchorsToRect;

	public virtual void OnEnable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			if (rectTrs != null)
				rectTrs = GetComponent<RectTransform>();
			return;
		}
#endif
	}

#if UNITY_EDITOR
	public virtual void Update ()
	{
		if (setAnchorsToRect)
		{
			rectTrs.SetAnchorsToRect ();
			setAnchorsToRect = false;
		}
	}
#endif
}
