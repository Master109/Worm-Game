﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace Worms
{
	public class Player : MonoBehaviour, IUpdatable
	{
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
		public bool PauseWhileUnfocused
		{
			get
			{
				return false;
			}
		}

		public virtual void Start ()
		{
			lineRenderer.startWidth = width;
			lineRenderer.endWidth = width;
			edgeCollider.edgeRadius = width / 2;
			lineRenderer.positionCount = vertexCount;
			localVerticies = new Vector2[vertexCount];
			localVertexDistance = 1f / (vertexCount - 1) * length;
			Vector2 vertex;
			for (int i = 0; i < vertexCount; i ++)
			{
				vertex = -trs.up * localVertexDistance * i;
				localVerticies[i] = vertex;
				lineRenderer.SetPosition(i, vertex);
			}
			edgeCollider.points = localVerticies;
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public virtual void DoUpdate ()
		{
			HandleMovement ();
			for (int i = 0; i < vertexCount; i ++)
				lineRenderer.SetPosition(i, localVerticies[i]);
			edgeCollider.points = localVerticies;
		}

		public virtual void HandleMovement ()
		{
			Move (InputManager.GetAxis2D("Move Horizontal", "Move Vertical"));
		}

		public virtual void Move (Vector2 move)
		{
			float moveAmount = moveSpeed * Time.deltaTime;
			if (Physics2D.Raycast((Vector2) trs.position + localVerticies[0] + (move.normalized * (width / 2 + crashCheckDistance)), move, moveAmount).collider != null || IsFalling())
				return;
			Vector2 headLocalVertex = localVerticies[0];
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
			previousHeadLocalVertex = headLocalVertex;
		}

		public virtual bool IsFalling ()
		{
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