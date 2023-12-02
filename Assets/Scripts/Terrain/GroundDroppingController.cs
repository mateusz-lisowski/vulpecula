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

	private ParticleSystem particlesIdle;
	private ParticleSystem particlesShake;
	private ParticleSystem particlesBreak;


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

		var idleEffect = Instantiate(data.groundDropping.idleEffectPrefab, transform);
		idleEffect.name = data.groundDropping.idleEffectPrefab.name;

		var shakeEffect = Instantiate(data.groundDropping.shakeEffectPrefab, transform);
		shakeEffect.name = data.groundDropping.shakeEffectPrefab.name;

		var breakEffect = Instantiate(data.groundDropping.breakEffectPrefab, transform);
		breakEffect.name = data.groundDropping.breakEffectPrefab.name;

		particlesIdle = idleEffect.GetComponent<ParticleSystem>();
		particlesShake = shakeEffect.GetComponent<ParticleSystem>();
		particlesBreak = breakEffect.GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		float idleCount = Time.deltaTime * particlesIdle.emission.rateOverTime.constant;

		foreach (var region in preprocessor.groundDroppingRegions)
			if (region.gameObject.activeInHierarchy == false)
				region.emit(particlesIdle, idleCount);
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

		region.emit(particlesShake, particlesShake.emission.GetBurst(0).count.constant);
		
		yield return new WaitForSeconds(data.groundDropping.shakeTime);

		region.emit(particlesBreak, particlesBreak.emission.GetBurst(0).count.constant);

		foreach (var tile in region.layers.SelectMany(l => l.tiles))
			tile.parent.SetTile(tile.coord, null);

		StartCoroutine(Effects.instance.fade.run(region.gameObject, region.layers));

		yield return new WaitForSeconds(data.groundDropping.respawnTime - Effects.instance.fade.time);

		yield return Effects.instance.fade.run(region.gameObject, region.layers,
			stop: () => !canRespawn(tiles), revert: true);

		region.gameObject.SetActive(false);

		foreach (var tile in region.layers.SelectMany(l => l.tiles).Concat(tiles))
			TilemapHelper.setTile(tile.parent, tile);
	}

}
