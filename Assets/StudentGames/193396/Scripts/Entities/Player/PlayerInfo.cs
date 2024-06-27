using System;
using System.Collections;
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
			[field: SerializeField, ReadOnly] public int collectibles = 0;
			[field: SerializeField, ReadOnly] public int collectibles2 = 0;
			[field: SerializeField, ReadOnly] public int damageTaken = 0;
			[field: SerializeField, ReadOnly] public int killCount = 0;
			[field: SerializeField, ReadOnly] public int deathCount = 0;
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
		[field: SerializeField, ReadOnly] private bool isUnhittable;
		[Space(5)]
		[field: SerializeField, ReadOnly] private int health;

		private List<int> justCollected = new List<int>();
		private bool justHit = false;
		private bool stopPlaytime = false;

		private float healingRate = 0f;


		public float healthNormalized => (float)health / data.hurt.health;
		public float playtime => runtimeData.playtime;
		public int collectibles => runtimeData.collectibles;
		public int collectibles2 => runtimeData.collectibles2;
		public int damageTaken => runtimeData.damageTaken;
		public int killCount => runtimeData.killCount;
		public int deathCount => runtimeData.deathCount;

		public int score()
		{
			int intScore = 1000
				+ 50 * collectibles
				+ 250 * collectibles2
				+ 100 * killCount
				- 10 * damageTaken
				- 500 * deathCount;

			if (intScore <= 0)
				return 0;

			float optimalTime = 360f;
			float maxScale = 20f;

			float timeMultiplier = maxScale / ((maxScale - 1) / optimalTime * playtime + 1);

			return Mathf.RoundToInt(intScore * timeMultiplier);
		}


		public void teleportSpawn()
		{
			controller.onEvent("hit", new HitData { isVertical = true, right = Vector3.right, strength = 999 });
		}

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
			"hit", "collect", "killed", "respawn", "focused", "unfocused", "regionEnter" };
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
				case "regionEnter": if ((string)eventData == "victory") won(); break;
			}
		}

		public override void onUpdate()
		{
			if (!stopPlaytime)
				runtimeData.playtime += Time.deltaTime;

			justCollected.Clear();
			justHit = false;
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
			if (justHit || isUnhittable)
				return;
			justHit = true;

			int oldHealth = health;
			health = Math.Max(health - hitData.strength, 0);
			runtimeData.damageTaken += Math.Max(oldHealth - health, 0);

			healingRate = 0f;

			controller.onEvent("hurt", healthNormalized);

			if (health == 0)
			{
				runtimeData.deathCount++;
				controller.onEvent("died", null);
			}
		}
		private bool tryHeal(int count)
		{
			int newHealth = Math.Min(health + count, data.hurt.health);
			int heal = newHealth - health;

			health = newHealth;

			if (heal > 0)
			{
				controller.onEvent("healed", heal);
				return true;
			}
			else
				return false;
		}
		private IEnumerator startHealing()
		{
			controller.onEvent("startHealing", null);

			while (tryHeal(1))
			{
				yield return new WaitForSeconds(1f / healingRate);

				if (healingRate == 0f)
					break;
			}

			healingRate = 0f;
			controller.onEvent("stopHealing", null);
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
					runtimeData.collectibles++;
					break;
				case "collectible2":
					runtimeData.collectibles2++;
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
					{
						float rate = float.Parse(collect.name.Substring(5));
						bool startNew = healingRate == 0f;

						if (rate > healingRate)
						{
							healingRate = rate;
							if (startNew)
								StartCoroutine(startHealing());
						}
					}
					else
						Debug.LogWarning("Collected item of unknown type: " + collect.name);
					break;
			}
		}
	
		private void killed(string name)
		{
			runtimeData.killCount++;

			switch (name)
			{
				case "boss":
					stopPlaytime = true;
					controller.animator.enabled = false;
					StartCoroutine(Effects.instance.fade.run(controller.spriteRenderer, move: false));
					isUnhittable = true;
					break;
			}
		}
		private void won()
		{
			stopPlaytime = true;
			controller.animator.enabled = false;
			StartCoroutine(Effects.instance.fade.run(controller.spriteRenderer, move: false));
			isUnhittable = true;
		}
	}
}