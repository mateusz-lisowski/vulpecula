using System.Collections.Generic;
using UnityEngine;

public class MeleeAtackBehavior : EntityBehavior
{
	public MeleeAttackBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isProvoked { get; private set; }
	[field: SerializeField, ReadOnly] public bool isAttacking { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public float attackCooldown { get; private set; }

	private HurtBehavior hurt;

	private Dictionary<string, Transform> attackTransforms;
	private Collider2D attackCheck;

	private bool justAttacked = false;
	private ProjectileBehavior currentAttackData = null;


	public override void onAwake()
	{
		hurt = controller.getBehavior<HurtBehavior>();

		Transform attackTransform = controller.transform.Find(data.provokeDetectionName);
		attackCheck = attackTransform.GetComponent<Collider2D>();

		attackTransforms = new Dictionary<string, Transform>();

		attackTransforms[data.attackInstantiateEventName] = attackTransform;
		foreach (Transform childTransform in attackTransform)
			attackTransforms[data.attackInstantiateEventName + "." + childTransform.name] = childTransform;
	}

	public override string[] capturableEvents => new string[] { "attack", "attackExit" };
	public override void onEvent(string eventName, object eventData)
	{
		string name = eventData == null ? "attack" : eventData as string;

		switch (eventName)
		{
			case "attackExit":
				if (name == data.attackInstantiateEventName)
					isAttacking = false;
				break;
			case "attack":
				if (attackTransforms.ContainsKey(name))
				{
					attackInstantiate(attackTransforms[name]);
					if (name == data.attackInstantiateEventName)
						isAttacking = false;
				}
				break;
		}
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
	private void attackInstantiate(Transform attackTransform)
	{
		GameObject currentAttack = Instantiate(data.attackPrefab,
			attackTransform.position, attackTransform.rotation, transform);

		currentAttackData = currentAttack.GetComponent<ProjectileBehavior>();

		currentAttackData.initialize(data);
		currentAttackData.setStrength(data.strength);
		currentAttackData.setHitboxSize(attackTransform.lossyScale);
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
	}
}