using System;
using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(PlayerMovement))]
	public class PlayerInfo : EntityBehavior
	{
		[Serializable]
		private class RuntimeData : RuntimeDataManager.Data
		{
			[field: SerializeField, ReadOnly] public int score = 0;
			[field: Space(5)]
			[field: SerializeField, ReadOnly] public float playtime = 0;
		}

		[field: SerializeField, Flatten] private RuntimeData runtimeData;

		private PlayerMovement movement;
		private PlayerData data;

		private List<int> justCollected = new List<int>();


		public override void onAwake()
		{
			movement = controller.getBehavior<PlayerMovement>();
			data = movement.data;
		}
		public override void onStart()
		{
			runtimeData = RuntimeDataManager.get<RuntimeData>("Player");
		}

		public override string[] capturableEvents => new string[] { "collect" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "collect": collect(eventData as CollectData); break;
			}
		}

		public override void onUpdate()
		{
			runtimeData.playtime += Time.deltaTime;

			justCollected.Clear();
		}


		private void collect(CollectData collect)
		{
			if (justCollected.Contains(collect.id))
				return;

			justCollected.Add(collect.id);
			runtimeData.score++;

			Debug.Log("Collected " + runtimeData.score + " collectible at: " + runtimeData.playtime);
		}

	}
}