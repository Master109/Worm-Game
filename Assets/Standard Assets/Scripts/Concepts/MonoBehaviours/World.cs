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

		void Start ()
		{
			tilemaps.Clear();
			tilemaps.Add(tilemap);
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
			BoundsInt cellBounds = tilemap.cellBounds;
			bool hasIsland = false;
			foreach (Vector3Int cellLocation in cellBounds.allPositionsWithin)
			{
				TileBase tile = tilemap.GetTile(cellLocation);
				if (tile != null)
				{
					if ((!tilemap.HasTile(cellLocation + Vector3Int.left) && !tilemap.HasTile(cellLocation + Vector3Int.right) && !tilemap.HasTile(cellLocation + Vector3Int.down) && !tilemap.HasTile(cellLocation + Vector3Int.up)) || output.Count == 0)
					{
						Tilemap _tilemap = null;
						if (!hasIsland)
						{
							_tilemap = tilemap;
							hasIsland = true;
						}
						Island island = new Island(new KeyValuePair<Vector3Int, TileBase>[1] { new KeyValuePair<Vector3Int, TileBase>(cellLocation, tile) }, _tilemap);
						output.Add(island);
					}
					else
					{
						for (int i = 0; i < output.Count; i ++)
						{
							Island island = output[i];
							Dictionary<Vector3Int, TileBase> tilesDict = island.tilesDict;
							if (!diagonalsConnect)
							{
								if (tilesDict.ContainsKey(cellLocation + Vector3Int.left) || tilesDict.ContainsKey(cellLocation + Vector3Int.right) || tilesDict.ContainsKey(cellLocation + Vector3Int.down) || tilesDict.ContainsKey(cellLocation + Vector3Int.up))
								{
									island.tilesDict.Add(cellLocation, tile);
									output[i] = island;
								}
							}
							else if (tilesDict.ContainsKey(cellLocation + Vector3Int.left) || tilesDict.ContainsKey(cellLocation + Vector3Int.right) || tilesDict.ContainsKey(cellLocation + Vector3Int.down) || tilesDict.ContainsKey(cellLocation + Vector3Int.up) || tilesDict.ContainsKey(cellLocation + Vector3Int.right + Vector3Int.up) || tilesDict.ContainsKey(cellLocation + Vector3Int.right + Vector3Int.down) || tilesDict.ContainsKey(cellLocation + Vector3Int.left + Vector3Int.up) || tilesDict.ContainsKey(cellLocation + Vector3Int.left + Vector3Int.down))
							{
								island.tilesDict.Add(cellLocation, tile);
								output[i] = island;
							}
						}
					}
				}
			}
			return output.ToArray();
		}

		public struct Island
		{
			public Dictionary<Vector3Int, TileBase> tilesDict;
			public Tilemap tilemap;

			public Island (KeyValuePair<Vector3Int, TileBase>[] tiles, Tilemap tilemap)
			{
				tilesDict = new Dictionary<Vector3Int, TileBase>();
				for (int i = 0; i < tiles.Length; i ++)
				{
					KeyValuePair<Vector3Int, TileBase> keyValuePair = tiles[i];
					tilesDict.Add(keyValuePair.Key, keyValuePair.Value);
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
				World.tilemaps.Add(tilemap);
			}
		}
	}
}