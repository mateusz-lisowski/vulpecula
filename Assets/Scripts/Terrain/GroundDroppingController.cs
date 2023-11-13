using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundDroppingController : MonoBehaviour
{
	public TerrainData data;
	public Tilemap[] droppableTilemaps;

	Tilemap tilemap;
	TilemapCollider2D tilemapCollider;


	private List<Vector3Int> findTriggeredWithinBounds(Bounds bounds)
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

		if (triggeredCoords.Count == 0)
			Debug.LogWarning("No tiles to trigger found within bounds: " 
				+ bounds.min + " to " + bounds.max);

		return triggeredCoords;
	}
	private List<Vector3Int> getTriggeredCoords(Bounds bounds)
	{
		List<Vector3Int> triggeredCoords = findTriggeredWithinBounds(bounds);

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
    
	private List<Effects.Tiles.Data> GetRemovedTiles(List<Vector3Int> triggeredCoords)
	{
		List<Effects.Tiles.Data> removedTiles = new List<Effects.Tiles.Data>();

		foreach (Tilemap droppedTilemap in droppableTilemaps.Concat(new Tilemap[] { tilemap }))
			foreach (Vector3Int triggeredCoord in triggeredCoords)
			{
				TileBase droppedTile = droppedTilemap.GetTile(triggeredCoord);

				if (droppedTile == null)
					continue;

				Effects.Tiles.Data removedTileData = new Effects.Tiles.Data();
				removedTileData.parent = droppedTilemap;
				removedTileData.coord = triggeredCoord;
				removedTileData.tile = droppedTile;

				removedTiles.Add(removedTileData);
			}

		return removedTiles;
	}
	public void triggerDrop(Bounds bounds)
    {
		List<Vector3Int> triggeredCoords = getTriggeredCoords(bounds);
		if (triggeredCoords.Count == 0)
			return;

		List<Effects.Tiles.Data> removedTiles = GetRemovedTiles(triggeredCoords);

		foreach (Vector3Int triggeredCoord in triggeredCoords)
			tilemap.SetTile(triggeredCoord, null);

		StartCoroutine(dropTiles(removedTiles));
	}

	private void Awake()
	{
		tilemap = transform.GetComponent<Tilemap>();
		tilemapCollider = transform.GetComponent<TilemapCollider2D>();
	}


	private bool canRespawn(List<Effects.Tiles.Data> tiles)
	{
		bool canRespawn = true;

		foreach (Effects.Tiles.Data removedTile in tiles)
			if (removedTile.parent == tilemap)
			{
				Vector2 center = tilemap.CellToWorld(removedTile.coord);
				Vector2 halfSize = tilemap.cellSize / 2;

				if (Physics2D.OverlapArea(
					center - halfSize, center + halfSize, data.groundDropping.collidingLayers))
				{
					canRespawn = false;
					break;
				}
			}


		if (canRespawn)
		{
			foreach (Effects.Tiles.Data removedTile in tiles)
				removedTile.parent.SetTile(removedTile.coord, removedTile.tile);
		}

		return canRespawn;
	}
	private IEnumerator dropTiles(List<Effects.Tiles.Data> tiles)
	{
		var instance = Effects.Tiles.instantiate(
			tiles.Where(tile => tile.parent != tilemap), transform.parent.GetComponent<Grid>());

		yield return new WaitForSeconds(data.groundDropping.shakeTime);

		foreach (Effects.Tiles.Data removedTile in tiles)
		{
			removedTile.parent.SetTile(removedTile.coord, null);
		}

		StartCoroutine(Effects.instance.fade.run(instance.gameObject, instance.tilemaps));

		yield return new WaitForSeconds(data.groundDropping.respawnTime - Effects.instance.fade.time);

		yield return Effects.instance.fade.run(instance.gameObject, instance.tilemaps, revert: true);

		while (!canRespawn(tiles))
			yield return null;

		Destroy(instance.gameObject);

		foreach (Effects.Tiles.Data removedTile in tiles)
			removedTile.parent.SetTile(removedTile.coord, removedTile.tile);
	}

}
