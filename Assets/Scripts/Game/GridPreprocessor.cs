using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using BoundedTile = GridPreprocessorData.BoundedTile;
using TilePrefabMapping = GridPreprocessorData.TilePrefabMapping;

public abstract class GroundController : MonoBehaviour
{
	public abstract Tilemap[] tilemaps { get; }
}

[ExecuteInEditMode]
[RequireComponent(typeof(Grid))]
public class GridPreprocessor : MonoBehaviour
{
	public GridPreprocessorData data;

	private Tilemap[] tilemaps;

	private Transform autogenGroup;
	private Tilemap slopesTilemap;
	private Tilemap bounceOnBreakTilemap;
	private Transform groundDroppingGroup;
	private Transform groundBreakingGroup;
	private Transform enemiesGroup;
	private Transform collectiblesGroup;

	public List<TilemapHelper.Region> groundDroppingRegions;
	public List<TilemapHelper.Region> groundBreakingRegions;


	private void Awake()
	{
		regenerate();
	}
	public void regenerate()
	{
		createIfNotExist(out autogenGroup, transform, "Autogen");

		tilemaps = transform.GetComponentsInChildren<Tilemap>();

		initializeRuntimeTilemaps();

		foreach (Tilemap tilemap in tilemaps)
		{
			tilemap.CompressBounds();

			foreach (Vector3Int coord in tilemap.cellBounds.allPositionsWithin)
			{
				TileBase tileBase = tilemap.GetTile(coord);
				if (tileBase == null)
					continue;

				Tile tile = tileBase as Tile;

				tryAddTiles(tile, coord, slopesTilemap, data.slopes.tiles);
				tryAddTiles(tile, coord, bounceOnBreakTilemap, data.bounceOnBreak.tiles);
				
				tryAddEntity(tile, tilemap, coord, enemiesGroup, data.enemies.mapping);
				tryAddEntity(tile, tilemap, coord, collectiblesGroup, data.collectibles.mapping);
			}
		}

		createRegion(ref groundDroppingRegions, groundDroppingGroup, transform.GetComponentsInChildren<GroundDroppingController>());
		createRegion(ref groundBreakingRegions, groundBreakingGroup, transform.GetComponentsInChildren<GroundBreakingController>());
	}
	public void clear()
	{
		if (autogenGroup != null)
		{
			DestroyImmediate(autogenGroup.gameObject);
			autogenGroup = null;
			groundDroppingRegions = null;
			groundBreakingRegions = null;
		}
	}

	private static bool createIfNotExist(out Transform target, Transform parent, string name)
	{
		target = parent.Find(name);

		if (target == null)
		{
			target = new GameObject(name).transform;
			target.parent = parent;
			return true;
		}
		else
			return false;
	}
	private static bool createIfNotExist(out Transform target, Transform parent, string name, GameObject prefab)
	{
		target = parent.Find(name);

		if (target == null)
		{
			target = QolUtility.Instantiate(prefab, parent).transform;
			target.name = name;
			target.parent = parent;
			return true;
		}
		else
			return false;
	}

	private bool createTilemap(out Tilemap target, Transform parent, string name, Color color)
	{
		Transform tilemapTransform;
		if (!createIfNotExist(out tilemapTransform, parent, name))
		{
			target = tilemapTransform.GetComponent<Tilemap>();
			return false;
		}

		tilemapTransform.gameObject.SetActive(false);

		target = tilemapTransform.AddComponent<Tilemap>();
		target.color = color;

		TilemapRenderer tilemapRenderer = tilemapTransform.AddComponent<TilemapRenderer>();
		tilemapRenderer.sortingLayerName = data.sortingLayer;
		tilemapRenderer.sortingOrder = data.sortingOrder;

		TilemapCollider2D tilemapCollider = tilemapTransform.AddComponent<TilemapCollider2D>();
		tilemapCollider.isTrigger = true;

		return true;
	}
	private void createSlopeTilemap()
	{
		if (createTilemap(out slopesTilemap, autogenGroup, "Slope", data.slopes.color))
		{
			slopesTilemap.gameObject.layer = LayerMask.NameToLayer("Slope");

			slopesTilemap.gameObject.SetActive(true);
		}
	}
	private void createBounceOnBreakTilemap()
	{
		if (createTilemap(out bounceOnBreakTilemap, autogenGroup, "Bounce On Break", data.bounceOnBreak.color))
		{
			bounceOnBreakTilemap.gameObject.layer = LayerMask.NameToLayer("Ground-Breakable");
			bounceOnBreakTilemap.tag = "BreakBounce";

			GroundBreakingController controller = bounceOnBreakTilemap.AddComponent<GroundBreakingController>();
			controller.data = data.terrainData;
			controller.breakableTilemaps = data.bounceOnBreak.breakableTilemaps;

			bounceOnBreakTilemap.gameObject.SetActive(true);
		}
	}
	private void initializeRuntimeTilemaps()
	{
		createSlopeTilemap();
		createBounceOnBreakTilemap();

		createIfNotExist(out groundDroppingGroup, autogenGroup, "Ground Dropping");
		createIfNotExist(out groundBreakingGroup, autogenGroup, "Ground Breaking");
		createIfNotExist(out enemiesGroup, autogenGroup, "Enemies");
		createIfNotExist(out collectiblesGroup, autogenGroup, "Collectibles");
	}

