﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Extensions;
// using Worms;

// [ExecuteInEditMode]
public class _Selectable : MonoBehaviour
{
	public Canvas canvas;
	public RectTransform canvasRectTrs;
	public RectTransform rectTrs;
	public Selectable selectable;
	public float priority;
	public bool Interactable
	{
		get
		{
			return selectable.interactable;
		}
		set
		{
			if (value != selectable.interactable)
			{
				if (value)
					OnEnable ();
				else
					OnDisable ();
			}
			selectable.interactable = value;
		}
	}
	
	public virtual void UpdateCanvas ()
	{
		canvas = GetComponent<Canvas>();
		canvasRectTrs = GetComponent<RectTransform>();
		while (canvas == null)
		{
			canvasRectTrs = canvasRectTrs.parent.GetComponent<RectTransform>();
			canvas = canvasRectTrs.GetComponent<Canvas>();
		}
	}
	
	public virtual void OnEnable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			if (rectTrs == null)
				rectTrs = GetComponent<RectTransform>();
			if (selectable == null)
				selectable = GetComponent<Selectable>();
			return;
		}
#endif
		// GameManager.GetSingleton<UIControlManager>().AddSelectable (this);
		UpdateCanvas ();
	}
	
	public virtual void OnDisable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
		// GameManager.GetSingleton<UIControlManager>().RemoveSelectable (this);
	}
}
