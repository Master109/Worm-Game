#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Events;
using Extensions;

[ExecuteAlways]
public class EditorScript : MonoBehaviour
{
	public static InputEvent inputEvent = new InputEvent();
	public Hotkey[] hotkeys = new Hotkey[0];

	public virtual void OnEnable ()
	{
		EditorApplication.update += Update;
	}

	public virtual void OnDisable ()
	{
		EditorApplication.update -= Update;
	}

	public virtual void Update ()
	{
		UpdateHotkeys ();
	}

	public virtual void UpdateHotkeys ()
	{
		Hotkey hotkey;
		bool shouldBreak;
		for (int i = 0; i < hotkeys.Length; i ++)
		{
			hotkey = hotkeys[i];
			if (hotkey.previousKeys.Equals((KeyCode[]) hotkey.keys.Clone()))
				hotkey.isPressingKeys = new bool[hotkey.keys.Length];
			if (Event.current != null)
			{
				shouldBreak = false;
				inputEvent.mousePosition = Event.current.mousePosition.ToVec2Int();
				inputEvent.type = Event.current.type;
				for (int i2 = 0; i2 < hotkey.keys.Length; i2 ++)
				{
					if (Event.current.keyCode == hotkey.keys[i2])
					{
						if (Event.current.type == EventType.KeyDown)
						{
							hotkey.isPressingKeys[i2] = true;
							if (hotkey.downType == Hotkey.DownType.All)
							{
								foreach (bool isPressingKey in hotkey.isPressingKeys)
								{
									if (!isPressingKey)
									{
										shouldBreak = true;
										break;
									}
								}
								if (shouldBreak)
									break;
							}
							hotkey.downAction.Invoke();
						}
						else if (Event.current.type == EventType.KeyUp)
						{
							hotkey.isPressingKeys[i2] = false;
							if (hotkey.upType == Hotkey.UpType.All)
							{
								foreach (bool isPressingKey in hotkey.isPressingKeys)
								{
									if (isPressingKey)
									{
										shouldBreak = true;
										break;
									}
								}
								if (shouldBreak)
									break;
							}
							hotkey.upAction.Invoke();
						}
					}
				}
			}
			hotkey.previousKeys = (KeyCode[]) hotkey.keys.Clone();
		}
	}

	public static Vector2 GetMousePositionInWorld ()
	{
		Vector2 output;
		Camera camera = SceneView.lastActiveSceneView.camera;
		if (camera == null)
			camera = SceneView.currentDrawingSceneView.camera;
		output = inputEvent.mousePosition;
		output.y = camera.ViewportToScreenPoint(Vector2.one).y - camera.ViewportToScreenPoint(Vector2.zero).y - output.y;
		output = camera.ScreenToWorldPoint(output);
		return output;
	}

	[Serializable]
	public class Hotkey
	{
		public string name;
		public KeyCode[] keys;
		[HideInInspector]
		public KeyCode[] previousKeys;
		// [HideInInspector]
		public bool[] isPressingKeys;
		public DownType downType;
		public UpType upType;
		public UnityEvent downAction;
		public UnityEvent upAction;

		public enum DownType
		{
			All,
			Any
		}

		public enum UpType
		{
			All
		}
	}

	public class InputEvent
	{
		public Vector2Int mousePosition;
		public EventType type;
	}
}
#endif