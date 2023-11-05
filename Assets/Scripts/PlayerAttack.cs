using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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


	private Transform playerTransform;
	private PlayerMovement movement;
	private PlayerData data;

	private Rigidbody2D rigidBody;
	private Animator animator;

	private Transform forwardTransform;
	private Transform downTransform;


	private void Awake()
	{
		playerTransform = transform.parent;
		movement = playerTransform.GetComponent<PlayerMovement>();
		data = movement.data;

		rigidBody = playerTransform.GetComponent<Rigidbody2D>();
		animator = playerTransform.GetComponent<Animator>();

		forwardTransform = transform.Find("Forward");
		downTransform = transform.Find("Down");

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
		transform.position = (Vector2)transform.parent.position 
			+ rigidBody.velocity * data.attackVelocityOffsetScale;
	}

	private void attackForward()
	{
		lastAttackTime = 0;
		attackCooldown = data.attackCooldown;

		GameObject currentAttack = Instantiate(
			data.attackForwardPrefab, forwardTransform.position, transform.rotation);

		AttackController currentAttackData = currentAttack.GetComponent<AttackController>();

		currentAttackData.setCollisionTime(data.attackCastTime, data.attackLastTime);
		currentAttackData.setHitboxSize(forwardTransform.localScale);
	}
	
	private void attackDownHitCallback(AttackController attackData)
	{
		movement.registeredDownHitJump = true;
		attackData.setHitCallback(null);
	}
	private void attackDown()
	{
		lastAttackTime = 0;
		lastAttackDownTime = 0;
		attackCooldown = data.attackCooldown;

		GameObject currentAttack = Instantiate(
			data.attackDownPrefab, downTransform.position, transform.rotation);

		AttackController currentAttackData = currentAttack.GetComponent<AttackController>();

		currentAttackData.setCollisionTime(data.attackCastTime, data.attackLastTime);
		currentAttackData.setHitboxSize(downTransform.localScale);
		currentAttackData.setHitCallback(attackDownHitCallback);
		currentAttackData.setVertical();
	}

	private bool canAttack()
	{
		return attackCooldown <= 0 && lastAttackInputTime <= data.attackInputBufferTime
			&& !movement.isDistressed;
	}
	private void updateAttack()
	{
		if (canAttack())
		{
			if (isAttackDownInput)
				attackDown();
			else
				attackForward();
		}
	}
}
