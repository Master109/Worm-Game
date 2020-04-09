// SaveAndLoadManager

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System.Reflection;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using FullSerializer;

namespace Worms
{
	/*
	This class is in charge of saving (".Save ()") and loading (".Load ()") data between sessions (and switching scenes)
	Data is stored (serialized) using JSON format with Unity's PlayerPrefs system
	To reigster values to be saved and loaded do any of the following:
		1) Make the value a property and use GetValue and SetValue in the get and set methods. Example:
			int MyInt
			{
				get
				{
					SaveAndLoadManager.GetValue<int>("MyInt");
				}
				set
				{
					SaveAndLoadManager.SetValue("MyInt", value);
				}
			}
		2) If the value is defined in a MonoBehaviour, then use the [SaveAndLoadValue] attribute before the variable declaration.
		The MonoBehaviour must also implement the ISavableAndLoadable interface. Next, the GameObject that the MonoBehaviour is found
		on must have a SaveAndLoadObject on it or any of it's parents. After that, the SaveAndLoadObject needs the Transform of the
		GameObject the MonoBehaviour is found on added to the SaveAndLoadObject's "saveableChildren" array. Finally, the
		SaveAndLoadObject must be added to SaveAndLoadManager's "saveAndLoadObjects" array. Also, note that the "uniqueId" member of
		each SaveAndLoadObject must be unique. Examples:
			public class MyMonoBehaviour : MonoBehaviour, ISavableAndLoadable
			{
				[SaveAndLoadValue]
				public int myInt;
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
			}
---------------------------------------------------------------------------------
			public class MyMonoBehaviour : SaveAndLoadObject, ISavableAndLoadable
			{
				[SaveAndLoadValue]
				public int myInt;
			}
	*/
	[ExecuteInEditMode]
	public class SaveAndLoadManager : MonoBehaviour
	{
		public static fsSerializer serializer = new fsSerializer();
		// [HideInInspector]
		public SaveAndLoadObject[] saveAndLoadObjects = new SaveAndLoadObject[0];
		public static SavedObjectEntry[] savedObjectEntries = new SavedObjectEntry[0];
		public static Dictionary<string, SaveAndLoadObject> saveAndLoadObjectTypeDict = new Dictionary<string, SaveAndLoadObject>();
		public static Dictionary<string, string> savedData = new Dictionary<string, string>();
		public static Dictionary<string, object> data = new Dictionary<string, object>();
		public const string PLAYER_PREFS_KEY = "Saved Data";
		public const string DATA_SEPARATOR = "☒";

// 		public virtual void Awake ()
// 		{
// #if UNITY_EDITOR
// 			if (!Application.isPlaying)
// 			{
// 				saveAndLoadObjects = FindObjectsOfType<SaveAndLoadObject>();
// 				for (int i = 0; i < saveAndLoadObjects.Length; i ++)
// 				{
// 					if (!saveAndLoadObjects[i].enabled)
// 					{
// 						saveAndLoadObjects = saveAndLoadObjects.RemoveAt(i);
// 						i --;
// 					}
// 				}
// 				return;
// 			}
// #endif
// 		}
		
		// Returns a string in JSON format representing the object of the given type
		public static string Serialize (object value, Type type)
		{
			fsData data;
			serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();
			return fsJsonPrinter.CompressedJson(data);
		}
		
		// Returns the object given the string in JSON format that represents it and the object type
		public static object Deserialize (string serializedState, Type type)
		{
			fsData data = fsJsonParser.Parse(serializedState);
			object deserialized = null;
			serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();
			return deserialized;
		}

		// To be used with Unity's UI system. Unity's Button component can't call static methods, among other restrictions.
		public virtual void _SetValue (string key, object value)
		{
			SetValue (key, value);
		}

		public static void SetValue (string key, object value)
		{
			if (data.ContainsKey(key))
				data[key] = value;
			else
				data.Add(key, value);
		}

		public static T GetValue<T> (string key, T defaultValue = default(T))
		{
			object value;
			if (data.TryGetValue(key, out value))
				return (T) value;
			else
				return defaultValue;
		}

