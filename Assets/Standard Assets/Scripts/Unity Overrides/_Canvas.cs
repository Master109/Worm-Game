﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Worms;

[RequireComponent(typeof(Canvas))]
[ExecuteInEditMode]
public class _Canvas : MonoBehaviour
{
	public Canvas canvas;

	public virtual void OnEnable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			if (canvas == null)
				canvas = GetComponent<Canvas>();
			return;
		}
#endif
		if (canvas.worldCamera == null)
			canvas.worldCamera = CameraScript.instance.camera;
	}
}
