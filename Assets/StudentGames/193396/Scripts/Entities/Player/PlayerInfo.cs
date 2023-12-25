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
			[field: SerializeField, ReadOnly] public int health = 0;
			[field: SerializeField, ReadOnly] public int score = 0;
			[field: Space(5)]
			[field: SerializeField, ReadOnly] public bool unlocked_key_1 = false;
			[field: SerializeField, ReadOnly] public bool unlocked_key_2 = false;
			[field: SerializeField, ReadOnly] public bool unlocked_key_3 = false;
			[field: Space(5)]
			[field: SerializeField, ReadOnly] public float playtime = 0;
		}

		[field: SerializeField, Flatten] private RuntimeData runtimeData;

		private PlayerMovement movement;
		private PlayerData data;

		private List<int> justCollected = new List<int>();
		private bool justHit = false;


		public float health()
		{
			return (float)runtimeData.health / data.hurt.health;
		}
		public float playtime()
		{
			return runtimeData.playtime;
		}
		public int score()
		{
			return runtimeData.score;
		}

		public override void onAwake()
		{
			movement = controller.getBehavior<PlayerMovement>();
			data = movement.data;
		}
		public override void onStart()
		{
			runtimeData = RuntimeDataManager.get<RuntimeData>("Player");

			runtimeData.health = data.hurt.health;
		}

		public override string[] capturableEvents => new string[] { "hit", "collect" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "hit": hit(eventData as HitData); break;
				case "collect": collect(eventData as CollectData); break;
			}
		}

		public override void onUpdate()
		{
			runtimeData.playtime += Time.deltaTime;

			justCollected.Clear();
			justHit = false;
		}


		private void hit(HitData hitData)
		{
			if (justHit)
				return;
			justHit = true;

			runtimeData.health = Math.Max(runtimeData.health - hitData.strength, 0);

			controller.onEvent("hurt", health());

			if (runtimeData.health == 0)
				controller.onEvent("died", null);
		}

		private void collect(CollectData collect)
		{
			if (justCollected.Contains(collect.id))
				return;
			justCollected.Add(collect.id);

			switch (collect.name)
			{
				case "collectible":
					runtimeData.score++;
					break;
				case "key-1":
					runtimeData.unlocked_key_1 = true;
					controller.onEvent("unlocked", "key-1");
					break;
				case "key-2":
					runtimeData.unlocked_key_2 = true;
					controller.onEvent("unlocked", "key-2");
					break;
				case "key-3":
					runtimeData.unlocked_key_3 = true;
					controller.onEvent("unlocked", "key-3");
					break;
				default:
					Debug.LogWarning("Collected item of unknown type: " + collect.name);
					break;
			}
		}
	}
}