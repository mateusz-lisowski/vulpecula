using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Grid))]
public class GridPreprocessor : MonoBehaviour
{
	public GridPreprocessorData data;

	private Tilemap[] tilemaps;

	private Tilemap slopesTilemap;
	private Tilemap bounceOnBreakTilemap;
	private GameObject groundDroppingGroup;
	private GameObject groundBreakingGroup;

	public List<TilemapHelper.Region> groundDroppingRegions { get; private set; }
	public List<TilemapHelper.Region> groundBreakingRegions { get; private set; }


	private void Awake()
	{
		tilemaps = transform.GetComponentsInChildren<Tilemap>();
	}

	private void Start()
	{
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

				tryAddSlope(tile, coord);
				tryAddBreakBounce(tile, coord);

				tryAddEnemy(tile, tilemap, coord);
				tryAddCollectible(tile, tilemap, coord);
			}
		}

		createGroundDroppableRegions();
		createGroundBreakableRegions();
	}


	private Tilemap createTilemap(string name, Color color)
	{
		GameObject tilemapObject = new GameObject(name);
		tilemapObject.SetActive(false);

		Tilemap tilemap = tilemapObject.AddComponent<Tilemap>();
		tilemap.color = color;

		TilemapRenderer tilemapRenderer = tilemapObject.AddComponent<TilemapRenderer>();
		tilemapRenderer.sortingLayerName = data.sortingLayer;
		tilemapRenderer.sortingOrder = data.sortingOrder;

		TilemapCollider2D tilemapCollider = tilemapObject.AddComponent<TilemapCollider2D>();
		tilemapCollider.isTrigger = true;

		return tilemap;
	}
	private void createSlopeTilemap(GameObject parent)
	{
		slopesTilemap = createTilemap("Slope", data.slopes.color);
		slopesTilemap.transform.parent = parent.transform;
		slopesTilemap.gameObject.layer = LayerMask.NameToLayer("Slope");

		slopesTilemap.gameObject.SetActive(true);
	}
	private void createBounceOnBreakTilemap(GameObject parent)
	{
		bounceOnBreakTilemap = createTilemap("Bounce On Break", data.bounceOnBreak.color);
		bounceOnBreakTilemap.transform.parent = parent.transform;
		bounceOnBreakTilemap.gameObject.layer = LayerMask.NameToLayer("Ground-Breakable");
		bounceOnBreakTilemap.tag = "BreakBounce";

		GroundBreakingController controller = bounceOnBreakTilemap.AddComponent<GroundBreakingController>();
		controller.data = data.terrainData;
		controller.breakableTilemaps = data.bounceOnBreak.breakableTilemaps;

		bounceOnBreakTilemap.gameObject.SetActive(true);
	}
	
	private void initializeRuntimeTilemaps()
	{
		GameObject runtimeTilemapsParent = new GameObject("Runtime");
		runtimeTilemapsParent.transform.parent = transform;

		createSlopeTilemap(runtimeTilemapsParent);
		createBounceOnBreakTilemap(runtimeTilemapsParent);

		groundDroppingGroup = new GameObject("Ground Dropping");
		groundDroppingGroup.transform.parent = runtimeTilemapsParent.transform;

		groundBreakingGroup = new GameObject("Ground Breaking");
		groundBreakingGroup.transform.parent = runtimeTilemapsParent.transform;
	}


	private void tryAddSlope(Tile tile, Vector3Int coord)
	{
		foreach (var tileData in data.slopes.tiles)
			if (tile == tileData.tile)
			{
				foreach (Vector3Int offset in tileData.offsetBounds.allPositionsWithin)
					slopesTilemap.SetTile(coord + offset, data.areaTile);
			}
	}
	private void tryAddBreakBounce(Tile tile, Vector3Int coord)
	{
		foreach (var tileData in data.bounceOnBreak.tiles)
			if (tile == tileData.tile)
			{
				foreach (Vector3Int offset in tileData.offsetBounds.allPositionsWithin)
					bounceOnBreakTilemap.SetTile(coord + offset, data.areaTile);
			}
	}
	
	private void tryAddEnemy(Tile tile, Tilemap tilemap, Vector3Int coord)
	{
		foreach (var mapping in data.enemies.mapping)
			if (tile == mapping.tile)
			{
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

				var newEnemy = Instantiate(mapping.prefab, position, rotation,
					GameManager.instance.runtimeGroup[GameManager.RuntimeGroup.Enemies]);

				RuntimeDataManager.setUniqueName(ref newEnemy, mapping.prefab, tilemap, coord);
			}
	}
	private void tryAddCollectible(Tile tile, Tilemap tilemap, Vector3Int coord)
	{
		foreach (var mapping in data.collectibles.mapping)
			if (tile == mapping.tile)
			{
				SpriteRenderer renderer = mapping.prefab.transform.Find("Sprite").GetComponent<SpriteRenderer>();

				Vector2 pivotOffset = (tile.sprite.pivot - tile.sprite.rect.size / 2) / tile.sprite.pixelsPerUnit;
				Vector3 offset = renderer.transform.position;

				Vector3 position = tilemap.CellToWorld(coord) + new Vector3(0.5f, 0.5f);
				position -= (Vector3)pivotOffset;
				position -= offset;
				position += tilemap.GetTransformMatrix(coord).GetT();

				var newCollectible = Instantiate(mapping.prefab, position, Quaternion.identity,
					GameManager.instance.runtimeGroup[GameManager.RuntimeGroup.Collectibles]);

				RuntimeDataManager.setUniqueName(ref newCollectible, mapping.prefab, tilemap, coord);
			}
	}


	private TilemapHelper.Region createTilemapRegion(
		Tilemap triggerTilemap, Vector3Int triggerCoord, Tilemap[] tilemaps, Transform parent)
	{
		List<Vector3Int> triggeredCoords = TilemapHelper.getTriggeredTiles(
			triggerTilemap, new List<Vector3Int> { triggerCoord });

		List<TilemapHelper.TileData> tiles = TilemapHelper.getAllTiles(
			tilemaps, triggeredCoords);

		var region = new TilemapHelper.Region(tiles, triggeredCoords, parent);

		region.gameObject.SetActive(false);

		return region;
	}

	private void createGroundDroppableRegions()
	{
		groundDroppingRegions = new List<TilemapHelper.Region>();
		List<Vector3Int> ignore = new List<Vector3Int>();

		foreach (var groundDropping in transform.GetComponentsInChildren<GroundDroppingController>())
		{
			Tilemap triggerTilemap = groundDropping.GetComponent<Tilemap>();
			Tilemap[] tilemaps = groundDropping.droppableTilemaps;

			foreach (Vector3Int coord in triggerTilemap.cellBounds.allPositionsWithin)
			{
				if (ignore.Contains(coord))
					continue;

				TileBase tileBase = triggerTilemap.GetTile(coord);
				if (tileBase == null)
					continue;

				TilemapHelper.Region region = createTilemapRegion(
					triggerTilemap, coord, tilemaps, groundDroppingGroup.transform);

				ignore.AddRange(region.coords);
				groundDroppingRegions.Add(region);
			}
		}
	}
	private void createGroundBreakableRegions()
	{
		groundBreakingRegions = new List<TilemapHelper.Region>();
		List<Vector3Int> ignore = new List<Vector3Int>();

		foreach (var groundBreaking in transform.GetComponentsInChildren<GroundBreakingController>())
		{
			Tilemap triggerTilemap = groundBreaking.GetComponent<Tilemap>();
			Tilemap[] tilemaps = groundBreaking.breakableTilemaps;

			foreach (Vector3Int coord in triggerTilemap.cellBounds.allPositionsWithin)
			{
				if (ignore.Contains(coord))
					continue;

				TileBase tileBase = triggerTilemap.GetTile(coord);
				if (tileBase == null)
					continue;

				TilemapHelper.Region region = createTilemapRegion(
					triggerTilemap, coord, tilemaps, groundBreakingGroup.transform);

				ignore.AddRange(region.coords);
				groundBreakingRegions.Add(region);
			}
		}
	}

}
