using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace Worms
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[DisallowMultipleComponent]
	public class CameraScript : SingletonMonoBehaviour<CameraScript>
	{
		public Transform trs;
		public Camera camera;
		public Vector2 viewSize;
		protected Rect normalizedScreenViewRect;
		protected float screenAspect;
		[HideInInspector]
		public Rect viewRect;
		
		public override void Awake ()
		{
			base.Awake ();
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (trs == null)
					trs = GetComponent<Transform>();
				if (camera == null)
					camera = GetComponent<Camera>();
				return;
			}
#endif
			trs.SetParent(null);
			trs.localScale = Vector3.one;
			viewRect.size = viewSize;
			// HandlePosition ();
			// HandleViewSize ();
			if (camera == Camera.main)
			{
				GameManager.singletons.Remove(typeof(CameraScript));
				GameManager.singletons.Add(typeof(CameraScript), this);
			}
		}
		
		public virtual void HandlePosition ()
		{
			viewRect.center = trs.position;
		}
		
		public virtual void HandleViewSize ()
		{
			screenAspect = GameManager.instance.gameViewRectTrs.rect.size.x / GameManager.instance.gameViewRectTrs.rect.size.y;
			camera.aspect = viewSize.x / viewSize.y;
			camera.orthographicSize = Mathf.Min(viewSize.x / 2 / camera.aspect, viewSize.y / 2);
			normalizedScreenViewRect = new Rect();
			normalizedScreenViewRect.size = new Vector2(camera.aspect / screenAspect, Mathf.Min(1, screenAspect / camera.aspect));
			normalizedScreenViewRect.center = Vector2.one / 2;
			camera.rect = normalizedScreenViewRect;
		}
	}
}