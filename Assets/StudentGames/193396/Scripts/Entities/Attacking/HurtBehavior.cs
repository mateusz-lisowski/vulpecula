using System;
using System.Collections;
using UnityEngine;

namespace _193396
{
	public class HurtBehavior : EntityBehavior
	{
		public HurtBehaviorData data;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isDistressed { get; private set; }
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public float lastHurtTime { get; private set; }
		[field: Space(5)]
		[field: SerializeField, ReadOnly] public float hurtCooldown { get; private set; }
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public int health { get; private set; }

		private FlipBehavior direction;

		private HitData hitData = null;

		private Func<bool> deathCondition;
		public void setDeathCondition(Func<bool> condition)
		{
			deathCondition = condition;
		}


		public float healthNormalized => (float)health / data.health;

		public void tryHeal(int count)
		{
			int newHealth = Math.Min(health + count, data.health);
			int heal = newHealth - health;

			health = newHealth;

			if (heal > 0)
				controller.onEvent("healed", heal);
		}

		public override void onAwake()
		{
			direction = controller.getBehavior<FlipBehavior>();

			lastHurtTime = float.PositiveInfinity;

			health = data.health;
		}

		public override string[] capturableEvents => new string[] { "hit", "heal" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "hit":
					hitData = eventData as HitData;
					break;
				case "heal":
					tryHeal((int)eventData);
					break;
			}
		}

		public override void onUpdate()
		{
			lastHurtTime += Time.deltaTime;
			hurtCooldown -= Time.deltaTime;

			if (controller.rigidBody.IsTouchingLayers(data.canInstaKillLayer))
				instaKill();

			if (canHurt())
				if (hitData.strength >= data.maxBlock)
				{
					health = Math.Max(health - hitData.strength, 0);

					if (health != 0)
						hurt();
					else
						die();
				}
				else
					block();
			hitData = null;

			if (hurtCooldown <= 0)
				setInvulnerability(false);

			if (isDistressed && lastHurtTime >= data.distressTime)
			{
				isDistressed = false;
				controller.onEvent("recover", null);
			}
		}

		public override bool onFixedUpdate()
		{
			if (health != 0)
				return false;

			addSmoothForce(0, 0.8f, transform.right);

			return true;
		}


		private void instaKill()
		{
			hitData = new HitData();
			hitData.isVertical = true;
			hitData.right = transform.right;
			hitData.strength = 9999;

			controller.onEvent("hit", hitData);
		}

		private bool canHurt()
		{
			return hitData != null && hurtCooldown < 0;
		}
		private void setDistressDirection()
		{
			if (direction == null || !data.faceHit)
				return;

			if (hitData.isVertical)
				return;

			// if attack faces the same direction
			if (Vector2.Dot(transform.right, hitData.right) > 0)
				direction.flip();
		}
		private void setInvulnerability(bool val)
		{
			switch (gameObject.layer)
			{
				case (int)RuntimeSettings.Layer.EnemyFlying:
					if (val)
						setHitboxLayer(RuntimeSettings.Layer.EnemyFlyingInvulnerable);
					else
						setHitboxLayer(RuntimeSettings.Layer.EnemyFlying);
					break;
				default:
					if (val)
						setHitboxLayer(RuntimeSettings.Layer.EnemyInvulnerable);
					else
						setHitboxLayer(RuntimeSettings.Layer.Enemy);
					break;
			}
		}
		private void hurt()
		{
			isDistressed = true;
			lastHurtTime = 0;
			hurtCooldown = data.invulnerabilityTime;

			setDistressDirection();
			setInvulnerability(true);

			controller.StartCoroutine(Effects.instance.flashing.run(
				controller.spriteRenderer, data.invulnerabilityTime, burst: true));

			controller.onEvent("hurt", (float)health / data.health);
		}

		private void messageDeath(EntityBehaviorController sourceEntity)
		{
			controller.onEvent("died", null);
			sourceEntity.onEvent("killed", data.killEventName);
		}
		private IEnumerator waitForDeathCondition(EntityBehaviorController sourceEntity)
		{
			do
			{
				yield return null;
			} while (deathCondition != null && !deathCondition());

			messageDeath(sourceEntity);
		}
		private void die()
		{
			setDistressDirection();

			controller.StartCoroutine(Effects.instance.flashing.run(
				controller.spriteRenderer, 0, burst: true));

			controller.onEvent("hurt", 0f);

			if (deathCondition == null || deathCondition())
				messageDeath(hitData.source.sourceEntity);
			else
				StartCoroutine(waitForDeathCondition(hitData.source.sourceEntity));			
		}

		private void block()
		{
			controller.StartCoroutine(Effects.instance.flashing.run(
					controller.spriteRenderer, 0, burst: true));
		}
	}
}