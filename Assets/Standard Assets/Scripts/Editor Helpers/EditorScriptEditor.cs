#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using IRLFriend;
using Extensions;

[CustomEditor(typeof(EditorScript))]
public class EditorScriptEditor : Editor
{
	public virtual void OnSceneGUI ()
	{
		EditorScript editorScript = (EditorScript) target;
		editorScript.UpdateHotkeys ();
	}
}
#endif