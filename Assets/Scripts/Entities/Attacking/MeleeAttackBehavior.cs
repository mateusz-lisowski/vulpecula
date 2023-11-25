using UnityEngine;

public class MeleeAtackBehavior : EntityBehavior
{
	public MeleeAtackBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isProvoked { get; private set; }
	[field: SerializeField, ReadOnly] public bool isAttacking { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public float attackCooldown { get; private set; }

	private FlipBehavior direction;
	private HurtBehavior hurt;

	private Transform attackTransform;
	private Collider2D attackCheck;

	private bool justAttacked = false;
	private AttackController currentAttackData = null;


	public override void onAwake()
	{
		direction = controller.getBehavior<FlipBehavior>();
		hurt = controller.getBehavior<HurtBehavior>();

		attackTransform = controller.transform.Find(data.provokeDetectionName);
		attackCheck = attackTransform.GetComponent<Collider2D>();
	}

	public override void onEvent(string eventName, object eventData)
	{
		if (eventName == data.attackInstantiateEventName)
			attackInstantiate();
		else if (eventName == data.attackInstantiateEventName + "Exit")
			isAttacking = false;
	}

	public override void onUpdate()
	{
		attackCooldown -= Time.deltaTime;

		updateCollisions();

		updateAttack();

		foreach (var param in controller.animator.parameters)
			if (param.name == data.animatorEventName)
			{
				if (justAttacked)
					controller.animator.SetTrigger(data.animatorEventName);
			}
	}

	public override bool onFixedUpdate()
	{
		if (!isAttacking)
			return false;

		addSmoothForce(data.attackRunSpeed, data.accelerationCoefficient, transform.right);

		return true;
	}


	private void updateCollisions()
	{
		isProvoked = attackCheck.IsTouchingLayers(data.provokeLayers);
	}

	private bool canAttack()
	{
		return attackCooldown <= 0 && isProvoked && !(hurt != null && hurt.isDistressed);
	}
	private void attackInstantiate()
	{
		isAttacking = false;

		GameObject currentAttack = Object.Instantiate(data.attackPrefab,
			attackTransform.position, attackTransform.rotation);

		currentAttackData = currentAttack.GetComponent<AttackController>();

		currentAttackData.setAttack(data);
		currentAttackData.setHitboxSize(attackTransform.localScale);
	}
	private void attack()
	{
		isAttacking = true;

		justAttacked = true;
		attackCooldown = data.cooldown;
	}
	private void updateAttack()
	{
		justAttacked = false;

		if (canAttack())
		{
			attack();
		}

		if (isAttacking)
			direction.disableFlipNextFrame();
	}
}