using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using Extensions;
using Object = UnityEngine.Object;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;
using System.IO;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using System.Collections;
using CoroutineWithData = ThreadingUtilities.CoroutineWithData;
using WaitForReturnedValueOfType = ThreadingUtilities.WaitForReturnedValueOfType;
using UnityEngine.Networking;

namespace Worms
{
	// This is the main script that "glues" TWP all together. It is the only MonoBehaviour used that contains an Update method. Having multiple Update methods slows down Unity programs.
	[ExecuteInEditMode]
	public class GameManager : SingletonMonoBehaviour<GameManager>, ISavableAndLoadable
	{
		public _Text textPrefab;
		public static bool paused;
		// public GameObject[] registeredGos = new GameObject[0];
		// [SaveAndLoadValue]
		// public static string enabledGosString = "";
		// [SaveAndLoadValue]
		// public static string disabledGosString = "";
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}
		public int uniqueId;
		public int UniqueId
		{
			get
			{
				return uniqueId;
			}
			set
			{
				uniqueId = value;
			}
		}
		public const string STRING_SEPERATOR = "\n";
		public static IUpdatable[] updatables = new IUpdatable[0];
		public static IUpdatable[] pausedUpdatables = new IUpdatable[0];
		public static Dictionary<Type, object> singletons = new Dictionary<Type, object>();
		public const string UNIQUE_ID_SEPERATOR = ",";
#if UNITY_EDITOR
		public static int[] UniqueIds
		{
			get
			{
				int[] output = new int[0];
				string[] uniqueIdsString = EditorPrefs.GetString("Unique ids").Split(new string[] { UNIQUE_ID_SEPERATOR }, StringSplitOptions.None);
				int uniqueIdParsed;
				foreach (string uniqueIdString in uniqueIdsString)
				{
					if (int.TryParse(uniqueIdString, out uniqueIdParsed))
						output = output.Add(uniqueIdParsed);
				}
				return output;
			}
			set
			{
				string uniqueIdString = "";
				foreach (int uniqueId in value)
					uniqueIdString += uniqueId + UNIQUE_ID_SEPERATOR;
				EditorPrefs.SetString("Unique ids", uniqueIdString);
			}
		}
#endif
		public static int framesSinceLoadedScene;
		public const int LAG_FRAMES_AFTER_LOAD_SCENE = 2;
		public static float UnscaledDeltaTime
		{
			get
			{
				if (framesSinceLoadedScene <= LAG_FRAMES_AFTER_LOAD_SCENE)
					return 0;
				else
					return Time.unscaledDeltaTime;
			}
		}
		public CursorEntry[] cursorEntries;
		public static Dictionary<string, CursorEntry> cursorEntriesDict = new Dictionary<string, CursorEntry>();
		public static CursorEntry activeCursorEntry;
		public RectTransform cursorCanvasRectTrs;
		public static float EPSILON = Mathf.Epsilon;
		public GameModifier[] gameModifiers;
		public static Dictionary<string, GameModifier> gameModifiersDict = new Dictionary<string, GameModifier>();
		public TMP_Dropdown[] dropdowns;
		public TemporaryTextObject notificationObj;
		[SaveAndLoadValue]
		public static float timeScale;
		// public RectTransform[] nonClickThroughRectTransforms = new RectTransform[0];
		public RectTransform gameViewRectTrs;

		public override void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				Transform[] transforms = FindObjectsOfType<Transform>();
				IIdentifiable[] identifiables = new IIdentifiable[0];
				foreach (Transform trs in transforms)
				{
					identifiables = trs.GetComponents<IIdentifiable>();
					foreach (IIdentifiable identifiable in identifiables)
					{
						if (!UniqueIds.Contains(identifiable.UniqueId))
							UniqueIds = UniqueIds.Add(identifiable.UniqueId);
					}
				}
				return;
			}