	private void tryAddTiles(Tile tile, Vector3Int coord, Tilemap parent, BoundedTile[] tiles)
	{
		foreach (var tileData in tiles)
			if (tile == tileData.tile)
			{
				foreach (Vector3Int offset in tileData.offsetBounds.allPositionsWithin)
					parent.SetTile(coord + offset, data.areaTile);
			}
	}
	private void tryAddEntity(Tile tile, Tilemap tilemap, Vector3Int coord, Transform parent, TilePrefabMapping[] mappings)
	{
		foreach (var mapping in mappings)
			if (tile == mapping.tile)
			{
				Transform newEntity;
				if (!createIfNotExist(out newEntity, parent,
					RuntimeDataManager.getUniqueName(mapping.prefab, tilemap, coord), mapping.prefab))
					continue;

				SpriteRenderer renderer = mapping.prefab.transform.Find("Sprite").GetComponent<SpriteRenderer>();

				Matrix4x4 tileTransform = tilemap.GetTransformMatrix(coord);
				Quaternion tileRotation = tileTransform.GetR();

				Quaternion rotation = tileRotation;
				if (renderer.flipX)
					rotation *= Quaternion.Euler(0, 180, 0);
				Vector2 pivotOffset = (tile.sprite.pivot - tile.sprite.rect.size / 2) / tile.sprite.pixelsPerUnit;
				Vector3 offset = renderer.transform.position;

				Vector3 position = tilemap.CellToWorld(coord) + new Vector3(0.5f, 0.5f);
				position -= (Vector3)pivotOffset;
				position -= offset;
				position += tileTransform.GetT();

				newEntity.position = position;
				newEntity.rotation = rotation;
			}
	}
	private void createRegion(ref List<TilemapHelper.Region> regions, Transform parent, GroundController[] groundControllers)
	{
		var newRegions = new List<TilemapHelper.Region>();
		List<Vector3Int> ignore = new List<Vector3Int>();

		foreach (var groundController in groundControllers)
		{
			Tilemap triggerTilemap = groundController.GetComponent<Tilemap>();
			Tilemap[] tilemaps = groundController.tilemaps;

			foreach (Vector3Int coord in triggerTilemap.cellBounds.allPositionsWithin)
			{
				if (ignore.Contains(coord))
					continue;

				TileBase tileBase = triggerTilemap.GetTile(coord);
				if (tileBase == null)
					continue;

				List<Vector3Int> triggeredCoords = TilemapHelper.getTriggeredTiles(
					triggerTilemap, new List<Vector3Int> { coord });

				TilemapHelper.Region region = null;
				Transform regionTransform;
				if (!createIfNotExist(out regionTransform, parent, RuntimeDataManager.getUniqueName(triggerTilemap, triggeredCoords)))
					if (region != null)
						region = regions.Find(r => r.gameObject == regionTransform.gameObject);

				if (region == null)
				{ 
					List<TilemapHelper.TileData> tiles = TilemapHelper.getAllTiles(tilemaps, triggeredCoords);

					region = new TilemapHelper.Region(regionTransform.gameObject, tiles, triggeredCoords);
					regionTransform.gameObject.SetActive(false);
				}

				ignore.AddRange(region.coords);
				newRegions.Add(region);
			}
		}

		regions = newRegions;
	}
	
}

[CustomEditor(typeof(GridPreprocessor))]
public class GridPreprocessorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		GridPreprocessor gridPreprocessor = target as GridPreprocessor;

		if (GUILayout.Button("Regenerate"))
			gridPreprocessor.regenerate();
		if (GUILayout.Button("Clear"))
			gridPreprocessor.clear();

		base.OnInspectorGUI();
	}
}
