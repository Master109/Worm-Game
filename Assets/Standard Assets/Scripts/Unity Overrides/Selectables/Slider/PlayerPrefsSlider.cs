using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace Worms
{
	public class PlayerPrefsSlider : _Slider
	{
		public string playerPrefsKey;
		
		public override void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			slider.value = SaveAndLoadManager.GetValue<float>(playerPrefsKey, slider.value);
			base.Awake ();
		}
		
		public override void DoUpdate ()
		{
			base.DoUpdate ();
			SaveAndLoadManager.SetValue(playerPrefsKey, slider.value);
		}
	}
}
