using System;
using System.Collections.Generic;
using System.Linq;
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
	private Dictionary<Vector3Int, List<GameObject>> invalidationCoords;
	private HashSet<GameObject> invalidObjects;
	private HashSet<GameObject> invalidableObjects;

	private Transform autogenGroup;
	private Tilemap slopesTilemap;
	private Tilemap bounceOnBreakTilemap;
	private Transform groundDroppingGroup;
	private Transform groundBreakingGroup;
	private Transform groundShadowCastingGroup;
	private Transform enemiesGroup;
	private Transform collectiblesGroup;

	public List<TilemapHelper.Region> groundDroppingRegions;
	public List<TilemapHelper.Region> groundBreakingRegions;
	public List<GroundShadowCaster> groundShadowCasters;


	private void Awake()
	{
		if (!Application.isPlaying)
			Tilemap.tilemapTileChanged += onTilemapChange;

		regenerate();
	}
	private void OnDestroy()
	{
		Tilemap.tilemapTileChanged -= onTilemapChange;
	}
	
	public void regenerate()
	{
		if (invalidableObjects != null)
			foreach (GameObject child in invalidableObjects)
				invalidObjects.Add(child);

		generate();
	}
	public void clear()
	{
		if (autogenGroup != null)
		{
			QolUtility.DestroyExecutableInEditMode(autogenGroup.gameObject);
			autogenGroup = null;
			groundDroppingRegions = null;
			groundBreakingRegions = null;
		}
	}
	
	private void generate()
	{
		invalidationCoords = new Dictionary<Vector3Int, List<GameObject>>();
		if (invalidObjects == null)
			invalidObjects = new HashSet<GameObject>();
		if (invalidableObjects == null)
			invalidableObjects = new HashSet<GameObject>();

		QolUtility.createIfNotExist(out autogenGroup, transform, "Autogen");

		autogenGroup.gameObject.SetActive(false);
		tilemaps = transform.GetComponentsInChildren<Tilemap>();
		autogenGroup.gameObject.SetActive(true);

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

		createRegions(ref groundDroppingRegions, groundDroppingGroup, transform.GetComponentsInChildren<GroundDroppingController>());
		createRegions(ref groundBreakingRegions, groundBreakingGroup, transform.GetComponentsInChildren<GroundBreakingController>());
		createShadowCasters();

		foreach (var invalidObject in invalidObjects)
			if (invalidObject != null)
				QolUtility.DestroyExecutableInEditMode(invalidObject);
		invalidObjects.Clear();
	}
	private void onTilemapChange(Tilemap tilemap, Tilemap.SyncTile[] tiles)
	{
		if (!tilemaps.Contains(tilemap))
			return;

		//foreach (var tile in tiles)
		//{
		//	Debug.Log("Change at: " + tile.position);
		//	if (invalidationCoords.ContainsKey(tile.position))
		//		foreach (var invalidObject in invalidationCoords[tile.position])
		//			invalidObjects.Add(invalidObject);
		//}
		//
		regenerate();
	}

	private bool createTilemap(out Tilemap target, Transform parent, string name, Color color)
	{
		Transform tilemapTransform;
		if (!QolUtility.createIfNotExist(out tilemapTransform, parent, name))
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

		QolUtility.createIfNotExist(out groundDroppingGroup, autogenGroup, "Ground Dropping");
		QolUtility.createIfNotExist(out groundBreakingGroup, autogenGroup, "Ground Breaking");
		QolUtility.createIfNotExist(out groundShadowCastingGroup, autogenGroup, "Ground Shadow Casting");
		QolUtility.createIfNotExist(out enemiesGroup, autogenGroup, "Enemies");
		QolUtility.createIfNotExist(out collectiblesGroup, autogenGroup, "Collectibles");
	}

	private void addInvalidation(GameObject gameObject, Vector3Int coord)
	{
		invalidableObjects.Add(gameObject);
		invalidObjects.Remove(gameObject);

		List<GameObject> newInvalidObjects;

		if (!invalidationCoords.ContainsKey(coord))
		{
			newInvalidObjects = new List<GameObject>();
			invalidationCoords.Add(coord, newInvalidObjects);
		}
		else
			newInvalidObjects = invalidationCoords[coord];

		if (newInvalidObjects == null)
			newInvalidObjects = new List<GameObject>();

		newInvalidObjects.Add(gameObject);
	}
	private void addInvalidation(GameObject gameObject, TilemapHelper.Region region)
	{
		var coords = TilemapHelper.extendToAdjacent(region.coords);

		foreach (var coord in coords)
			addInvalidation(gameObject, coord);
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
				bool alreadyExists = !QolUtility.createIfNotExist(out newEntity, parent,
					RuntimeDataManager.getUniqueName(mapping.prefab, tilemap, coord), mapping.prefab);

				addInvalidation(newEntity.gameObject, coord);

				if (alreadyExists)
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
	private void createRegions(ref List<TilemapHelper.Region> regions, Transform parent, GroundController[] groundControllers)
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
				if (!QolUtility.createIfNotExist(out regionTransform, parent, 
					RuntimeDataManager.getUniqueName(triggerTilemap.gameObject, TilemapHelper.hash(triggeredCoords))))
					if (region != null)
						region = regions.Find(r => r.gameObject == regionTransform.gameObject);

				if (region == null)
				{ 
					List<TilemapHelper.TileData> tiles = TilemapHelper.getAllTiles(tilemaps, triggeredCoords);

					region = new TilemapHelper.Region(regionTransform.gameObject, tiles, triggeredCoords);
					regionTransform.gameObject.SetActive(false);
				}

				addInvalidation(region.gameObject, region);

				ignore.AddRange(region.coords);
				newRegions.Add(region);
			}
		}

		regions = newRegions;
	}
	
	public void createShadowCasters()
	{
		var newCasters = new List<GroundShadowCaster>();

		foreach (var groundShadowSource in transform.GetComponentsInChildren<GroundShadowSource>())
		{
			if (!groundShadowSource.shouldReinitialize)
				continue;

			var collider = groundShadowSource.lightCollider;

			for (int i = 0; i < groundShadowSource.lightCollider.pathCount; i++)
			{
				List<Vector2> points = new List<Vector2>();
				collider.GetPath(i, points);

				Transform newShadowCaster;
				if (!QolUtility.createIfNotExist(out newShadowCaster, groundShadowCastingGroup,
					RuntimeDataManager.getUniqueName(groundShadowSource.gameObject, TilemapHelper.hash(points))))
					continue;

				GroundShadowCaster caster = newShadowCaster.AddComponent<GroundShadowCaster>();

				caster.setShape(groundShadowSource, points);
			}
		}

		groundShadowCasters = newCasters;
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
