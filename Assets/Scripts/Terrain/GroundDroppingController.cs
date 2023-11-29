using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundDroppingController : MonoBehaviour
{
	public TerrainData data;
	public Tilemap[] droppableTilemaps;

	private Tilemap tilemap;
	private GridPreprocessor preprocessor;


	public void triggerDrop(Bounds bounds)
    {
		List<Vector3Int> triggeredCoords = TilemapHelper.findTriggeredWithinBounds(tilemap, bounds);
		if (triggeredCoords.Count == 0)
			return;

		var region = preprocessor.groundDroppingRegions.Find(r => r.contains(triggeredCoords));

		var tiles = TilemapHelper.getAllTiles(new Tilemap[] { tilemap }, region.coords);

		StartCoroutine(dropTiles(region, tiles));
	}


	private void Awake()
	{
		tilemap = transform.GetComponent<Tilemap>();
		preprocessor = tilemap.layoutGrid.transform.GetComponent<GridPreprocessor>();
	}

	private bool canRespawn(List<TilemapHelper.TileData> tiles)
	{
		bool canRespawn = !TilemapHelper.isOverlappingLayers(
			tiles, data.groundDropping.collidingLayers);

		return canRespawn;
	}
	private IEnumerator dropTiles(TilemapHelper.Region region, List<TilemapHelper.TileData> tiles)
	{
		region.gameObject.SetActive(true);

		foreach (var tile in tiles)
			tile.parent.SetTile(tile.coord, null);

		yield return new WaitForSeconds(data.groundDropping.shakeTime);

		foreach (var tile in region.tiles)
			tile.parent.SetTile(tile.coord, null);

		StartCoroutine(Effects.instance.fade.run(region.gameObject, region.tilemaps));

		yield return new WaitForSeconds(data.groundDropping.respawnTime - Effects.instance.fade.time);

		yield return Effects.instance.fade.run(region.gameObject, region.tilemaps,
			stop: () => !canRespawn(tiles), revert: true);

		region.gameObject.SetActive(false);

		foreach (var tile in region.tiles.Concat(tiles))
			TilemapHelper.setTile(tile.parent, tile);
	}

}
