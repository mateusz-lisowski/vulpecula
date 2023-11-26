using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Grid))]
public class GridPreprocessor : MonoBehaviour
{
	public GridPreprocessorData data;

	private Tilemap[] tilemaps;

	private Tilemap slopesTilemap;
	private Tilemap bounceOnBreakTilemap;


	private void Awake()
	{
		tilemaps = transform.GetComponentsInChildren<Tilemap>();
	}

	private void Start()
	{
		initializeRuntimeTilemaps();

		foreach (Tilemap tilemap in tilemaps)
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


	private Tilemap createTilemap(string name, Color color)
	{
		GameObject tilemapObject = new GameObject(name);

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
	}
	private void createBounceOnBreakTilemap(GameObject parent)
	{
		bounceOnBreakTilemap = createTilemap("BounceOnBreak", data.bounceOnBreak.color);
		bounceOnBreakTilemap.transform.parent = parent.transform;
		bounceOnBreakTilemap.gameObject.layer = LayerMask.NameToLayer("Ground-Breakable");
		bounceOnBreakTilemap.tag = "BreakBounce";

		GroundBreakingController controller = bounceOnBreakTilemap.AddComponent<GroundBreakingController>();
		controller.data = data.terrainData;
		controller.breakableTilemaps = data.bounceOnBreak.breakableTilemaps;
	}
	private void initializeRuntimeTilemaps()
	{
		GameObject runtimeTilemapsParent = new GameObject("Runtime");
		runtimeTilemapsParent.transform.parent = transform;

		createSlopeTilemap(runtimeTilemapsParent);
		createBounceOnBreakTilemap(runtimeTilemapsParent);
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

				Instantiate(mapping.prefab, position, rotation, GameManager.instance.runtimeEnemiesGroup);
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

				Instantiate(mapping.prefab, position, Quaternion.identity, 
					GameManager.instance.runtimeCollectiblesGroup);
			}
	}

}
