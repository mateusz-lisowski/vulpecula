using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerAttack : MonoBehaviour
{
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isAttacking { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastAttackInputTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastAttackTime { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public float attackCooldown { get; private set; }


	private Transform playerTransform;
	private PlayerMovement movement;
	private PlayerData data;

	private Rigidbody2D rigidBody;
	private Animator animator;


	private void Awake()
	{
		playerTransform = transform.parent;
		movement = playerTransform.GetComponent<PlayerMovement>();
		data = movement.data;

		rigidBody = playerTransform.GetComponent<Rigidbody2D>();
		animator = playerTransform.GetComponent<Animator>();

		lastAttackInputTime = float.PositiveInfinity;
		lastAttackTime = float.PositiveInfinity;
	}

	// handle inputs
	void Update()
	{
		updateTimers();

		updateInputs();

		updateAttack();

		if (lastAttackTime == 0)
			animator.SetTrigger("isAttacking");
	}


	private void updateTimers()
	{
		lastAttackInputTime += Time.deltaTime;
		lastAttackTime += Time.deltaTime;

		attackCooldown -= Time.deltaTime;
	}

	private void updateInputs()
	{
		if (Input.GetKeyDown(KeyCode.X))
			lastAttackInputTime = 0;
	}

	private bool canAttack()
	{
		return attackCooldown <= 0 && lastAttackInputTime <= data.attackInputBufferTime
			&& !movement.isDistressed;
	}
	private Vector2 attackForwardPosition()
	{
		Vector2 targetPosition = transform.position;
		targetPosition += rigidBody.velocity * data.attackVelocityOffsetScale;

		return targetPosition;
	}
	private void attackForward()
	{
		lastAttackTime = 0;
		attackCooldown = data.attackCooldown;

		GameObject currentAttack = Instantiate(
			data.attackForwardPrefab, attackForwardPosition(), transform.rotation);

		AttackController currentAttackData = currentAttack.GetComponent<AttackController>();

		currentAttackData.setCollisionTime(data.attackCastTime, data.attackLastTime);
		currentAttackData.setHitboxSize(transform.localScale);
	}
	private void updateAttack()
	{
		if (canAttack())
		{
			attackForward();
		}
	}


	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(attackForwardPosition(), transform.lossyScale);
	}
}
