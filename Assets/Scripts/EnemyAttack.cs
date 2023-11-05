using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isAttacking { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastAttackTime { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public float attackCooldown { get; private set; }


	private Transform enemyTransform;
	private EnemyMovement movement;
	private EnemyData data;

	private Rigidbody2D rigidBody;
	private Animator animator;


	private void Awake()
	{
		enemyTransform = transform.parent;
		movement = enemyTransform.GetComponent<EnemyMovement>();
		data = movement.data;

		rigidBody = enemyTransform.GetComponent<Rigidbody2D>();
		animator = enemyTransform.GetComponent<Animator>();

		lastAttackTime = float.PositiveInfinity;
	}

	// handle inputs
	void Update()
	{
		updateTimers();

		updateAttack();

		foreach (AnimatorControllerParameter param in animator.parameters)
			if (param.name == "isAttacking")
			{
				if (lastAttackTime == 0)
					animator.SetTrigger("isAttacking");
			}
	}

	private void updateTimers()
	{
		lastAttackTime += Time.deltaTime;

		attackCooldown -= Time.deltaTime;
	}

	private bool canAttack()
	{
		return attackCooldown <= 0 && movement.isProvoked && !movement.isDistressed;
	}
	private void attack()
	{
		lastAttackTime = 0;
		attackCooldown = data.attackCooldown;

		GameObject currentAttack = Instantiate(data.attackPrefab, transform);

		AttackController currentAttackData = currentAttack.GetComponent<AttackController>();

		currentAttackData.setCollisionTime(data.attackCastTime, data.attackLastTime);
		currentAttackData.setHitboxSize(new Vector2(1f, 1f));
	}
	private void updateAttack()
	{
		if (canAttack())
		{
			attack();
		}
	}
}
