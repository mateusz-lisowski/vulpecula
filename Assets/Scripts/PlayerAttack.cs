using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
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

	private Rigidbody2D rigidBody;
	private Animator animator;

	private Transform attackTransform;
	private Transform attackForwardTransform;
	private Transform attackDownTransform;

	private AttackController currentAttackData = null;


	private void Awake()
	{
		movement = transform.GetComponent<PlayerMovement>();
		data = movement.data;

		rigidBody = transform.GetComponent<Rigidbody2D>();
		animator = transform.GetComponent<Animator>();

		attackTransform = transform.Find("Attack");
		attackForwardTransform = attackTransform.Find("Forward");
		attackDownTransform = attackTransform.Find("Down");

		lastAttackInputTime = float.PositiveInfinity;
		lastAttackTime = float.PositiveInfinity;
		lastAttackDownTime = float.PositiveInfinity;
	}

	// handle inputs
	void Update()
	{
		updateTimers();

		updateInputs();

		updateAttackPosition();
		updateAttack();

		if (lastAttackTime == 0)
			if (lastAttackDownTime == 0)
				animator.SetTrigger("isAttackingDown");
			else if (attackForwardCombo == 1)
				animator.SetTrigger("isAttacking1");
			else if (attackForwardCombo == 2)
				animator.SetTrigger("isAttacking2");
			else
				animator.SetTrigger("isAttacking3");
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

	private void updateAttackPosition()
	{
		attackTransform.position = (Vector2)transform.position 
			+ rigidBody.velocity * data.attack.hitboxOffsetScale;
	}

	public void attackFinish()
	{
		movement.isAttacking = false;
	}

	public void attackForwardReset()
	{
		attackFinish();
		
		attackAnyCooldown = 0;
		attackForwardCooldown = 0;
	}
	public void attackForwardInstantiate()
	{
		GameObject currentAttack = Instantiate(data.attack.attackForwardPrefab, 
			attackForwardTransform.position, attackForwardTransform.rotation);
		
		currentAttackData = currentAttack.GetComponent<AttackController>();

		currentAttackData.setAttack(data);
		currentAttackData.setVelocity(new Vector2(rigidBody.velocity.x, 0));
		currentAttackData.setHitboxSize(attackForwardTransform.localScale);

		currentAttack.transform.parent = transform;
	}
	private void attackForward()
	{
		movement.isAttacking = true;
		movement.lastAttackDown = false;

		lastAttackTime = 0;
		lastAttackInputTime = float.PositiveInfinity;
		attackAnyCooldown = attackForwardCooldown = data.attack.forwardCooldown;

		attackForwardCombo++;
	}
	
	private void attackDownHitCallback(AttackController attackData)
	{
		movement.registeredDownHitJump = true;
		attackData.setHitCallback(null);
	}
	public void attackDownInstantiate()
	{
		GameObject currentAttack = Instantiate(data.attack.attackDownPrefab, 
			attackDownTransform.position, attackDownTransform.rotation);

		currentAttackData = currentAttack.GetComponent<AttackController>();

		currentAttackData.setAttack(data);
		currentAttackData.setHitboxSize(attackDownTransform.localScale);
		currentAttackData.setHitCallback(attackDownHitCallback);
		currentAttackData.setVertical();

		currentAttack.transform.parent = transform;
	}
	private void attackDown()
	{
		movement.isAttacking = true;
		movement.lastAttackDown = true;

		lastAttackTime = 0;
		lastAttackDownTime = 0;
		lastAttackInputTime = float.PositiveInfinity;
		attackAnyCooldown = attackDownCooldown = data.attack.downCooldown;
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
					attackForward();
			}
		}
	}
}
