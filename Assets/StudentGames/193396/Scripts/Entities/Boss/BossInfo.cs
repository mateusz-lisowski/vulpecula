using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _193396
{
	[RequireComponent(typeof(HurtBehavior))]
	public class BossInfo : EntityBehavior
	{
		public BossData data;

		private HurtBehavior hurt;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isActive { get; private set; }
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public float passiveHealCooldown { get; private set; }


		public float healthNormalized => hurt == null ? 1f : (float)hurt.health / hurt.data.health;

		public override void onAwake()
		{
			hurt = controller.getBehavior<HurtBehavior>();
		}

		public override string[] capturableEvents => new string[] { "awake", "sleep" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "awake": activate(); break;
				case "sleep": deactivate(); break;
			}
		}

		public override void onUpdate()
		{
			passiveHealCooldown -= Time.deltaTime;

			if (!isActive && passiveHealCooldown <= 0f)
			{
				int healCount = 0;
				while (passiveHealCooldown <= 0f)
				{
					healCount++;
					passiveHealCooldown += data.passiveHealCooldown;
				}

				hurt.tryHeal(healCount);
			}
		}


		private void activate()
		{
			isActive = true;
		}
		private void deactivate()
		{
			isActive = false;
			passiveHealCooldown = 0;
		}

	}
}