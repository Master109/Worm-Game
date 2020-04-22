﻿#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Extensions;
using UnityEditor;
using Worms;
using System.IO;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class MakeSpriteFromCollider2D : MonoBehaviour
{
	public float colliderChecksPerDistUnit;
	public SpriteRenderer spriteRenderer;
	public new Collider2D collider;
	public Behaviour behaviour;
	public Sprite sprite;
	public bool checkDiagonals;
	public float outlineRadius;
	public int textureBorder;
	BoxCollider2D boxCollider;
	float boxColliderEdgeRadius;
	Bounds colliderBounds;
	Vector2Int colliderChecks;
	public string spriteAssetPath;
	public string textureAssetPath;
	public bool makeAssetPathsUnique;
	public bool update;

	public virtual void Start ()
	{
		if (!Application.isPlaying)
		{
			if (collider == null)
				collider = GetComponent<Collider2D>();
			return;
		}
	}

	public virtual void Update ()
	{
		if (!update || Application.isPlaying)
			return;
		update = false;
		Do ();
	}

	public virtual void Do ()
	{
		Vector2Int[] outlinePositions = new Vector2Int[0];
		bool foundCollider;
		bool foundNoCollider;
		colliderBounds = collider.bounds;
		boxCollider = collider as BoxCollider2D;
		if (boxCollider != null)
		{
			boxColliderEdgeRadius = boxCollider.edgeRadius;
			colliderBounds.Expand(Vector2.one * boxColliderEdgeRadius * 2);
			boxCollider.edgeRadius = 0;
		}
		colliderChecks = (colliderBounds.size * colliderChecksPerDistUnit).ToVec2Int();
		Color[] colors = new Color[(colliderChecks.x + textureBorder * 2) * (colliderChecks.y + textureBorder * 2)];
		for (int x = 0; x < colliderChecks.x + textureBorder * 2; x ++)
		{
			for (int y = 0; y < colliderChecks.y + textureBorder * 2; y ++)
			{
				if (behaviour == Behaviour.FillInside)
				{
					if (CheckForCollider(new Vector2Int(x, y)))
						colors[x + y * (colliderChecks.x + textureBorder * 2)] = Color.white;
				}
				else if (behaviour == Behaviour.FillOutside)
				{
					if (!CheckForCollider(new Vector2Int(x, y)))
						colors[x + y * (colliderChecks.x + textureBorder * 2)] = Color.white;
				}
				else
				{
					foundCollider = CheckForCollider(new Vector2Int(x, y)) || CheckForCollider(new Vector2Int(x + 1, y)) || CheckForCollider(new Vector2Int(x, y + 1)) || CheckForCollider(new Vector2Int(x - 1, y)) || CheckForCollider(new Vector2Int(x, y - 1));
					foundNoCollider = !CheckForCollider(new Vector2Int(x, y)) || !CheckForCollider(new Vector2Int(x + 1, y)) || !CheckForCollider(new Vector2Int(x, y + 1)) || !CheckForCollider(new Vector2Int(x - 1, y)) || !CheckForCollider(new Vector2Int(x, y - 1));
					if (checkDiagonals)
					{
						foundCollider |= CheckForCollider(new Vector2Int(x + 1, y + 1)) || CheckForCollider(new Vector2Int(x - 1, y + 1)) || CheckForCollider(new Vector2Int(x - 1, y + 1)) || CheckForCollider(new Vector2Int(x - 1, y - 1)); 
						foundNoCollider |= !CheckForCollider(new Vector2Int(x + 1, y + 1)) || !CheckForCollider(new Vector2Int(x - 1, y + 1)) || !CheckForCollider(new Vector2Int(x - 1, y + 1)) || !CheckForCollider(new Vector2Int(x - 1, y - 1));
					}
					if (foundCollider && foundNoCollider)
					{
						colors[x + y * (colliderChecks.x + textureBorder * 2)] = Color.white;
						outlinePositions = outlinePositions.Add(new Vector2Int(x, y));
					}
				}
			}
		}
		if (behaviour == Behaviour.Outline)
		{
			for (int x = 0; x < colliderChecks.x + textureBorder * 2; x ++)
			{
				for (int y = 0; y < colliderChecks.y + textureBorder * 2; y ++)
				{
					foreach (Vector2Int outlinePosition in outlinePositions)
					{
						if (Vector2Int.Distance(new Vector2Int(x, y), outlinePosition) <= outlineRadius)
						{
							colors[x + y * (colliderChecks.x + textureBorder * 2)] = Color.white;
							break;
						}
					}
				}
			}
		}
		Texture2D texture = new Texture2D(colliderChecks.x + textureBorder * 2, colliderChecks.y + textureBorder * 2);
		texture.SetPixels(colors);
		texture.Apply();
		if (makeAssetPathsUnique)
		{
			string newAssetPath = textureAssetPath;
			while (File.Exists(newAssetPath))
				newAssetPath = newAssetPath.Replace(".asset", "1.asset");
			textureAssetPath = newAssetPath;
			AssetDatabase.CreateAsset(texture, newAssetPath);
		}
		else
			AssetDatabase.CreateAsset(texture, textureAssetPath);
		AssetDatabase.Refresh();
		sprite = Sprite.Create(texture, Rect.MinMaxRect(0, 0, colliderChecks.x + textureBorder * 2, colliderChecks.y + textureBorder * 2), Vector2.one / 2, colliderChecksPerDistUnit);
		if (makeAssetPathsUnique)
		{
			string newAssetPath = spriteAssetPath;
			while (File.Exists(newAssetPath))
				newAssetPath = newAssetPath.Replace(".asset", "1.asset");
			spriteAssetPath = newAssetPath;
			AssetDatabase.CreateAsset(sprite, newAssetPath);
		}
		else
			AssetDatabase.CreateAsset(sprite, spriteAssetPath);
		AssetDatabase.Refresh();
		if (boxCollider != null)
			boxCollider.edgeRadius = boxColliderEdgeRadius;
		if (spriteRenderer != null)
		{
			spriteRenderer.sprite = sprite;
			Transform trs = spriteRenderer.GetComponent<Transform>();
			trs.SetWorldScale (Vector3.one);
			trs.position = collider.GetCenter();
		}
	}

	public virtual bool CheckForCollider (Vector2Int checkPosition)
	{
		checkPosition -= new Vector2Int(textureBorder, textureBorder);
		Vector2 checkPoint = colliderBounds.min + (colliderBounds.size.Multiply(new Vector2((float) checkPosition.x / colliderChecks.x, (float) checkPosition.y / colliderChecks.y)));
		if (boxCollider != null)
			return Vector2.Distance(collider.ClosestPoint(checkPoint), checkPoint) <= boxColliderEdgeRadius;
		else
			return collider.OverlapPoint(checkPoint);
	}

	public enum Behaviour
	{
		FillInside,
		FillOutside,
		Outline
	}
}
#endif