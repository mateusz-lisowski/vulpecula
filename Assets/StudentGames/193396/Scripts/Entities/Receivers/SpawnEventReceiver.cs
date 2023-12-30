using System.Collections;
using UnityEngine;

namespace _193396
{
	public class SpawnEventReceiver : EntityEventReceiver
	{
		public string eventName;
		public string eventData;
		[Space(5)]
		public GameManager.RuntimeGroup group = GameManager.RuntimeGroup.Effects;
		public GameObject prefab;
		[Space(5)]
		public float timeOffset = 0f;

		private Transform target;


		public override string[] capturableEvents => new string[] { eventName };
		public override void onEvent(string eventName, object eventData)
		{
			if (!InputtableEvent.matches(this.eventName, this.eventData, eventName, eventData))
				return;

			if (timeOffset == 0f)
				spawn();
			else
				StartCoroutine(spawnWaited());
		}


		private void spawn()
		{
			QolUtility.Instantiate(prefab, transform.position, transform.rotation,
					GameManager.instance.runtimeGroup[group]);
		}
		private IEnumerator spawnWaited()
		{
			yield return new WaitForSeconds(timeOffset);

			spawn();
		}
	}
}