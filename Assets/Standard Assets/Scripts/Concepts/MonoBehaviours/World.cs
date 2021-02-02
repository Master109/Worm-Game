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
			List<Vector3Int> remainingCellLocations = new List<Vector3Int>();
			foreach (Vector3Int cellLocation in tilemap.cellBounds.allPositionsWithin)
				remainingCellLocations.Add(cellLocation);
			while (remainingCellLocations.Count > 0)
			{
				Island island = GetIsland(tilemap, tilemap.cellBounds.min, diagonalsConnect);
				if (output.Count > 0)
					island.tilemap = null;
				Vector3Int[] cellLocations = new Vector3Int[island.tilesDict.Count];
				island.tilesDict.Keys.CopyTo(cellLocations, 0);
				for (int i = 0; i < cellLocations.Length; i ++)
				{
					Vector3Int cellLocation = cellLocations[i];
					remainingCellLocations.Remove(cellLocation);
				}
				if (remainingCellLocations.Count > 0)
					remainingCellLocations.RemoveAt(0);
				output.Add(island);
			}
			// Island island = new Island(new KeyValuePair<Vector3Int, TileBase>[0], tilemap);
			// List<Vector3Int> checkedCellLocations = new List<Vector3Int>();
			// List<Vector3Int> uncheckedCellLocations = new List<Vector3Int>();
			// foreach (Vector3Int cellLocation in tilemap.cellBounds.allPositionsWithin)
			// 	uncheckedCellLocations.Add(cellLocation);
			// while (checkedCellLocations.Count < tilemap.cellBounds.size.x * tilemap.cellBounds.size.y)
			// // while (uncheckedCellLocations.Count > 0)
			// {
			// 	while (uncheckedCellLocations.Count > 0)
			// 	{
			// 		Vector3Int cellLocation = uncheckedCellLocations[0];
			// 		TileBase tile = tilemap.GetTile(cellLocation);
			// 		if (tile != null)
			// 		{
			// 			// if (uncheckedCellLocations.Contains(cellLocation))
			// 			island.tilesDict.Add(cellLocation, tile);
			// 			// CheckTile (tilemap.cellBounds.ToRectInt(), cellLocation, ref island);
			// 			// CheckTile (tilemap.cellBounds.ToRectInt(), cellLocation, diagonalsConnect, ref uncheckedCellLocations, ref checkedCellLocations);
			// 		}
			// 		uncheckedCellLocations.RemoveAt(0);
			// 		// if (uncheckedCellLocations.Count > 0)
			// 			// uncheckedCellLocations.RemoveAt(0);
			// 			// uncheckedCellLocations.Remove(cellLocation);
			// 		// if (!checkedCellLocations.Contains(cellLocation))
			// 			checkedCellLocations.Add(cellLocation);
			// 	}
			// 	output.Add(island);
			// 	// if (uncheckedCellLocations.Count > 0)
			// 	// {
			// 		island = new Island(new KeyValuePair<Vector3Int, TileBase>[0], null);
			// 		// uncheckedCellLocations.Add(uncheckedCellLocations[0]);
			// 	// }
			// }
			return output.ToArray();
		}

		// void CheckTile (RectInt checkRect, Vector3Int cellLocation, ref Island island)
		// {
		// 	if (cellLocation.x > checkRect.xMin && !checkedCellLocations.Contains(cellLocation + Vector3Int.left) && !uncheckedCellLocations.Contains(cellLocation + Vector3Int.left))
		// 	{
		// 		island.remainingCellLocations.Add(cellLocation + Vector3Int.left);
		// 		// checkedCellLocations.Add(cellLocation + Vector3Int.left);
		// 	}
		// 	if (cellLocation.x < checkRect.xMax && !checkedCellLocations.Contains(cellLocation + Vector3Int.right) && !uncheckedCellLocations.Contains(cellLocation + Vector3Int.right))
		// 	{
		// 		island.remainingCellLocations.Add(cellLocation + Vector3Int.right);
		// 		// checkedCellLocations.Add(cellLocation + Vector3Int.right);
		// 	}
		// 	if (cellLocation.y > checkRect.yMin && !checkedCellLocations.Contains(cellLocation + Vector3Int.down) && !uncheckedCellLocations.Contains(cellLocation + Vector3Int.down))
		// 	{
		// 		island.remainingCellLocations.Add(cellLocation + Vector3Int.down);
		// 		// checkedCellLocations.Add(cellLocation + Vector3Int.down);
		// 	}
		// 	if (cellLocation.y < checkRect.yMax && !checkedCellLocations.Contains(cellLocation + Vector3Int.up) && !uncheckedCellLocations.Contains(cellLocation + Vector3Int.up))
		// 	{
		// 		island.remainingCellLocations.Add(cellLocation + Vector3Int.up);
		// 		// checkedCellLocations.Add(cellLocation + Vector3Int.up);
		// 	}
		// 	// if (diagonalsConnect)
		// 	// {
		// 	// 	if (!checkedCellLocations.Contains(cellLocation + Vector3Int.left + Vector3Int.up))
		// 	// 		remainingCellLocations.Add(cellLocation + Vector3Int.left + Vector3Int.up);
		// 	// 	if (!checkedCellLocations.Contains(cellLocation + Vector3Int.left + Vector3Int.up))
		// 	// 		remainingCellLocations.Add(cellLocation + Vector3Int.left + Vector3Int.up);
		// 	// 	if (!checkedCellLocations.Contains(cellLocation + Vector3Int.right + Vector3Int.down))
		// 	// 		remainingCellLocations.Add(cellLocation + Vector3Int.right + Vector3Int.down);
		// 	// 	if (!checkedCellLocations.Contains(cellLocation + Vector3Int.right + Vector3Int.up))
		// 	// 		remainingCellLocations.Add(cellLocation + Vector3Int.right + Vector3Int.up);
		// 	// }
		// }

		// void CheckTile (RectInt checkRect, Vector3Int cellLocation, bool diagonalsConnect, ref List<Vector3Int> uncheckedCellLocations, ref List<Vector3Int> checkedCellLocations)
		// {
		// 	if (cellLocation.x > checkRect.xMin && !checkedCellLocations.Contains(cellLocation + Vector3Int.left) && !uncheckedCellLocations.Contains(cellLocation + Vector3Int.left))
		// 	{
		// 		uncheckedCellLocations.Add(cellLocation + Vector3Int.left);
		// 		// checkedCellLocations.Add(cellLocation + Vector3Int.left);
		// 	}
		// 	if (cellLocation.x < checkRect.xMax && !checkedCellLocations.Contains(cellLocation + Vector3Int.right) && !uncheckedCellLocations.Contains(cellLocation + Vector3Int.right))
		// 	{
		// 		uncheckedCellLocations.Add(cellLocation + Vector3Int.right);
		// 		// checkedCellLocations.Add(cellLocation + Vector3Int.right);
		// 	}
		// 	if (cellLocation.y > checkRect.yMin && !checkedCellLocations.Contains(cellLocation + Vector3Int.down) && !uncheckedCellLocations.Contains(cellLocation + Vector3Int.down))
		// 	{
		// 		uncheckedCellLocations.Add(cellLocation + Vector3Int.down);
		// 		// checkedCellLocations.Add(cellLocation + Vector3Int.down);
		// 	}
		// 	if (cellLocation.y < checkRect.yMax && !checkedCellLocations.Contains(cellLocation + Vector3Int.up) && !uncheckedCellLocations.Contains(cellLocation + Vector3Int.up))
		// 	{
		// 		uncheckedCellLocations.Add(cellLocation + Vector3Int.up);
		// 		// checkedCellLocations.Add(cellLocation + Vector3Int.up);
		// 	}
		// 	// if (diagonalsConnect)
		// 	// {
		// 	// 	if (!checkedCellLocations.Contains(cellLocation + Vector3Int.left + Vector3Int.up))
		// 	// 		remainingCellLocations.Add(cellLocation + Vector3Int.left + Vector3Int.up);
		// 	// 	if (!checkedCellLocations.Contains(cellLocation + Vector3Int.left + Vector3Int.up))
		// 	// 		remainingCellLocations.Add(cellLocation + Vector3Int.left + Vector3Int.up);
		// 	// 	if (!checkedCellLocations.Contains(cellLocation + Vector3Int.right + Vector3Int.down))
		// 	// 		remainingCellLocations.Add(cellLocation + Vector3Int.right + Vector3Int.down);
		// 	// 	if (!checkedCellLocations.Contains(cellLocation + Vector3Int.right + Vector3Int.up))
		// 	// 		remainingCellLocations.Add(cellLocation + Vector3Int.right + Vector3Int.up);
		// 	// }
		// 	checkedCellLocations.Add(cellLocation);
		// }

		Island GetIsland (Tilemap tilemap, Vector3Int cellLocation, bool diagonalsConnect)
		{
			Island output = new Island(new KeyValuePair<Vector3Int, TileBase>[0], tilemap);
			List<Vector3Int> checkedCellLocations = new List<Vector3Int>();
			List<Vector3Int> uncheckedCellLocations = new List<Vector3Int>();
			uncheckedCellLocations.Add(cellLocation);
			List<Vector3Int> remainingCellLocations = new List<Vector3Int>();
			foreach (Vector3Int _cellLocation in tilemap.cellBounds.allPositionsWithin)
				remainingCellLocations.Add(_cellLocation);
			remainingCellLocations.Remove(cellLocation);
			while (uncheckedCellLocations.Count > 0)
			{
				Vector3Int _cellLocation = uncheckedCellLocations[0];
				TileBase tile = tilemap.GetTile(_cellLocation);
				if (tile != null)
				{
					output.tilesDict.Add(_cellLocation, tile);
					AddConnectedTiles (tilemap.cellBounds.ToRectInt(), _cellLocation, diagonalsConnect, ref uncheckedCellLocations, ref remainingCellLocations);
				}
				uncheckedCellLocations.RemoveAt(0);
			}
			return output;
		}

		void AddConnectedTiles (RectInt checkRect, Vector3Int cellLocation, bool diagonalsConnect, ref List<Vector3Int> uncheckedCellLocations, ref List<Vector3Int> remainingCellLocations)
		{
			if (cellLocation.x > checkRect.xMin && remainingCellLocations.Contains(cellLocation + Vector3Int.left))
			{
				remainingCellLocations.Remove(cellLocation + Vector3Int.left);
				uncheckedCellLocations.Add(cellLocation + Vector3Int.left);
			}
			if (cellLocation.x < checkRect.xMax && remainingCellLocations.Contains(cellLocation + Vector3Int.right))
			{
				remainingCellLocations.Remove(cellLocation + Vector3Int.right);
				uncheckedCellLocations.Add(cellLocation + Vector3Int.right);
			}
			if (cellLocation.y > checkRect.yMin && remainingCellLocations.Contains(cellLocation + Vector3Int.down))
			{
				remainingCellLocations.Remove(cellLocation + Vector3Int.down);
				uncheckedCellLocations.Add(cellLocation + Vector3Int.down);
			}
			if (cellLocation.y < checkRect.yMax && remainingCellLocations.Contains(cellLocation + Vector3Int.up))
			{
				remainingCellLocations.Remove(cellLocation + Vector3Int.up);
				uncheckedCellLocations.Add(cellLocation + Vector3Int.up);
			}
		}

		public struct Island
		{
			public Dictionary<Vector3Int, TileBase> tilesDict;
			public BoundsInt bounds;
			public Tilemap tilemap;

			public Island (KeyValuePair<Vector3Int, TileBase>[] tilesAndLocations, Tilemap tilemap)
			{
				tilesDict = new Dictionary<Vector3Int, TileBase>();
				bounds = new BoundsInt();
				if (tilesAndLocations.Length > 0)
				{
					KeyValuePair<Vector3Int, TileBase> keyValuePair = tilesAndLocations[0];
					Vector3Int cellLocation = keyValuePair.Key;
					bounds.SetMinMax(cellLocation, cellLocation);
					for (int i = 1; i < tilesAndLocations.Length; i ++)
					{
						keyValuePair = tilesAndLocations[i];
						cellLocation = keyValuePair.Key;
						tilesDict.Add(cellLocation, keyValuePair.Value);
						if (cellLocation.x > bounds.xMax)
							bounds.xMax = cellLocation.x;
						else if (cellLocation.x < bounds.xMin)
							bounds.xMin = cellLocation.x;
						if (cellLocation.y > bounds.yMax)
							bounds.yMax = cellLocation.y;
						else if (cellLocation.y < bounds.yMin)
							bounds.yMin = cellLocation.y;
					}
				}
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
				tilemap.CompressBounds();
				World.tilemaps.Add(tilemap);
			}
		}
	}
}