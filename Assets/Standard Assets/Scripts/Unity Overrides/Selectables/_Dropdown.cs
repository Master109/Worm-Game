using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Extensions;
using Worms;

[DisallowMultipleComponent]
public class _Dropdown : _Selectable, IUpdatable
{
	public bool PauseWhileUnfocused
	{
		get
		{
			return true;
		}
	}
	public bool raiseOnChoseOption;
	public bool raiseOnClickOff;
	public RectTransform optionsRectTrs;

	public virtual void DoUpdate ()
	{
		if (Input.GetMouseButtonUp(0))
		{
			if (optionsRectTrs.GetWorldRect().Contains(Input.mousePosition))
			{
				if (raiseOnChoseOption)
					StartCoroutine(DelayRaiseRoutine ());
			}
			else if (raiseOnClickOff && !rectTrs.GetWorldRect().Contains(Input.mousePosition))
				Raise ();
		}
	}

	public virtual void Drop ()
	{
		GameManager.updatables = GameManager.updatables.Add(this);
		optionsRectTrs.gameObject.SetActive(true);
	}

	public virtual void Raise ()
	{
		GameManager.updatables = GameManager.updatables.Remove(this);
		optionsRectTrs.gameObject.SetActive(false);
	}

	public virtual void ToggleDrop ()
	{
		if (optionsRectTrs.gameObject.activeSelf)
			Raise ();
		else
			Drop ();
	}

	public virtual IEnumerator DelayRaiseRoutine ()
	{
		yield return null;
		Raise ();
	}
}