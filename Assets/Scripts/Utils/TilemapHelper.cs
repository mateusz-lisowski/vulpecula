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

	private static List<Vector3Int> findTriggeredWithinBounds(Tilemap tilemap, Bounds bounds)
	{
		List<Vector3Int> triggeredCoords = new List<Vector3Int>();

		Vector3Int min = tilemap.WorldToCell(bounds.min);
		Vector3Int max = tilemap.WorldToCell(bounds.max);

		for (int i = min.x; i <= max.x; i++)
			for (int j = min.y; j < max.y; j++)
			{
				Vector3Int coord = new Vector3Int(i, j, min.z);

				if (tilemap.GetTile(coord) != null)
					triggeredCoords.Add(coord);
			}

		//if (triggeredCoords.Count == 0)
		//	Debug.LogWarning("No tiles to trigger found within bounds: "
		//		+ bounds.min + " to " + bounds.max);

		return triggeredCoords;
	}
	public static List<Vector3Int> getTriggeredTiles(Tilemap tilemap, Bounds triggerBounds)
	{
		List<Vector3Int> triggeredCoords = findTriggeredWithinBounds(tilemap, triggerBounds);

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

}
