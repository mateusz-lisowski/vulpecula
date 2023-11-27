using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapHelper
{
	public struct TileData
	{
		public Tilemap parent;
		public Vector3Int coord;
		public TileBase tile;
	}

	public static List<Vector3Int> findTriggeredWithinBounds(Tilemap tilemap, Bounds bounds)
	{
		List<Vector3Int> triggeredCoords = new List<Vector3Int>();

		Vector3Int min = tilemap.WorldToCell(bounds.min);
		Vector3Int max = tilemap.WorldToCell(bounds.max) + (Vector3Int)Vector2Int.one;

		for (int i = min.x; i <= max.x; i++)
			for (int j = min.y; j < max.y; j++)
			{
				Vector3Int coord = new Vector3Int(i, j, min.z);

				if (tilemap.GetTile(coord) != null)
					triggeredCoords.Add(coord);
			}

		return triggeredCoords;
	}
	public static List<Vector3Int> getTriggeredTiles(Tilemap tilemap, Bounds triggerBounds)
	{
		return getTriggeredTiles(tilemap, findTriggeredWithinBounds(tilemap, triggerBounds));
	}
	
	public static List<Vector3Int> getTriggeredTiles(Tilemap tilemap, List<Vector3Int> triggeredCoords)
	{
		for (int i = 0; i < triggeredCoords.Count; i++)
		{
			Vector3Int triggeredCoord = triggeredCoords[i];

			foreach (Vector3Int adjacentCoord in new Vector3Int[]{
				triggeredCoord + Vector3Int.up,
				triggeredCoord + Vector3Int.down,
				triggeredCoord + Vector3Int.left,
				triggeredCoord + Vector3Int.right,
			})
			{
				if (tilemap.GetTile(adjacentCoord) == null)
					continue;

				if (triggeredCoords.Contains(adjacentCoord))
					continue;

				triggeredCoords.Add(adjacentCoord);
			}
		}

		return triggeredCoords;
	}

	public static List<TileData> getAllTiles(IEnumerable<Tilemap> tilemaps, List<Vector3Int> triggeredCoords)
	{
		List<TileData> tiles = new List<TileData>();

		foreach (Tilemap tilemap in tilemaps)
			foreach (Vector3Int triggeredCoord in triggeredCoords)
			{
				TileBase droppedTile = tilemap.GetTile(triggeredCoord);

				if (droppedTile == null)
					continue;

				TileData tileData = new TileData();
				tileData.parent = tilemap;
				tileData.coord = triggeredCoord;
				tileData.tile = droppedTile;

				tiles.Add(tileData);
			}

		return tiles;
	}

	public static bool isOverlappingLayers(IEnumerable<TileData> tiles, LayerMask layerMask)
	{
		foreach (TileData tile in tiles)
		{
			Vector2 min = tile.parent.CellToWorld(tile.coord);
			Vector2 max = min + (Vector2)tile.parent.cellSize;

			if (Physics2D.OverlapArea(min, max, layerMask))
				return true;
		}

		return false;
	}


	public struct Region
	{
		public GameObject gameObject;
		public Tilemap[] tilemaps;
		public TilemapRenderer[] tilemapRenderers;

		public List<TileData> tiles;
		public List<Vector3Int> coords;

		public Region(IEnumerable<TileData> tilesE, List<Vector3Int> triggeredCoords, Transform parent)
		{
			gameObject = new GameObject("TileRegion");
			gameObject.transform.parent = parent;
			tiles = new List<TileData>();
			coords = triggeredCoords;

			Dictionary<Tilemap, Tilemap> dict = new Dictionary<Tilemap, Tilemap>();

			foreach (TileData tile in tilesE)
			{
				if (!dict.ContainsKey(tile.parent))
					dict.Add(tile.parent, copyTilemap(gameObject, tile.parent));

				Tilemap tilemap = dict[tile.parent];

				tiles.Add(tile);
				tilemap.SetTile(tilemap.WorldToCell(tile.parent.CellToWorld(tile.coord)), tile.tile);
			}


			tilemaps = new Tilemap[gameObject.transform.childCount];
			tilemapRenderers = new TilemapRenderer[gameObject.transform.childCount];

			int i = 0;
			foreach (Transform child in gameObject.transform)
			{
				tilemaps[i] = child.GetComponent<Tilemap>();
				tilemapRenderers[i] = child.GetComponent<TilemapRenderer>();
				i++;
			}
		}

		public bool contains(List<Vector3Int> triggeredCoords)
		{
			if (triggeredCoords.Count == 0)
				return false;

			return coords.Contains(triggeredCoords[0]);
		}

		private static Tilemap copyTilemap(GameObject instance, Tilemap parent)
		{
			GameObject child = new GameObject("Tilemap");
			
			child.transform.parent = instance.transform;
			child.layer = parent.gameObject.layer;

			Tilemap newTilemap = child.AddComponent<Tilemap>();
			
			newTilemap.color = parent.color;

			TilemapRenderer newTilemapRenderer = child.AddComponent<TilemapRenderer>();
			TilemapRenderer oldTilemapRenderer = parent.gameObject.GetComponent<TilemapRenderer>();

			newTilemapRenderer.sortingLayerID = oldTilemapRenderer.sortingLayerID;
			newTilemapRenderer.sortingOrder = oldTilemapRenderer.sortingOrder;

			return newTilemap;
		}
	}
}
