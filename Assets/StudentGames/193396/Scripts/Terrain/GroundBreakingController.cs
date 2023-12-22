using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _193396
{
	public class GroundBreakingController : GroundController
	{
		public TerrainData data;
		public override Tilemap[] tilemaps { get => breakableTilemaps; }

		public Tilemap[] breakableTilemaps;

		private Tilemap tilemap;
		private GridPreprocessor preprocessor;

		private ParticleSystem particlesBreak = null;


		public void onMessage(EntityMessage msg)
		{
			if (msg.name != "hit")
				return;

			HitData contact = msg.data as HitData;

			List<Vector3Int> triggeredCoords = TilemapHelper.findTriggeredWithinBounds(tilemap, contact.bounds);
			if (triggeredCoords.Count == 0)
				return;

			var region = preprocessor.groundBreakingRegions.Find(r => r.contains(triggeredCoords));

			var tiles = TilemapHelper.getAllTiles(new Tilemap[] { tilemap }, region.coords);

			StartCoroutine(breakTiles(region, tiles));
		}


		private void Start()
		{
			tilemap = transform.GetComponent<Tilemap>();
			preprocessor = tilemap.layoutGrid.transform.GetComponent<GridPreprocessor>();

			if (data.groundBreaking.breakEffectPrefab != null)
			{
				var breakEffect = QolUtility.Instantiate(data.groundBreaking.breakEffectPrefab, transform);
				breakEffect.name = data.groundBreaking.breakEffectPrefab.name;
				particlesBreak = breakEffect.GetComponent<ParticleSystem>();
			}
		}

		private bool canRespawn(List<TilemapHelper.TileData> tiles)
		{
			bool canRespawn = !TilemapHelper.isOverlappingLayers(
				tiles, data.groundDropping.collidingLayers);

			return canRespawn;
		}
		private IEnumerator breakTiles(TilemapHelper.Region region, List<TilemapHelper.TileData> tiles)
		{
			region.gameObject.SetActive(true);

			foreach (var tile in region.layers.SelectMany(l => l.tiles).Concat(tiles))
				tile.parent.SetTile(tile.coord, null);

			if (particlesBreak != null)
				region.emit(particlesBreak, particlesBreak.emission.GetBurst(0).count.constant);

			StartCoroutine(Effects.instance.fade.run(region.gameObject, region.layers, move: false));

			yield return new WaitForSeconds(data.groundBreaking.respawnTime - Effects.instance.fade.time);

			yield return Effects.instance.fade.run(region.gameObject, region.layers, move: false,
				stop: () => !canRespawn(tiles), revert: true);

			region.gameObject.SetActive(false);

			foreach (var tile in region.layers.SelectMany(l => l.tiles).Concat(tiles))
				TilemapHelper.setTile(tile.parent, tile);
		}

	}
}