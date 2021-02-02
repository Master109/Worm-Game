using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Extensions;

namespace Worms
{
	public class World : SingletonMonoBehaviour<World>
	{
		public Transform gridTrs;
		public Transform tilemapTrs;
		public Tilemap tilemap;
		public _Tilemap tilemapPrefab;
		public static List<Tilemap> tilemaps = new List<Tilemap>();
		// public static List<Vector3Int> filledCellLocations = new List<Vector3Int>();
		// public static Island[] islands = new Island[0];

		void Start ()
		{
			tilemaps.Clear();
			tilemaps.Add(tilemap);
			// filledCellLocations.Clear();
			// islands = new Island[1];
			// Island island = new Island(new KeyValuePair<Vector3Int, TileBase>[0], tilemap);
			// foreach (Vector3Int cellLocation in tilemap.cellBounds.allPositionsWithin)
			// {
			// 	if (tilemap.HasTile(cellLocation))
			// 		filledCellLocations.Add(cellLocation);
			// }
			// islands[0] = island;
		}

		public Rect CellToRect (Vector2Int cellPosition)
		{
			Vector2 min = tilemap.CellToWorld(cellPosition.ToVec3Int());
			Vector2 max = tilemap.CellToWorld(new Vector3Int(cellPosition.x + 1, cellPosition.y + 1, 0));
			Rect output = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
			output.position -= output.size / 2;
			return output;
		}

		public Island[] GetIslands (Tilemap tilemap, bool diagonalsConnect)
		{
			List<Island> output = new List<Island>();
			Island island = new Island(new KeyValuePair<Vector3Int, TileBase>[0], tilemap);
			List<Vector3Int> checkedCellLocations = new List<Vector3Int>();
			List<Vector3Int> uncheckedCellLocations = new List<Vector3Int>();
			foreach (Vector3Int cellLocation in tilemap.cellBounds.allPositionsWithin)
				uncheckedCellLocations.Add(cellLocation);
			uncheckedCellLocations.Remove(tilemap.cellBounds.min);
			while (checkedCellLocations.Count < tilemap.cellBounds.size.x * tilemap.cellBounds.size.y)
			// while (uncheckedCellLocations.Count > 0)
			{
				while (uncheckedCellLocations.Count > 0)
				{
					Vector3Int cellLocation = uncheckedCellLocations[0];
					TileBase tile = tilemap.GetTile(cellLocation);
					if (tile != null)
					// {
						// if (uncheckedCellLocations.Contains(cellLocation))
							island.tilesDict.Add(cellLocation, tile);
						// CheckTile (tilemap.cellBounds.ToRectInt(), cellLocation, diagonalsConnect, ref uncheckedCellLocations, ref checkedCellLocations);
					// }
					uncheckedCellLocations.RemoveAt(0);
					// if (uncheckedCellLocations.Count > 0)
						// uncheckedCellLocations.RemoveAt(0);
						// uncheckedCellLocations.Remove(cellLocation);
					// if (!checkedCellLocations.Contains(cellLocation))
						checkedCellLocations.Add(cellLocation);
				}
				output.Add(island);
				// if (uncheckedCellLocations.Count > 0)
				// {
					island = new Island(new KeyValuePair<Vector3Int, TileBase>[0], null);
					// uncheckedCellLocations.Add(uncheckedCellLocations[0]);
				// }
			}
			return output.ToArray();
		}

		void CheckTile (RectInt checkRect, Vector3Int cellLocation, bool diagonalsConnect, ref List<Vector3Int> uncheckedCellLocations, ref List<Vector3Int> checkedCellLocations)
		{
			if (cellLocation.x > checkRect.xMin && !checkedCellLocations.Contains(cellLocation + Vector3Int.left) && !uncheckedCellLocations.Contains(cellLocation + Vector3Int.left))
			{
				uncheckedCellLocations.Add(cellLocation + Vector3Int.left);
				// checkedCellLocations.Add(cellLocation + Vector3Int.left);
			}
			if (cellLocation.x < checkRect.xMax && !checkedCellLocations.Contains(cellLocation + Vector3Int.right) && !uncheckedCellLocations.Contains(cellLocation + Vector3Int.right))
			{
				uncheckedCellLocations.Add(cellLocation + Vector3Int.right);
				// checkedCellLocations.Add(cellLocation + Vector3Int.right);
			}
			if (cellLocation.y > checkRect.yMin && !checkedCellLocations.Contains(cellLocation + Vector3Int.down) && !uncheckedCellLocations.Contains(cellLocation + Vector3Int.down))
			{
				uncheckedCellLocations.Add(cellLocation + Vector3Int.down);
				// checkedCellLocations.Add(cellLocation + Vector3Int.down);
			}
			if (cellLocation.y < checkRect.yMax && !checkedCellLocations.Contains(cellLocation + Vector3Int.up) && !uncheckedCellLocations.Contains(cellLocation + Vector3Int.up))
			{
				uncheckedCellLocations.Add(cellLocation + Vector3Int.up);
				// checkedCellLocations.Add(cellLocation + Vector3Int.up);
			}
			// if (diagonalsConnect)
			// {
			// 	if (!checkedCellLocations.Contains(cellLocation + Vector3Int.left + Vector3Int.up))
			// 		remainingCellLocations.Add(cellLocation + Vector3Int.left + Vector3Int.up);
			// 	if (!checkedCellLocations.Contains(cellLocation + Vector3Int.left + Vector3Int.up))
			// 		remainingCellLocations.Add(cellLocation + Vector3Int.left + Vector3Int.up);
			// 	if (!checkedCellLocations.Contains(cellLocation + Vector3Int.right + Vector3Int.down))
			// 		remainingCellLocations.Add(cellLocation + Vector3Int.right + Vector3Int.down);
			// 	if (!checkedCellLocations.Contains(cellLocation + Vector3Int.right + Vector3Int.up))
			// 		remainingCellLocations.Add(cellLocation + Vector3Int.right + Vector3Int.up);
			// }
			checkedCellLocations.Add(cellLocation);
		}

		public struct Island
		{
			public Dictionary<Vector3Int, TileBase> tilesDict;
			public List<Vector3Int> remainingCellLocations;
			public Tilemap tilemap;

			public Island (KeyValuePair<Vector3Int, TileBase>[] tiles, Tilemap tilemap)
			{
				tilesDict = new Dictionary<Vector3Int, TileBase>();
				for (int i = 0; i < tiles.Length; i ++)
				{
					KeyValuePair<Vector3Int, TileBase> keyValuePair = tiles[i];
					tilesDict.Add(keyValuePair.Key, keyValuePair.Value);
				}
				remainingCellLocations = new List<Vector3Int>();
				this.tilemap = tilemap;
			}

			public void Split ()
			{
				Vector3Int[] cellLocations = new Vector3Int[tilesDict.Count];
				tilesDict.Keys.CopyTo(cellLocations, 0);
				World.instance.tilemap.SetTiles(cellLocations, new TileBase[tilesDict.Count]);
				_Tilemap _tilemap = ObjectPool.instance.SpawnComponent<_Tilemap>(World.instance.tilemapPrefab.prefabIndex, World.instance.tilemapTrs.position, World.instance.tilemapTrs.rotation, World.instance.gridTrs);
				TileBase[] tiles = new TileBase[tilesDict.Count];
				tilesDict.Values.CopyTo(tiles, 0);
				tilemap = _tilemap.tilemap;
				tilemap.SetTiles(cellLocations, tiles);
				World.tilemaps.Add(tilemap);
			}
		}
	}
}