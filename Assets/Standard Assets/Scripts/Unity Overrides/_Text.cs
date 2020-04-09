using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Worms
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(TMP_Text))]
	[DisallowMultipleComponent]
	public class _Text : Spawnable
	{
		public TMP_Text text;
		public string initTextStr;

// #if UNITY_EDITOR
		public virtual void Awake ()
		{
			// if (Application.isPlaying)
			// 	return;
			// if (text == null)
			// 	text = GetComponent<TMP_Text>();
			initTextStr = text.text;
		}
// #endif
	}
}
