using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class GroundBreakingController : MonoBehaviour
{
	public TerrainData data;
	public Tilemap[] breakableTilemaps;

	Tilemap tilemap;


	public void onMessage(EntityMessage msg)
	{
		if (msg.name != "hit")
			return;

		AttackController contact = msg.data as AttackController;

		List<Vector3Int> triggeredCoords = TilemapHelper.getTriggeredTiles(tilemap, contact.hitboxBounds);
		if (triggeredCoords.Count == 0)
			return;

		List<TilemapHelper.TileData> tiles = TilemapHelper.getAllTiles(
			breakableTilemaps.Concat(new Tilemap[] { tilemap }), triggeredCoords);

		foreach (Vector3Int triggeredCoord in triggeredCoords)
			tilemap.SetTile(triggeredCoord, null);

		StartCoroutine(breakTiles(tiles));
	}


	private void Awake()
	{
		tilemap = transform.GetComponent<Tilemap>();
	}

	private bool canRespawn(List<TilemapHelper.TileData> tiles)
	{
		bool canRespawn = !TilemapHelper.isOverlappingLayers(
			tiles.Where(t => t.parent == tilemap), data.groundBreaking.collidingLayers);

		return canRespawn;
	}
	private IEnumerator breakTiles(List<TilemapHelper.TileData> tiles)
	{
		var instance = Effects.Tiles.instantiate(
			tiles.Where(tile => tile.parent != tilemap), transform.parent.GetComponent<Grid>());

		foreach (TilemapHelper.TileData tile in tiles)
			tile.parent.SetTile(tile.coord, null);

		StartCoroutine(Effects.instance.fade.run(instance.gameObject, instance.tilemaps, move: false));

		yield return new WaitForSeconds(data.groundBreaking.respawnTime - Effects.instance.fade.time);

		yield return Effects.instance.fade.run(instance.gameObject, instance.tilemaps, move: false, 
			stop: () => !canRespawn(tiles), revert: true);

		Destroy(instance.gameObject);

		foreach (TilemapHelper.TileData tile in tiles)
			tile.parent.SetTile(tile.coord, tile.tile);
	}

}
