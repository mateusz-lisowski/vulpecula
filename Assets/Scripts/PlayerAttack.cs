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
	[field: SerializeField, ReadOnly] public float attackCooldown { get; private set; }


	private PlayerMovement movement;
	private PlayerData data;

	private Rigidbody2D rigidBody;
	private Animator animator;

	private Transform attackTransform;
	private Transform attackForwardTransform;
	private Transform attackDownTransform;


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
			else
				animator.SetTrigger("isAttacking");
	}


	private void updateTimers()
	{
		lastAttackInputTime += Time.deltaTime;
		lastAttackTime += Time.deltaTime;
		lastAttackDownTime += Time.deltaTime;

		attackCooldown -= Time.deltaTime;
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

	public void attackForwardInstantiate()
	{
		movement.isAttacking = false;

		GameObject currentAttack = Instantiate(data.attack.attackForwardPrefab, 
			attackForwardTransform.position, attackForwardTransform.rotation);
		AttackController currentAttackData = currentAttack.GetComponent<AttackController>();

		currentAttackData.setAttack(data);
		currentAttackData.setHitboxSize(attackForwardTransform.localScale);

		currentAttackData.resolve();
	}
	private void attackForward()
	{
		lastAttackTime = 0;
		attackCooldown = data.attack.cooldown;
	}
	
	private void attackDownHitCallback(AttackController attackData)
	{
		movement.registeredDownHitJump = true;
		attackData.setHitCallback(null);
	}
	public void attackDownInstantiate()
	{
		movement.isAttacking = false;

		GameObject currentAttack = Instantiate(data.attack.attackDownPrefab, 
			attackDownTransform.position, attackDownTransform.rotation);

		AttackController currentAttackData = currentAttack.GetComponent<AttackController>();

		currentAttackData.setAttack(data);
		currentAttackData.setHitboxSize(attackDownTransform.localScale);
		currentAttackData.setHitCallback(attackDownHitCallback);
		currentAttackData.setVertical();

		currentAttackData.resolve();
	}
	private void attackDown()
	{
		lastAttackTime = 0;
		lastAttackDownTime = 0;
		attackCooldown = data.attack.cooldown;
	}

	private bool canAttack()
	{
		return attackCooldown <= 0 && lastAttackInputTime <= data.attack.inputBufferTime
			&& !movement.isDistressed && !movement.isAttacking;
	}
	private void updateAttack()
	{
		if (movement.isDistressed)
			movement.isAttacking = false;

		if (canAttack())
		{
			movement.isAttacking = true;

			if (isAttackDownInput)
				attackDown();
			else
				attackForward();
		}
	}
}
