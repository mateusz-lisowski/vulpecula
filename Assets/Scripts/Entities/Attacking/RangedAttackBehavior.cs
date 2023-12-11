using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChaseBehavior))]
public class RangedAttackBehavior : EntityBehavior
{
	public RangedAttackBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isAttacking { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public float attackCooldown { get; private set; }

	private HurtBehavior hurt;
	private ChaseBehavior chase;

	private Dictionary<string, Transform> attackTransforms;

	private bool justAttacked = false;
	private ProjectileBehavior currentAttackData = null;


	public override void onAwake()
	{
		hurt = controller.getBehavior<HurtBehavior>();
		chase = controller.getBehavior<ChaseBehavior>();

		Transform attackTransform = controller.transform.Find(data.provokeDetectionName);

		attackTransforms = new Dictionary<string, Transform>();

		attackTransforms[data.attackInstantiateEventName] = attackTransform;
		foreach (Transform childTransform in attackTransform)
			attackTransforms[data.attackInstantiateEventName + ":" + childTransform.name] = childTransform;
	}

	public override void onEvent(string eventName, object eventData)
	{
		if (eventName == data.attackInstantiateEventName + "Exit")
			isAttacking = false;
		else if (attackTransforms.ContainsKey(eventName))
		{
			attackInstantiate(attackTransforms[eventName]);
			if (eventName == data.attackInstantiateEventName)
				isAttacking = false;
		}
	}

	public override void onUpdate()
	{
		attackCooldown -= Time.deltaTime;

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


	private bool canAttack()
	{
		return attackCooldown <= 0 && chase.lastSeenTime == 0f && !(hurt != null && hurt.isDistressed);
	}
	private void attackBreak(HitData data)
	{
		data.source.controller.rigidBody.velocity = Vector2.zero;

		Animator animator = data.source.controller.animator;

		if (animator != null)
			foreach (var param in animator.parameters)
				if (param.name == "onHit")
				{
					animator.SetTrigger("onHit");
					return;
				}

		Destroy(data.source.gameObject);
	}
	private void attackInstantiate(Transform attackTransform)
	{
		GameObject currentAttack = Instantiate(data.attackPrefab,
			attackTransform.position, attackTransform.rotation, transform);

		currentAttackData = currentAttack.GetComponent<ProjectileBehavior>();

		currentAttackData.initialize(data);
		currentAttackData.setStrength(data.strength);
		currentAttackData.setHitboxSize(attackTransform.lossyScale);
		currentAttackData.setHitCallback(attackBreak);

		currentAttackData.controller.rigidBody.velocity = 
			(chase.lastTargetPosition - (Vector2)transform.position).normalized * data.speed;
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