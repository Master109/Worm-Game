using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace Worms
{
	public class GameCamera : CameraScript, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		
		public override void Awake ()
		{
			base.Awake ();
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Add(this);
		}
		
		public virtual void OnDestroy ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		public virtual void DoUpdate ()
		{
			HandleViewSize ();
			HandlePosition ();
		}

		public override void HandlePosition ()
		{
			base.HandlePosition ();
			trs.position = trs.position.SetY(GameManager.GetSingleton<Player>().trs.position.y + GameManager.GetSingleton<Player>().localVerticies[0].y);
		}
	}
}