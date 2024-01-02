using System.Collections;
using System.Linq;
using UnityEngine;

namespace _193396
{
    [RequireComponent(typeof(CustomRegionsJoin))]
    public class GateController : EntityBehavior
    {
        public TerrainData data;
		[Space(10)]
		public float openTime;
		public Vector2 openDistance;
		[Space(5)]
		public float openMinDist = 0.0625f;

		private CustomRegionsJoin join;
		private Grid grid => join.grid;
		private TilemapHelper.Region region => join.region;

		private Effects.Fade openEffect;
		private bool isOpened = false;
		private bool isInTransition = false;
		private int currentTransition = 0;


		public override void onAwake()
		{
			join = GetComponent<CustomRegionsJoin>();

			openEffect = new Effects.Fade();

			openEffect.distance = openDistance.magnitude;
			openEffect.time = openTime;
			openEffect.updateFrequency = openEffect.distance / (openMinDist * openEffect.time);

			openEffect.direction = openDistance.normalized;
			openEffect.alphaDelta = 0f;
		}

		public override string[] capturableEvents => new string[] { "open", "close" };
		public override void onEvent(string eventName, object eventData)
		{
			bool focused = (string)eventData == "focused";

			switch (eventName)
			{
				case "open":
					StartCoroutine(transition(opening: true, focused));
					break;
				case "close":
					StartCoroutine(transition(opening: false, focused));
					break;
			}
		}


		private bool canRespawn()
		{
			bool canRespawn = !TilemapHelper.isOverlappingLayers(
				grid, region.coords, data.collidingLayers);

			return canRespawn;
		}

		private IEnumerator open()
		{
			region.gameObject.SetActive(true);
			foreach (var tile in region.layers.SelectMany(l => l.tiles))
				tile.parent.SetTile(tile.coord, null);

			controller.onEvent("moveBegin", "open");
			yield return openEffect.run(region.gameObject, region.layers, move: true);
			controller.onEvent("moveEnd", "open");
		}
		private IEnumerator close()
		{
			controller.onEvent("moveBegin", "close");
			yield return openEffect.run(region.gameObject, region.layers, move: true,
				stop: () => !canRespawn(), revert: true);
			controller.onEvent("moveEnd", "close");

			region.gameObject.SetActive(false);
			foreach (var tile in region.layers.SelectMany(l => l.tiles))
				TilemapHelper.setTile(tile.parent, tile);
		}
	
		private IEnumerator transition(bool opening, bool focused)
		{
			int transition = ++currentTransition;

			yield return new WaitUntil(() => !isInTransition);
			if (transition == currentTransition && isOpened != opening)
			{
				isInTransition = true;
				isOpened = opening;

				if (opening)
					yield return open();
				else
					yield return close();

				isInTransition = false;
			}
		}
	}
}