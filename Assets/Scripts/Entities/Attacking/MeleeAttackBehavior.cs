using UnityEngine;

public class MeleeAtackBehavior : EntityBehavior
{
	public MeleeAtackBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isProvoked { get; private set; }
	[field: SerializeField, ReadOnly] public bool isAttacking { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastAttackTime { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public float attackCooldown { get; private set; }

	private HurtBehavior hurt;
	private RunBehavior run;

	private Transform attackTransform;
	private Collider2D attackCheck;

	private AttackController currentAttackData = null;


	public override void onAwake()
	{
		hurt = controller.getBehavior<HurtBehavior>();
		run = controller.getBehavior<RunBehavior>();

		attackTransform = controller.transform.Find(data.provokeDetectionName);
		attackCheck = attackTransform.GetComponent<Collider2D>();

		lastAttackTime = float.PositiveInfinity;
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
		lastAttackTime += Time.deltaTime;
		attackCooldown -= Time.deltaTime;

		updateCollisions();

		updateAttack();

		foreach (var param in controller.animator.parameters)
			if (param.name == data.animatorEventName)
			{
				if (lastAttackTime == 0)
					controller.animator.SetTrigger(data.animatorEventName);
			}
	}

	public override void onFixedUpdate()
	{
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
		if (run != null)
			isAttacking = false;

		GameObject currentAttack = Object.Instantiate(data.attackPrefab,
			attackTransform.position, attackTransform.rotation);

		currentAttackData = currentAttack.GetComponent<AttackController>();

		currentAttackData.setAttack(data);
		currentAttackData.setHitboxSize(attackTransform.localScale);
	}
	private void attack()
	{
		if (run != null)
			isAttacking = true;

		lastAttackTime = 0;
		attackCooldown = data.cooldown;
	}
	private void updateAttack()
	{
		if (canAttack())
		{
			attack();
		}

		if (isAttacking)
			run.currentFrameStopping = true;
	}
}