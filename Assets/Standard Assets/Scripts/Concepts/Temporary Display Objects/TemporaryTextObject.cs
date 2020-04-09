using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;

namespace Worms
{
	[Serializable]
	public class TemporaryTextObject : TemporaryDisplayObject
	{
		public TMP_Text text;
		public float durationPerCharacter;
		
		public override IEnumerator DisplayRoutine ()
		{
			duration = text.text.Length * durationPerCharacter;
			yield return base.DisplayRoutine ();
		}
	}
}