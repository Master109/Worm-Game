using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace Worms
{
	public class GameCamera : CameraScript, IUpdatable
	{
		public new static GameCamera instance;
		public new static GameCamera Instance
		{
			get
			{
				if (instance == null)
					instance = FindObjectOfType<GameCamera>();
				return instance;
			}
		}
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		
		public override void Awake ()
		{
			Player.instance = Player.Instance;
			base.Awake ();
			instance = this;
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
			trs.position = trs.position.SetY(Player.instance.trs.position.y + Player.instance.localVerticies[0].y);
		}
	}
}