using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class _Tilemap : MonoBehaviour, ISpawnable
{
	public Tilemap tilemap;
	public int prefabIndex;
	public int PrefabIndex
	{
		get
		{
			return prefabIndex;
		}
	}
}
