using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _193396
{
	[RequireComponent(typeof(PlayerMovement))]
	public class PlayerInfo : EntityBehavior
	{
		[Serializable]
		private class RuntimeData : RuntimeDataManager.Data
		{
			[field: SerializeField, ReadOnly] public float playtime = 0;
			[field: Space(5)]
			[field: SerializeField, ReadOnly] public int score = 0;
			[field: Space(5)]
			[field: SerializeField, ReadOnly] public bool unlockedKey1 = false;
			[field: SerializeField, ReadOnly] public bool unlockedKey2 = false;
			[field: SerializeField, ReadOnly] public bool unlockedKey3 = false;
			[field: Space(5)]
			[field: SerializeField, ReadOnly] public string spawnpointLevel;
			[field: SerializeField, ReadOnly] public Vector2 spawnpoint;
		}

		[field: SerializeField] private RuntimeData runtimeData;
		[Space(5)]
		[field: SerializeField, ReadOnly] private int health;

		private PlayerMovement movement;
		private PlayerData data;

		private List<int> justCollected = new List<int>();
		private bool justHit = false;


		public float healthNormalized()
		{
			return (float)health / data.hurt.health;
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

		public override string[] capturableEvents => new string[] { "hit", "collect", "respawn" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "hit": hit(eventData as HitData); break;
				case "collect": collect(eventData as CollectData); break;
				case "respawn": respawn(); break;
			}
		}

		public override void onUpdate()
		{
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
			if (justHit)
				return;
			justHit = true;

			health = Math.Max(health - hitData.strength, 0);

			controller.onEvent("hurt", healthNormalized());

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
					runtimeData.unlockedKey1 = true;
					controller.onEvent("unlocked", "key-1");
					break;
				case "key-2":
					runtimeData.unlockedKey2 = true;
					controller.onEvent("unlocked", "key-2");
					break;
				case "key-3":
					runtimeData.unlockedKey3 = true;
					controller.onEvent("unlocked", "key-3");
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
	}
}