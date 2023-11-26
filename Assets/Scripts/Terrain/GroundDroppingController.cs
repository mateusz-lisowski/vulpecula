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


	public void triggerDrop(Bounds bounds)
    {
		List<Vector3Int> triggeredCoords = TilemapHelper.getTriggeredTiles(tilemap, bounds);
		if (triggeredCoords.Count == 0)
			return;

		List<TilemapHelper.TileData> tiles = TilemapHelper.getAllTiles(
			droppableTilemaps.Concat(new Tilemap[] { tilemap }), triggeredCoords);

		foreach (Vector3Int triggeredCoord in triggeredCoords)
			tilemap.SetTile(triggeredCoord, null);

		StartCoroutine(dropTiles(tiles));
	}

	private void Awake()
	{
		tilemap = transform.GetComponent<Tilemap>();
	}


	private bool canRespawn(List<TilemapHelper.TileData> tiles)
	{
		bool canRespawn = !TilemapHelper.isOverlappingLayers(
			tiles.Where(t => t.parent == tilemap), data.groundDropping.collidingLayers);

		return canRespawn;
	}
	private IEnumerator dropTiles(List<TilemapHelper.TileData> tiles)
	{
		var instance = Effects.Tiles.instantiate(
			tiles.Where(tile => tile.parent != tilemap), tilemap.layoutGrid);

		yield return new WaitForSeconds(data.groundDropping.shakeTime);

		foreach (TilemapHelper.TileData tile in tiles)
			tile.parent.SetTile(tile.coord, null);

		StartCoroutine(Effects.instance.fade.run(instance.gameObject, instance.tilemaps));

		yield return new WaitForSeconds(data.groundDropping.respawnTime - Effects.instance.fade.time);

		yield return Effects.instance.fade.run(instance.gameObject, instance.tilemaps,
			stop: () => !canRespawn(tiles), revert: true);

		Destroy(instance.gameObject);

		foreach (TilemapHelper.TileData tile in tiles)
			tile.parent.SetTile(tile.coord, tile.tile);
	}

}
