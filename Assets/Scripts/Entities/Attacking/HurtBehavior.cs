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

	private FlipBehavior direction;
	private LayerMask enemyLayer;
	private LayerMask enemyInvulnerableLayer;

	private AttackController hitContact = null;


	public override void onAwake()
	{
		direction = controller.getBehavior<FlipBehavior>();

		enemyLayer = LayerMask.NameToLayer("Enemy");
		enemyInvulnerableLayer = LayerMask.NameToLayer("Enemy Invulnerable");

		lastHurtTime = float.PositiveInfinity;
	}

	public override void onEvent(string eventName, object eventData)
	{
		switch (eventName)
		{
			case "hit":
				hitContact = (AttackController)eventData;
				break;
		}
	}

	public override void onUpdate()
	{
		lastHurtTime += Time.deltaTime;
		hurtCooldown -= Time.deltaTime;

		if (canHurt())
			hurt();
		hitContact = null;

		if (hurtCooldown <= 0)
			setInvulnerability(false);

		if (isDistressed && lastHurtTime >= data.distressTime)
		{
			isDistressed = false;
		}

		foreach (var param in controller.animator.parameters)
			if (param.name == "isDistressed")
				controller.animator.SetBool("isDistressed", isDistressed);
	}


	private bool canHurt()
	{
		return hitContact != null && hurtCooldown <= 0;
	}
	private void setDistressDirection()
	{
		if (direction == null)
			return;

		if (hitContact.isVertical)
			return;

		// if attack faces the same direction
		if (Vector2.Dot(transform.right, hitContact.transform.right) > 0)
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
	}
}
