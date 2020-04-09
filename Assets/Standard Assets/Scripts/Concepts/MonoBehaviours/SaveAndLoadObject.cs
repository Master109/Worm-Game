﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.SceneManagement;
using System.Reflection;
using System;
using SavedObjectEntry = Worms.SaveAndLoadManager.SavedObjectEntry;
using Random = UnityEngine.Random;

namespace Worms
{
	[ExecuteInEditMode]
	public class SaveAndLoadObject : MonoBehaviour, IIdentifiable
	{
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
		public Transform[] saveableChildren = new Transform[0];
		public ISavableAndLoadable[] saveables = new ISavableAndLoadable[0];
		public string typeId;
		public SavedObjectEntry[] saveEntries = new SavedObjectEntry[0];
		
#if UNITY_EDITOR
		public virtual void Start ()
		{
			if (Application.isPlaying)
				return;
			if (uniqueId == 0)
				uniqueId = Random.Range(int.MinValue, int.MaxValue);
			Transform[] _saveableChildren = new Transform[0];
			_saveableChildren.AddRange(saveableChildren);
			Transform trs = GetComponent<Transform>();
			if (!_saveableChildren.Contains(trs) && GetComponentsInChildren<ISavableAndLoadable>().Length > 0)
			{
				_saveableChildren = _saveableChildren.Add(trs);
				saveableChildren = _saveableChildren;
			}
		}

		public virtual void Reset ()
		{
			Start ();
		}
#endif

		public virtual void Init ()
		{
			List<ISavableAndLoadable> _saveables = new List<ISavableAndLoadable>();
			foreach (Transform saveableChild in saveableChildren)
				_saveables.AddRange(saveableChild.GetComponentsInChildren<ISavableAndLoadable>());
			saveables = _saveables.ToArray();
			SaveAndLoadObject sameTypeObj;
			if (!SaveAndLoadManager.saveAndLoadObjectTypeDict.TryGetValue(typeId, out sameTypeObj))
			{
				MemberInfo memberInfo;
				saveEntries = new SavedObjectEntry[saveables.Length];
				for (int i = 0; i < saveables.Length; i ++)
				{
					SavedObjectEntry saveEntry = new SavedObjectEntry();
					saveEntry.saveAndLoadObject = this;
					saveEntry.saveableAndLoadable = saveables[i];
					saveEntry.members = saveEntry.members.AddRange(saveEntry.saveableAndLoadable.GetType().GetMembers());
					for (int i2 = 0; i2 < saveEntry.members.Length; i2 ++)
					{
						memberInfo = saveEntry.members[i2];
						SaveAndLoadValue saveAndLoadValue = Attribute.GetCustomAttribute(memberInfo, typeof(SaveAndLoadValue)) as SaveAndLoadValue;
						if (saveAndLoadValue == null)
						{
							saveEntry.members = saveEntry.members.RemoveAt(i2);
							i2 --;
						}
						else
							saveEntry.saveAndLoadValues = saveEntry.saveAndLoadValues.Add(saveAndLoadValue);
					}
					saveEntries[i] = saveEntry;
				}
				SaveAndLoadManager.saveAndLoadObjectTypeDict.Add(typeId, this);
			}
			else
			{
				saveEntries = sameTypeObj.saveEntries;
				SavedObjectEntry saveEntry;
				for (int i = 0; i < saveEntries.Length; i ++)
				{
					saveEntry = saveEntries[i];
					saveEntry.saveableAndLoadable = saveables[i];
					saveEntry.saveAndLoadObject = this;
					saveEntries[i] = saveEntry;
				}
			}
		}
	}
}