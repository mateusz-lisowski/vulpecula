using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isAttacking { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastAttackTime { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public float attackCooldown { get; private set; }


	private EnemyMovement movement;
	private EnemyData data;

	private Rigidbody2D rigidBody;
	private Animator animator;

	private Transform attackTransform;


	private void Awake()
	{
		movement = transform.GetComponent<EnemyMovement>();
		data = movement.data;

		rigidBody = transform.GetComponent<Rigidbody2D>();
		animator = transform.GetComponent<Animator>();

		attackTransform = transform.Find("Attack");

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
	public void attackInstantiate()
	{
		GameObject currentAttack = Instantiate(data.attackPrefab,
			attackTransform.position, attackTransform.rotation);

		AttackController currentAttackData = currentAttack.GetComponent<AttackController>();

		currentAttackData.setAttack(data);
		currentAttackData.setHitboxSize(attackTransform.localScale);

		currentAttackData.resolve();
	}
	private void attack()
	{
		lastAttackTime = 0;
		attackCooldown = data.attackCooldown;
	}
	private void updateAttack()
	{
		if (canAttack())
		{
			attack();
		}
	}
}
