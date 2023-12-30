using System;
using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class PlayerInfo : EntityBehavior
	{
		[Serializable]
		private class RuntimeData : RuntimeDataManager.Data
		{
			[field: SerializeField, ReadOnly] public float playtime = 0;
			[field: Space(5)]
			[field: SerializeField, ReadOnly] public int score = 0;
			[field: SerializeField, ReadOnly] public int killCount = 0;
			[field: Space(5)]
			[field: SerializeField, ReadOnly] public bool unlockedKey1 = false;
			[field: SerializeField, ReadOnly] public bool unlockedKey2 = false;
			[field: SerializeField, ReadOnly] public bool unlockedKey3 = false;
			[field: Space(5)]
			[field: SerializeField, ReadOnly] public string spawnpointLevel;
			[field: SerializeField, ReadOnly] public Vector2 spawnpoint;
		}

		public PlayerData data;
		[field: Space(10)]
		[field: SerializeField] private RuntimeData runtimeData;
		[Space(5)]
		[field: SerializeField, ReadOnly] private int health;

		private List<int> justCollected = new List<int>();
		private bool justHit = false;


		public float healthNormalized => (float)health / data.hurt.health;
		public float playtime => runtimeData.playtime;
		public float score => runtimeData.score;

		public override void onAwake()
		{
			health = data.hurt.health;
		}
		public override void onStart()
		{
			runtimeData = RuntimeDataManager.get<RuntimeData>("Player");

			if (runtimeData.spawnpointLevel == gameObject.scene.name)
				transform.position = runtimeData.spawnpoint;
			else
			{
				runtimeData.spawnpoint = transform.position;
				runtimeData.spawnpointLevel = gameObject.scene.name;
			}
		}

		public override string[] capturableEvents => new string[] { 
			"hit", "collect", "killed", "respawn", "focused", "unfocused" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "hit": hit(eventData as HitData); break;
				case "collect": collect(eventData as CollectData); break;
				case "killed": killed((string)eventData); break;
				case "respawn": respawn(); break;
				case "focused": controller.onEvent("inputEnable", null); break;
				case "unfocused": controller.onEvent("inputDisable", null); break;
			}
		}

		public override void onUpdate()
		{
			runtimeData.playtime += Time.deltaTime;

			justCollected.Clear();
			justHit = false;


			if (Input.GetKeyDown(KeyCode.F1))
				controller.onEvent("hit", new HitData { isVertical = true, right = Vector3.right, strength = 999 });
			else if (Input.GetKeyDown(KeyCode.F2))
			{
				controller.onEvent("collect", new CollectData { id = CollectData.idGenerator, name = "key-1" });
				controller.onEvent("collect", new CollectData { id = CollectData.idGenerator, name = "key-2" });
				controller.onEvent("collect", new CollectData { id = CollectData.idGenerator, name = "key-3" });
			}
		}


		private void respawn()
		{
			if (health != 0)
				return;

			health = data.hurt.health;
			transform.position = runtimeData.spawnpoint;
		}

		private void hit(HitData hitData)
		{
			if (justHit)
				return;
			justHit = true;

			health = Math.Max(health - hitData.strength, 0);

			controller.onEvent("hurt", healthNormalized);

			if (health == 0)
				controller.onEvent("died", null);
		}
		private void tryHeal(int count)
		{
			int newHealth = Math.Min(health + count, data.hurt.health);
			int heal = newHealth - health;

			health = newHealth;

			if (heal > 0)
				controller.onEvent("healed", heal);
		}

		private void unlock(string name)
		{
			switch (name)
			{
				case "key-1":
					if (runtimeData.unlockedKey1)
						return;
					runtimeData.unlockedKey1 = true;
					break;
				case "key-2":
					if (runtimeData.unlockedKey2)
						return;
					runtimeData.unlockedKey2 = true;
					break;
				case "key-3":
					if (runtimeData.unlockedKey3)
						return;
					runtimeData.unlockedKey3 = true;
					break;
			}

			controller.onEvent("unlocked", name);

			if (runtimeData.unlockedKey1 && runtimeData.unlockedKey2 && runtimeData.unlockedKey3)
				controller.onEvent("unlocked", "boss");
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
				case "key-2":
				case "key-3":
					unlock(collect.name);
					break;
				case "checkpoint":
					runtimeData.spawnpoint = collect.position;
					runtimeData.spawnpointLevel = gameObject.scene.name;
					break;
				default:
					if (collect.name.StartsWith("heal:"))
						tryHeal(int.Parse(collect.name.Substring(5)));
					else
						Debug.LogWarning("Collected item of unknown type: " + collect.name);
					break;
			}
		}
	
		private void killed(string name)
		{
			runtimeData.killCount++;
		}
	}
}