using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.Tilemaps;

namespace Worms
{
	public class Player : SingletonMonoBehaviour<Player>, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return false;
			}
		}
		public Transform trs;
		public Rigidbody2D rigid;
		public LineRenderer lineRenderer;
		public float length;
		public Vector2[] localVerticies = new Vector2[0];
		public int vertexCount;
		public EdgeCollider2D edgeCollider;
		public float width;
		public float moveSpeed;
		Vector2 previousHeadLocalVertex;
		float localVertexDistance;
		public float crashCheckDistance;
		public Transform headTrs;
		public PolygonCollider2D headCollider;
		public int headCheckTileRange;
		public LineSegment2D[] headLineSegments = new LineSegment2D[0];
		// public RectInt headCheckTileRect;

		public virtual void Start ()
		{
			lineRenderer.startWidth = width;
			lineRenderer.endWidth = width;
			edgeCollider.edgeRadius = width / 2;
			lineRenderer.positionCount = vertexCount;
			localVerticies = new Vector2[vertexCount];
			localVertexDistance = 1f / (vertexCount - 1) * length;
			for (int i = 0; i < vertexCount; i ++)
			{
				Vector2 vertex = -trs.up * localVertexDistance * i;
				localVerticies[i] = vertex;
				lineRenderer.SetPosition(i, vertex);
			}
			edgeCollider.points = localVerticies;
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public virtual void DoUpdate ()
		{
			HandleMovement ();
		}

		public virtual void HandleMovement ()
		{
			// Move (InputManager.GetAxis2D("Move Horizontal", "Move Vertical"));
			Move (Vector2.ClampMagnitude(GameCamera.instance.camera.ScreenToWorldPoint(Input.mousePosition) - GameCamera.instance.camera.ViewportToWorldPoint(Vector2.one / 2), 1));
		}

		public virtual void Move (Vector2 move)
		{
			if (IsFalling())
			// if (Physics2D.Raycast((Vector2) trs.position + localVerticies[0] + (move.normalized * (width / 2 + crashCheckDistance)), move, moveAmount, LayerMask.GetMask("Player", "Wall")).collider != null || IsFalling())
				return;
			Vector2 headLocalVertex = localVerticies[0];
			float moveAmount = moveSpeed * Time.deltaTime;
			headLocalVertex += move * moveAmount;
			localVerticies[0] = headLocalVertex;
			float moveDistance = Vector2.Distance(headLocalVertex, previousHeadLocalVertex);
			Vector2 previousLocalVertex = headLocalVertex;
			for (int i = 1; i < vertexCount; i ++)
			{
				Vector2 localVertex = previousLocalVertex - (previousLocalVertex - localVerticies[i]).normalized * localVertexDistance;
				localVerticies[i] = localVertex;
				previousLocalVertex = localVertex;
			}
			headTrs.localPosition = headLocalVertex;
			headTrs.up = headLocalVertex - previousHeadLocalVertex;
			// Physics2D.SyncTransforms();
			previousHeadLocalVertex = headLocalVertex;
			for (int i = 0; i < vertexCount; i ++)
				lineRenderer.SetPosition(i, localVerticies[i]);
			edgeCollider.points = localVerticies;
			if (move != Vector2.zero)
				DestroyTerrain ();
		}

		void DestroyTerrain ()
		{
			headLineSegments[0] = new LineSegment2D(headTrs.TransformPoint(headCollider.points[0]), headTrs.TransformPoint(headCollider.points[1]));
			headLineSegments[1] = new LineSegment2D(headTrs.TransformPoint(headCollider.points[1]), headTrs.TransformPoint(headCollider.points[2]));
			Vector3Int headCellPosition = World.Instance.tilemap.WorldToCell(headTrs.position);
			List<Vector3Int> cellPositions = new List<Vector3Int>();
			for (int x = headCellPosition.x - headCheckTileRange; x < headCellPosition.x + headCheckTileRange; x ++)
			{
				for (int y = headCellPosition.y - headCheckTileRange; y < headCellPosition.y + headCheckTileRange; y ++)
				{
					for (int i = 0; i < headLineSegments.Length; i ++)
					{
						LineSegment2D headLineSegment = headLineSegments[i];
						if (headLineSegment.DoIIntersectWith(World.instance.CellToRect(new Vector2Int(x, y))))
							cellPositions.Add(new Vector3Int(x, y, 0));
					}
				}
			}
			if (cellPositions.Count > 0)
			{
				World.instance.tilemap.SetTiles(cellPositions.ToArray(), new TileBase[cellPositions.Count]);
				// for (int i = 0; i < World.tilemaps.Count; i ++)
				// {
					// Tilemap tilemap = World.tilemaps[i];
					World.Island[] islands = World.instance.GetIslands(World.instance.tilemap, false);
					for (int i2 = 0; i2 < islands.Length; i2 ++)
					{
						World.Island island = islands[i2];
						if (!World.tilemaps.Contains(island.tilemap))
							island.Split();
					}
				// }
			}
		}

		public virtual bool IsFalling ()
		{
			if (headCollider.IsTouchingLayers(LayerMask.GetMask("Wall")))
				return false;
			foreach (Vector2 localVertex in localVerticies)
			{
				if (Physics2DExtensions.LinecastWithWidth((Vector2) trs.position + localVertex, (Vector2) trs.position + localVertex + (Vector2.down * (width / 2 + crashCheckDistance)), crashCheckDistance, LayerMask.GetMask("Wall")).collider != null)
					return false;
			}
			return true;
		}

		public virtual void OnDestroy ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}