using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _193396
{
	public class BossInfo : EntityBehavior
	{
		public BossData data;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isActive { get; private set; }
		[field: Space(10)]
		[field: SerializeField, ReadOnly] private int health;

		private bool justHit = false;


		public float healthNormalized => (float)health / data.hurt.health;

		public override void onAwake()
		{
			health = data.hurt.health;
		}

		public override string[] capturableEvents => new string[] { "awake", "sleep", "hit" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "hit": hit(eventData as HitData); break;
				case "awake": activate(); break;
				case "sleep": deactivate(); break;
			}
		}

		public override void onUpdate()
		{
			if (!isActive)
				health = data.hurt.health;

			justHit = false;
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
		private void activate()
		{
			isActive = true;
		}
		private void deactivate()
		{
			isActive = false;
		}

	}
}