#endif
			if (BuildManager.IsFirstStartup)
			{
				if (GetSingleton<BuildManager>().clearDataOnFirstStartup)
					PlayerPrefs.DeleteAll();
				BuildManager.IsFirstStartup = false;
			}
			if (SceneManager.GetActiveScene().name == "Game")
				StartGame ();
			base.Awake ();
		}

		public virtual void StartGame ()
		{
			if (gameModifiersDict.Count == 0)
			{
				foreach (GameModifier gameModifier in gameModifiers)
					gameModifiersDict.Add(gameModifier.name, gameModifier);
			}
			if (GetSingleton<GameManager>() != this)
				return;
			UpdateDropdowns ();
			if (cursorEntries.Length > 0)
			{
				activeCursorEntry = null;
				cursorEntriesDict.Clear();
				foreach (CursorEntry cursorEntry in cursorEntries)
				{
					cursorEntriesDict.Add(cursorEntry.name, cursorEntry);
					cursorEntry.rectTrs.gameObject.SetActive(false);
				}
				Cursor.visible = false;
				cursorEntriesDict["Default"].SetAsActive ();
			}
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		public virtual void DisplayNotification (string text)
		{
			notificationObj.text.text = text;
			StartCoroutine(notificationObj.DisplayRoutine ());
		}

		public virtual void UpdateDropdowns ()
		{
			foreach (TMP_Dropdown dropdown in dropdowns)
				dropdown.onValueChanged.Invoke(dropdown.value);
		}

		public virtual void Update ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			// try
			// {
				if (Time.frameCount <= LAG_FRAMES_AFTER_LOAD_SCENE)
					return;
				Physics2D.Simulate(Time.deltaTime);
				foreach (IUpdatable updatable in updatables)
					updatable.DoUpdate ();
				if (GetSingleton<ObjectPool>() != null && GetSingleton<ObjectPool>().enabled)
					GetSingleton<ObjectPool>().DoUpdate ();
				activeCursorEntry.rectTrs.position = Input.mousePosition;
				framesSinceLoadedScene ++;
				if (InputManager.inputter.GetButtonDown("Reset"))
					ReloadActiveScene ();
			// }
			// catch (Exception e)
			// {
			// 	Debug.Log(e.Message + "\n" + e.StackTrace);
			// }
		}

		public virtual void OnSceneLoaded (Scene scene = new Scene(), LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			GetSingleton<SaveAndLoadManager>().Load ();
			// SetGosActive ();
			// PauseGame (false);
		}

		public static T GetSingleton<T> ()
		{
			if (!singletons.ContainsKey(typeof(T)))
				return GetSingleton<T>(FindObjectsOfType<Object>());
			else
			{
				if (singletons[typeof(T)] == null || singletons[typeof(T)].Equals(default(T)))
				{
					T singleton = GetSingleton<T>(FindObjectsOfType<Object>());
					singletons[typeof(T)] = singleton;
					return singleton;
				}
				else
					return (T) singletons[typeof(T)];
			}
		}

		public static T GetSingleton<T> (Object[] objects)
		{
			if (typeof(T).IsSubclassOf(typeof(Object)))
			{
				foreach (Object obj in objects)
				{
					if (obj is T)
					{
						singletons.Remove(typeof(T));
						singletons.Add(typeof(T), obj);
						break;
					}
				}
			}
			if (singletons.ContainsKey(typeof(T)))
				return (T) singletons[typeof(T)];
			else
				return default(T);
		}

		public virtual void _LoadScene (string name)
		{
			LoadScene (name);
		}

		public static void LoadScene (string name)
		{
			framesSinceLoadedScene = 0;
			SceneManager.LoadScene(name);
		}

		public virtual void LoadScene (int index)
		{
			LoadScene (SceneManager.GetSceneByBuildIndex(index).name);
		}

		public virtual void ReloadActiveScene ()
		{
			GetSingleton<SaveAndLoadManager>().Save ();
			LoadScene (SceneManager.GetActiveScene().name);
		}

		public virtual void PauseGame (bool pause)
		{
			paused = pause;
			Time.timeScale = timeScale * (1 - paused.GetHashCode());
			AudioListener.pause = paused.GetHashCode() == 1;
		}

		public virtual void TogglePauseGame ()
		{
			PauseGame (!paused);
		}

		public virtual void Quit ()
		{
			Application.Quit();
		}

		public virtual void OnApplicationQuit ()
		{
			GetSingleton<SaveAndLoadManager>().Save ();
		}

		public virtual void OnApplicationFocus (bool isFocused)
		{
			if (isFocused)
			{
				if (Screen.fullScreenMode == FullScreenMode.Windowed)
					Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
				foreach (IUpdatable pausedUpdatable in pausedUpdatables)
					updatables = updatables.Add(pausedUpdatable);
				pausedUpdatables = new IUpdatable[0];
				foreach (Timer runningTimer in Timer.runningInstances)
					runningTimer.pauseIfCan = false;
				foreach (TemporaryDisplayObject displayedObject in TemporaryDisplayObject.displayedInstances)
					StartCoroutine(displayedObject.DisplayRoutine ());
			}
			else
			{
				IUpdatable updatable;
				for (int i = 0; i < updatables.Length; i ++)
				{
					updatable = updatables[i];
					if (!updatable.PauseWhileUnfocused)
					{
						pausedUpdatables = pausedUpdatables.Add(updatable);
						updatables = updatables.RemoveAt(i);
						i --;
					}
				}
				foreach (Timer runningTimer in Timer.runningInstances)
					runningTimer.pauseIfCan = true;
				foreach (TemporaryDisplayObject displayedObject in TemporaryDisplayObject.displayedInstances)
					StopCoroutine(displayedObject.DisplayRoutine ());
			}
		}

		// public virtual void SetGosActive ()
		// {
		// 	if (GetSingleton<GameManager>() != this)
		// 	{
		// 		GetSingleton<GameManager>().SetGosActive ();
		// 		return;
		// 	}
		// 	string[] stringSeperators = { STRING_SEPERATOR };
		// 	string[] enabledGos = enabledGosString.Split(stringSeperators, StringSplitOptions.None);
		// 	foreach (string goName in enabledGos)
		// 	{
		// 		for (int i = 0; i < registeredGos.Length; i ++)
		// 		{
		// 			if (goName == registeredGos[i].name)
		// 			{
		// 				registeredGos[i].SetActive(true);
		// 				break;
		// 			}
		// 		}
		// 	}
		// 	string[] disabledGos = disabledGosString.Split(stringSeperators, StringSplitOptions.None);
		// 	foreach (string goName in disabledGos)
		// 	{
		// 		GameObject go = GameObject.Find(goName);
		// 		if (go != null)
		// 			go.SetActive(false);
		// 	}
		// }
		
		// public virtual void ActivateGoForever (GameObject go)
		// {
		// 	go.SetActive(true);
		// 	ActivateGoForever (go.name);
		// }
		
		// public virtual void DeactivateGoForever (GameObject go)
		// {
		// 	go.SetActive(false);
		// 	DeactivateGoForever (go.name);
		// }
		
		// public virtual void ActivateGoForever (string goName)
		// {
		// 	disabledGosString = disabledGosString.Replace(STRING_SEPERATOR + goName, "");
		// 	if (!enabledGosString.Contains(goName))
		// 		enabledGosString += STRING_SEPERATOR + goName;
		// }
		
		// public virtual void DeactivateGoForever (string goName)
		// {
		// 	enabledGosString = enabledGosString.Replace(STRING_SEPERATOR + goName, "");
		// 	if (!disabledGosString.Contains(goName))
		// 		disabledGosString += STRING_SEPERATOR + goName;
		// }

		// public virtual void SetGameObjectActive (string name)
		// {
		// 	GameObject.Find(name).SetActive(true);
		// }

		// public virtual void SetGameObjectInactive (string name)
		// {
		// 	GameObject.Find(name).SetActive(false);
		// }

		public virtual void OnDestroy ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				EditorApplication.update -= Update;
				return;
			}
