using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Extensions;
using Worms;

[RequireComponent(typeof(Slider))]
public class _Slider : _Selectable, IUpdatable
{
	public bool PauseWhileUnfocused
	{
		get
		{
			return true;
		}
	}
	public Slider slider;
	public Text displayValue;
	string initDisplayValue;
	public float[] snapValues;
	[HideInInspector]
	public int indexOfCurrentSnapValue;
	public RectTransform slidingArea;
	
	public virtual void Awake ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
		if (displayValue != null)
			initDisplayValue = displayValue.text;
		SetDisplayValue ();
		if (snapValues.Length > 0)
			indexOfCurrentSnapValue = MathfExtensions.GetIndexOfClosestNumber(slider.value, snapValues);
	}

	public override void OnEnable ()
	{
		if (slidingArea == null)
		{
			slidingArea = GetComponent<RectTransform>();
			slidingArea = slidingArea.Find("Handle Slide Area") as RectTransform;
		}
		if (rectTrs == null)
		{
			rectTrs = GetComponent<RectTransform>();
			rectTrs = rectTrs.Find("Handle") as RectTransform;
		}
		base.OnEnable ();
		GameManager.updatables = GameManager.updatables.Add(this);
	}

	public virtual void OnDisable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
		GameManager.updatables = GameManager.updatables.Remove(this);
	}

	public virtual void DoUpdate ()
	{
		if (snapValues.Length > 0)
			slider.value = MathfExtensions.GetClosestNumber(slider.value, snapValues);
	}

	public virtual void SetDisplayValue ()
	{
		displayValue.text = initDisplayValue + slider.value;
	}
}