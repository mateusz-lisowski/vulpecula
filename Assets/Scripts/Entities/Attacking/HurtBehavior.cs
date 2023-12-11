using System;
using UnityEngine;

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
	private LayerMask enemyLayer;
	private LayerMask enemyInvulnerableLayer;

	private HitData hitData = null;


	public override void onAwake()
	{
		direction = controller.getBehavior<FlipBehavior>();

		enemyLayer = LayerMask.NameToLayer("Enemy");
		enemyInvulnerableLayer = LayerMask.NameToLayer("Enemy Invulnerable");

		if (enemyLayer != gameObject.layer)
		{
			enemyLayer = LayerMask.NameToLayer("Enemy Flying");
			enemyInvulnerableLayer = LayerMask.NameToLayer("Enemy Flying Invulnerable");
		}

		lastHurtTime = float.PositiveInfinity;

		health = data.health;
	}

	public override string[] capturableEvents => new string[] { "hit" };
	public override void onEvent(string eventName, object eventData)
	{
		switch (eventName)
		{
			case "hit":
				hitData = eventData as HitData;
				break;
		}
	}

	public override void onUpdate()
	{
		lastHurtTime += Time.deltaTime;
		hurtCooldown -= Time.deltaTime;

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


	private bool canHurt()
	{
		return hitData != null && hurtCooldown <= 0;
	}
	private void setDistressDirection()
	{
		if (direction == null)
			return;

		if (hitData.isVertical)
			return;

		// if attack faces the same direction
		if (Vector2.Dot(transform.right, hitData.right) > 0)
			direction.flip();
	}
	private void setInvulnerability(bool val)
	{
		int layer = val ? enemyInvulnerableLayer : enemyLayer;

		foreach (Transform child in controller.hitbox)
			child.gameObject.layer = layer;

		controller.hitbox.gameObject.layer = layer;
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
		
		controller.onEvent("hurt", null);
	}

	private void die()
	{
		setDistressDirection();

		controller.StartCoroutine(Effects.instance.flashing.run(
			controller.spriteRenderer, 0, burst: true));

		controller.onEvent("died", null);
	}

	private void block()
	{
		controller.StartCoroutine(Effects.instance.flashing.run(
				controller.spriteRenderer, 0, burst: true));
	}
}