#endif
			SceneManager.sceneLoaded -= OnSceneLoaded;
			StopAllCoroutines();
			// if (GetSingleton<GameManager>() == this)
			// 	OnApplicationQuit ();
		}

		// public virtual void _Debug (object o)
		// {
		// 	Debug.LogError(o);
		// }

		public static Object Clone (Object obj)
		{
			return Instantiate(obj);
		}

		public static Object Clone (Object obj, Vector3 position, Quaternion rotation)
		{
			return Instantiate(obj, position, rotation);
		}

		public virtual void ToggleGameObject (GameObject go)
		{
			go.SetActive(!go.activeSelf);
		}

		public virtual bool MouseIsOverUnblockedWorldSpace ()
		{
			// if (!GetSingleton<Minimap>().worldRect.Contains(InputManager.GetWorldMousePosition()) || GetSingleton<EventSystem>().IsPointerOverGameObject())
			// 	return false;
			// foreach (RectTransform rectTrs in nonClickThroughRectTransforms)
			// {
			// 	if (rectTrs.GetWorldRect().Contains(Input.mousePosition))
			// 		return false;
			// }
			// return true;
			return !GetSingleton<EventSystem>().IsPointerOverGameObject();
		}

		public virtual void Minimize ()
		{
			Screen.fullScreenMode = FullScreenMode.Windowed;
		}

		public static void _Destroy (Object obj)
		{
			Destroy(obj);
		}

		public virtual void OpenURL (string url)
		{
			Application.OpenURL(url);
		}

		[Serializable]
		public class CursorEntry
		{
			public string name;
			public RectTransform rectTrs;

			public virtual void SetAsActive ()
			{
				if (activeCursorEntry != null)
					activeCursorEntry.rectTrs.gameObject.SetActive(false);
				rectTrs.gameObject.SetActive(true);
				activeCursorEntry = this;
			}
		}

		[Serializable]
		public class GameModifier
		{
			public string name;
			public bool isActive;
		}
	}
}
