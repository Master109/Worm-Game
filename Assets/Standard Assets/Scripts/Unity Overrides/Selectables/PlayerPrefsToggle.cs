using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.UI;
using Worms;

namespace Worms
{
	[RequireComponent(typeof(Toggle))]
	public class PlayerPrefsToggle : _Selectable, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public Toggle toggle;
		public string playerPrefsKey;
		
		public virtual void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			toggle.isOn = SaveAndLoadManager.GetValue<bool>(playerPrefsKey, toggle.isOn);
		}
		
		public override void OnEnable ()
		{
			base.OnEnable ();
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
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
			SaveAndLoadManager.SetValue(playerPrefsKey, toggle.isOn);
		}
	}
}
