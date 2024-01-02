using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _193396
{
	public class GroundDroppingController : GroundController
	{
		public TerrainData data;
		public override Tilemap[] tilemaps { get => droppableTilemaps; }

		public Tilemap[] droppableTilemaps;

		private Tilemap tilemap;
		private GridPreprocessor preprocessor;

		private ParticleSystem particlesIdle;
		private ParticleSystem particlesShake = null;
		private ParticleSystem particlesBreak = null;


		public void triggerDrop(Bounds bounds)
		{
			List<Vector3Int> triggeredCoords = TilemapHelper.findTriggeredWithinBounds(tilemap, bounds);
			if (triggeredCoords.Count == 0)
				return;

			var region = preprocessor.groundDroppingRegions.Find(r => r.contains(triggeredCoords));

			var tiles = TilemapHelper.getAllTiles(new Tilemap[] { tilemap }, region.coords);

			StartCoroutine(dropTiles(region, tiles));
		}


		private void Start()
		{
			tilemap = transform.GetComponent<Tilemap>();
			preprocessor = tilemap.layoutGrid.transform.GetComponent<GridPreprocessor>();

			var idleEffect = QolUtility.Instantiate(data.groundDropping.idleEffectPrefab, transform);
			idleEffect.name = data.groundDropping.idleEffectPrefab.name;
			particlesIdle = idleEffect.GetComponent<ParticleSystem>();

			if (data.groundDropping.shakeEffectPrefab != null)
			{
				var shakeEffect = QolUtility.Instantiate(data.groundDropping.shakeEffectPrefab, transform);
				shakeEffect.name = data.groundDropping.shakeEffectPrefab.name;
				particlesShake = shakeEffect.GetComponent<ParticleSystem>();
			}

			if (data.groundDropping.breakEffectPrefab != null)
			{
				var breakEffect = QolUtility.Instantiate(data.groundDropping.breakEffectPrefab, transform);
				breakEffect.name = data.groundDropping.breakEffectPrefab.name;
				particlesBreak = breakEffect.GetComponent<ParticleSystem>();
			}
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
				tiles, data.collidingLayers);

			return canRespawn;
		}
		private IEnumerator dropTiles(TilemapHelper.Region region, List<TilemapHelper.TileData> tiles)
		{
			region.gameObject.SetActive(true);
			var regionController = region.gameObject.GetComponent<EntityBehaviorController>();

			foreach (var tile in tiles)
				tile.parent.SetTile(tile.coord, null);

			if (particlesShake != null)
				region.emit(particlesShake, particlesShake.emission.GetBurst(0).count.constant);
			regionController.onEvent("shake", region.gameObject.tag);

			yield return new WaitForSeconds(data.groundDropping.shakeTime);

			foreach (var tile in region.layers.SelectMany(l => l.tiles))
				tile.parent.SetTile(tile.coord, null);

			if (particlesBreak != null)
				region.emit(particlesBreak, particlesBreak.emission.GetBurst(0).count.constant);
			regionController.onEvent("break", region.gameObject.tag);

			StartCoroutine(Effects.instance.fade.run(region.gameObject, region.layers));

			yield return new WaitForSeconds(data.groundDropping.respawnTime - Effects.instance.fade.time);

			yield return Effects.instance.fade.run(region.gameObject, region.layers,
				stop: () => !canRespawn(tiles), revert: true);

			region.gameObject.SetActive(false);

			foreach (var tile in region.layers.SelectMany(l => l.tiles).Concat(tiles))
				TilemapHelper.setTile(tile.parent, tile);
		}

	}
}