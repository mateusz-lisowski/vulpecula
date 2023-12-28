using System.Collections;
using System.Linq;
using UnityEngine;

namespace _193396
{
    [RequireComponent(typeof(CustomRegionsJoin))]
    public class BridgeController : EntityBehavior
    {
        public TerrainData data;

		private CustomRegionsJoin join;
		private Grid grid => join.grid;
		private TilemapHelper.Region region => join.region;

		private ParticleSystem particlesBreak = null;
		private bool isDestroyed = false;
		private bool isInTransition = false;
		private int currentTransition = 0;


		public override void onAwake()
		{
			join = GetComponent<CustomRegionsJoin>();

			if (data.groundBreaking.breakEffectPrefab != null)
			{
				var breakEffect = QolUtility.Instantiate(data.groundBreaking.breakEffectPrefab, transform);
				breakEffect.name = data.groundBreaking.breakEffectPrefab.name;
				particlesBreak = breakEffect.GetComponent<ParticleSystem>();
			}
		}

		public override string[] capturableEvents => new string[] { "break", "restore" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "break":
					StartCoroutine(transition(removing: true));
					break;
				case "restore":
					StartCoroutine(transition(removing: false));
					break;
			}
		}


		private bool canRespawn()
		{
			bool canRespawn = !TilemapHelper.isOverlappingLayers(
				grid, region.coords, data.collidingLayers);

			return canRespawn;
		}

		private IEnumerator remove()
		{
			region.gameObject.SetActive(true);
			foreach (var tile in region.layers.SelectMany(l => l.tiles))
				tile.parent.SetTile(tile.coord, null);

			if (particlesBreak != null)
				region.emit(particlesBreak, particlesBreak.emission.GetBurst(0).count.constant);

			yield return Effects.instance.fade.run(region.gameObject, region.layers, move: false);
		}
		private IEnumerator restore()
		{
			yield return Effects.instance.fade.run(region.gameObject, region.layers, move: false,
				stop: () => !canRespawn(), revert: true);

			region.gameObject.SetActive(false);
			foreach (var tile in region.layers.SelectMany(l => l.tiles))
				TilemapHelper.setTile(tile.parent, tile);
		}
	
		private IEnumerator transition(bool removing)
		{
			int transition = ++currentTransition;

			yield return new WaitUntil(() => !isInTransition);
			if (transition == currentTransition && isDestroyed != removing)
			{
				isInTransition = true;
				isDestroyed = removing;

				if (removing)
					yield return remove();
				else
					yield return restore();

				isInTransition = false;
			}
		}
	}
}