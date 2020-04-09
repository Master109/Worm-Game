﻿#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Worms;
using UnityEditor;

namespace Extensions
{
	public static class EditorPrefsExtensions
	{
		public const bool USE_REGISTRY = false;
		public const string REGISTRY_KEY = "All Keys And Values";
		public const string REGISTRY_SEPERATOR = "↕";
		public const string REGISTRY_ENTRY_DATA_SEPARATOR = "↔";

		public static void SetInt (string key, int value, bool registerKey = USE_REGISTRY)
		{
			EditorPrefs.SetInt(key, value);
			if (registerKey)
				RegisterKey (key, EditorPrefsValueType.Int);
		}

		public static void SetFloat (string key, float value, bool registerKey = USE_REGISTRY)
		{
			EditorPrefs.SetFloat(key, value);
			if (registerKey)
				RegisterKey (key, EditorPrefsValueType.Float);
		}

		public static void SetString (string key, string value, bool registerKey = USE_REGISTRY)
		{
			EditorPrefs.SetString(key, value);
			if (registerKey)
				RegisterKey (key, EditorPrefsValueType.String);
		}

		public static bool GetBool (string key, bool defaultValue = false)
		{
			int _defaultValue = 0;
			if (defaultValue)
				_defaultValue = 1;
			return SaveAndLoadManager.GetValue<int>(key, _defaultValue) == 1;
		}
		
		public static void SetBool (string key, bool value, bool registerKey = USE_REGISTRY)
		{
			SetInt (key, value.GetHashCode(), registerKey);
		}
		
		public static Color GetColor (string key)
		{
			return GetColor(key, Color.black.SetAlpha(0));
		}
		
		public static Color GetColor (string key, Color defaultValue)
		{
			return new Color(SaveAndLoadManager.GetValue<float>(key + ".r", defaultValue.r), SaveAndLoadManager.GetValue<float>(key + ".g", defaultValue.g), SaveAndLoadManager.GetValue<float>(key + ".b", defaultValue.b), SaveAndLoadManager.GetValue<float>(key + ".a", defaultValue.a));
		}
		
		public static void SetColor (string key, Color value, bool registerKey = USE_REGISTRY)
		{
			SetFloat (key + ".r", value.r, registerKey);
			SetFloat (key + ".g", value.g, registerKey);
			SetFloat (key + ".b", value.b, registerKey);
			SetFloat (key + ".a", value.a, registerKey);
		}
		
		public static Vector2 GetVector2 (string key, Vector2 defaultValue = new Vector2())
		{
			return new Vector2(SaveAndLoadManager.GetValue<float>(key + ".x", defaultValue.x), SaveAndLoadManager.GetValue<float>(key + ".y", defaultValue.y));
		}
		
		public static void SetVector2 (string key, Vector2 value, bool registerKey = USE_REGISTRY)
		{
			SetFloat (key + ".x", value.x, registerKey);
			SetFloat (key + ".y", value.y, registerKey);
		}

		public static void RegisterKey (string key, EditorPrefsValueType valueType)
		{
			string value = "";
			if (valueType == EditorPrefsValueType.Int)
				value = "" + SaveAndLoadManager.GetValue<int>(key);
			else if (valueType == EditorPrefsValueType.Float)
				value = "" + SaveAndLoadManager.GetValue<float>(key);
			else if (valueType == EditorPrefsValueType.String)
				value = SaveAndLoadManager.GetValue<string>(key);
			string registry = SaveAndLoadManager.GetValue<string>(REGISTRY_KEY, "");
			int indexOfKey = registry.IndexOf(key);
			if (indexOfKey != -1)
				registry = registry.RemoveStartEnd(indexOfKey, indexOfKey + registry.StartAfter(key).IndexOf(REGISTRY_SEPERATOR) + 1 + key.Length);
			EditorPrefs.SetString(REGISTRY_KEY, registry + key + REGISTRY_ENTRY_DATA_SEPARATOR + value + REGISTRY_ENTRY_DATA_SEPARATOR + valueType.ToString() + REGISTRY_SEPERATOR);
		}

		public static void DeregisterKey (string key)
		{
			string registry = SaveAndLoadManager.GetValue<string>(REGISTRY_KEY, "");
			int indexOfKey = registry.IndexOf(key);
			if (indexOfKey != -1)
				registry = registry.RemoveStartEnd(indexOfKey, indexOfKey + registry.StartAfter(key).IndexOf(REGISTRY_SEPERATOR) + 1 + key.Length);
			EditorPrefs.SetString(REGISTRY_KEY, registry);
		}

		public static void DeleteKey (string key, bool deregisterKey = USE_REGISTRY)
		{
			EditorPrefs.DeleteKey(key);
			if (deregisterKey)
				DeregisterKey (key);
		}

		public enum EditorPrefsValueType
		{
			Int,
			Float,
			String
		}
	}
}
#endif