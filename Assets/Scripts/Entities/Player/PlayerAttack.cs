using UnityEngine;

public class PlayerAttack : EntityBehavior
{
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isAttackDownInput { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastAttackInputTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastAttackTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastAttackDownTime { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public float attackAnyCooldown { get; private set; }
	[field: SerializeField, ReadOnly] public float attackForwardCooldown { get; private set; }
	[field: SerializeField, ReadOnly] public float attackDownCooldown { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public int attackForwardCombo { get; private set; }


	private PlayerMovement movement;
	private PlayerData data;

	private Transform attackTransform;
	private Transform attackForwardTransform;
	private Transform attackForwardStrongTransform;
	private Transform attackForwardAirTransform;
	private Transform attackDownTransform;

	private ProjectileBehavior currentAttackData = null;


	public override void onAwake()
	{
		movement = controller.getBehavior<PlayerMovement>();
		data = movement.data;

		attackTransform = transform.Find("Attack");
		attackForwardTransform = attackTransform.Find("Forward");
		attackForwardStrongTransform = attackTransform.Find("Forward Strong");
		attackForwardAirTransform = attackTransform.Find("Forward Air");
		attackDownTransform = attackTransform.Find("Down");

		lastAttackInputTime = float.PositiveInfinity;
		lastAttackTime = float.PositiveInfinity;
		lastAttackDownTime = float.PositiveInfinity;
	}

	public override void onEvent(string eventName, object eventData)
	{
		switch (eventName)
		{
			case "attackForward": attackForwardInstantiate(); break;
			case "attackForwardReset": attackForwardReset(); break;
			case "attackForwardAir": attackForwardAirInstantiate(); break;
			case "attackDown": attackDownInstantiate(); break;
			case "attackExit": attackFinish(); break;
		}
	}

	public override void onUpdate()
	{
		updateTimers();

		updateInputs();

		updateAttack();
	}


	private void updateTimers()
	{
		lastAttackInputTime += Time.deltaTime;
		lastAttackTime += Time.deltaTime;
		lastAttackDownTime += Time.deltaTime;

		attackAnyCooldown -= Time.deltaTime;
		attackForwardCooldown -= Time.deltaTime;
		attackDownCooldown -= Time.deltaTime;
	}

	private void updateInputs()
	{
		if (Input.GetKeyDown(KeyCode.X))
			lastAttackInputTime = 0;

		isAttackDownInput = Input.GetKey(KeyCode.DownArrow);
	}


	private void attackFinish()
	{
		movement.isAttacking = false;
	}

	private void attackForwardReset()
	{
		attackFinish();

		attackAnyCooldown = 0;
		attackForwardCooldown = 0;
	}
	private void attackForwardInstantiate()
	{
		bool isStrong = attackForwardCombo >= 3;
		Transform currentAttackTransform = 
			isStrong ? attackForwardStrongTransform : attackForwardTransform;

		GameObject currentAttack = Instantiate(
			isStrong ? data.attack.attackForward3Prefab 
			: attackForwardCombo == 2 ? data.attack.attackForward2Prefab
			: data.attack.attackForward1Prefab,
			currentAttackTransform.position, currentAttackTransform.rotation, transform);
		
		currentAttackData = currentAttack.GetComponent<ProjectileBehavior>();

		currentAttackData.initialize(data);
		currentAttackData.setVelocity(new Vector2(controller.rigidBody.velocity.x, 0));
		currentAttackData.setHitboxSize(currentAttackTransform.localScale);
	}
	private void attackForward()
	{
		movement.isAttacking = true;
		movement.lastAttackDown = false;

		lastAttackTime = 0;
		lastAttackInputTime = float.PositiveInfinity;
		attackAnyCooldown = attackForwardCooldown = data.attack.forwardCooldown;

		attackForwardCombo++;

		if (attackForwardCombo == 1)
			controller.animator.SetTrigger("isAttacking1");
		else if (attackForwardCombo == 2)
			controller.animator.SetTrigger("isAttacking2");
		else
			controller.animator.SetTrigger("isAttacking3");
	}

	private void attackForwardAirInstantiate()
	{
		GameObject currentAttack = Instantiate(data.attack.attackForwardAirPrefab,
			attackForwardAirTransform.position, attackForwardAirTransform.rotation, transform);

		currentAttackData = currentAttack.GetComponent<ProjectileBehavior>();

		currentAttackData.initialize(data);
		currentAttackData.setVelocity(new Vector2(controller.rigidBody.velocity.x, 0));
		currentAttackData.setHitboxSize(attackForwardAirTransform.localScale);
	}
	private void attackForwardAir()
	{
		movement.isAttacking = true;
		movement.lastAttackDown = false;

		lastAttackTime = 0;
		lastAttackInputTime = float.PositiveInfinity;
		attackAnyCooldown = attackForwardCooldown = data.attack.forwardCooldown;

		controller.animator.SetTrigger("isAttackingAir");
	}

	private void attackDownHitCallback(HitData hitData)
	{
		if (hitData.bounce)
			movement.registeredDownHitHighJump = true;

		movement.registeredDownHitJump = true;
		currentAttackData.setHitCallback(null);
	}
	private void attackDownInstantiate()
	{
		GameObject currentAttack = Instantiate(data.attack.attackDownPrefab, 
			attackDownTransform.position, attackDownTransform.rotation, transform);

		currentAttackData = currentAttack.GetComponent<ProjectileBehavior>();

		currentAttackData.initialize(data);
		currentAttackData.setHitboxSize(attackDownTransform.localScale);
		currentAttackData.setHitCallback(attackDownHitCallback);
		currentAttackData.setVertical();
	}
	private void attackDown()
	{
		movement.isAttacking = true;
		movement.lastAttackDown = true;

		lastAttackTime = 0;
		lastAttackDownTime = 0;
		lastAttackInputTime = float.PositiveInfinity;
		attackAnyCooldown = attackDownCooldown = data.attack.downCooldown;

		controller.animator.SetTrigger("isAttackingDown");
	}

	private bool canAttackAny()
	{
		return attackAnyCooldown <= 0 && lastAttackInputTime <= data.attack.inputBufferTime
			&& !movement.isDistressed && !movement.isAttacking;
	}
	private bool canAttackDown()
	{
		return attackDownCooldown <= 0;
	}
	private bool canAttackForward()
	{
		return attackForwardCooldown <= 0;
	}
	private bool canAttackForwardAir()
	{
		return !movement.isGrounded;
	}
	private void updateAttack()
	{
		if (movement.isDistressed)
		{
			movement.isAttacking = false;
			if (currentAttackData != null)
				currentAttackData.halt();
		}

		if (movement.isDistressed || lastAttackTime > data.attack.comboResetTime)
			attackForwardCombo = 0;

		if (canAttackAny())
		{
			if (isAttackDownInput)
			{
				if (canAttackDown())
					attackDown();
			}
			else
			{
				if (canAttackForward())
					if (canAttackForwardAir())
						attackForwardAir();
					else
						attackForward();
			}
		}
	}
}