		public virtual void Init ()
		{
			SaveAndLoadObject saveAndLoadObject;
			List<SavedObjectEntry> _savedObjectEntries = new List<SavedObjectEntry>();
			for (int i = 0; i < saveAndLoadObjects.Length; i ++)
			{
				saveAndLoadObject = saveAndLoadObjects[i];
				if (saveAndLoadObject != null)
				{
					saveAndLoadObject.Init ();
					_savedObjectEntries.AddRange(saveAndLoadObject.saveEntries);
				}
			}
			savedObjectEntries = _savedObjectEntries.ToArray();
		}
		
		public virtual void Save ()
		{
			if (GameManager.GetSingleton<SaveAndLoadManager>() != this)
			{
				GameManager.GetSingleton<SaveAndLoadManager>().Save ();
				return;
			}
			Init ();
			for (int i = 0; i < savedObjectEntries.Length; i ++)
				savedObjectEntries[i].Save ();
			string playerPrefsValue = "";
			foreach (KeyValuePair<string, object> keyValuePair in data)
				playerPrefsValue += keyValuePair.Key.Replace(DATA_SEPARATOR, "") + DATA_SEPARATOR + Serialize(keyValuePair.Value, keyValuePair.Value.GetType()) + DATA_SEPARATOR;
			PlayerPrefs.SetString(PLAYER_PREFS_KEY, playerPrefsValue);
		}
		
		public virtual void Load ()
		{
			if (GameManager.GetSingleton<SaveAndLoadManager>() != this)
			{
				GameManager.GetSingleton<SaveAndLoadManager>().Load ();
				return;
			}
			saveAndLoadObjectTypeDict.Clear();
			Init ();
			string playerPrefsValue = PlayerPrefs.GetString(PLAYER_PREFS_KEY, "");
			if (string.IsNullOrEmpty(playerPrefsValue))
			{
				return;
			}
			string[] _savedData = playerPrefsValue.Split(new string[] { DATA_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
			savedData.Clear();
			for (int i = 1; i < _savedData.Length; i += 2)
				savedData.Add(_savedData[i - 1], _savedData[i]);
			for (int i = 0; i < savedObjectEntries.Length; i ++)
				savedObjectEntries[i].Load ();
		}

		public class SavedObjectEntry
		{
			public SaveAndLoadObject saveAndLoadObject;
			public ISavableAndLoadable saveableAndLoadable;
			public MemberInfo[] members = new MemberInfo[0];
			public SaveAndLoadValue[] saveAndLoadValues = new SaveAndLoadValue[0];
			public const string INFO_SEPARATOR = "↕";
			
			public SavedObjectEntry ()
			{
			}

			public virtual string GetKeyForMember (MemberInfo member)
			{
				return "" + saveableAndLoadable.UniqueId + INFO_SEPARATOR + member.Name;
			}
			
			public virtual void Save ()
			{
				foreach (MemberInfo member in members)
				{
					PropertyInfo property = member as PropertyInfo;
					if (property != null)
						SetValue (GetKeyForMember(member), property.GetValue(saveableAndLoadable, null));
					else
					{
						FieldInfo field = member as FieldInfo;
						if (field != null)
							SetValue (GetKeyForMember(member), field.GetValue(saveableAndLoadable));
					}
				}
			}
			
			public virtual void Load ()
			{
				string valueString = "";
				object value;
				foreach (MemberInfo member in members)
				{
					PropertyInfo property = member as PropertyInfo;
					if (property != null)
					{
						if (savedData.TryGetValue(GetKeyForMember(property), out valueString))
						{
							value = Deserialize(valueString, property.PropertyType);
							property.SetValue(saveableAndLoadable, value, null);
						}
					}
					else
					{
						FieldInfo field = member as FieldInfo;
						if (field != null)
						{
							if (savedData.TryGetValue(GetKeyForMember(field), out valueString))
							{
								value = Deserialize(valueString, field.FieldType);
								field.SetValue(saveableAndLoadable, value);
							}
						}
					}
				}
			}
		}
	}
}