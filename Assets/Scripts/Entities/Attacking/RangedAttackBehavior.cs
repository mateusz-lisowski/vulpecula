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

	private ProjectileBehavior currentAttackData = null;


	public override void onAwake()
	{
		hurt = controller.getBehavior<HurtBehavior>();
		chase = controller.getBehavior<ChaseBehavior>();

		Transform attackTransform = controller.transform.Find(data.provokeDetectionName);

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

		updateAttack();
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
		data.source.controller.onEvent("hit", data);
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

		attackCooldown = data.cooldown;

		controller.onEvent("attackBegin", data.attackInstantiateEventName);
	}
	private void updateAttack()
	{
		if (canAttack())
		{
			attack();
		}
	}
}