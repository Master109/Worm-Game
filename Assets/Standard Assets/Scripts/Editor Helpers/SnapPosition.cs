﻿#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class SnapPosition : EditorScript
{
	public Transform trs;
	public bool useLocalPosition;
	public Vector3 snap = new Vector3(1, 1, 0);
	public Vector3 offset = new Vector3(0, 0, 0);

	public virtual void Start ()
	{
		if (!Application.isPlaying)
		{
			if (trs == null)
				trs = GetComponent<Transform>();
			return;
		}
		Destroy(this);
	}

	public override void Update ()
	{
		base.Update ();
		if (useLocalPosition)
			trs.localPosition = trs.localPosition.Snap(snap) + offset;
		else
			trs.position = trs.position.Snap(snap) + offset;
	}
}
#endif