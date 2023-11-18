using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isProvoked { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastAttackTime { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public float attackCooldown { get; private set; }


	private EnemyMovement movement;
	private EnemyData data;

	private Animator animator;

	private Transform attackTransform;
	private Collider2D attackCheck;

	private AttackController currentAttackData = null;


	private void Awake()
	{
		movement = transform.GetComponent<EnemyMovement>();
		data = movement.data;

		animator = transform.GetComponent<Animator>();

		attackTransform = transform.Find("Attack");
		attackCheck = attackTransform.GetComponent<Collider2D>();

		lastAttackTime = float.PositiveInfinity;
	}

	// handle inputs
	void Update()
	{
		updateTimers();

		updateCollisions();

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

	private void updateCollisions()
	{
		isProvoked = attackCheck.IsTouchingLayers(data.attack.provokeLayers);
	}

	private bool canAttack()
	{
		return attackCooldown <= 0 && isProvoked && !movement.isDistressed;
	}
	public void attackInstantiate()
	{
		movement.isAttacking = false;

		GameObject currentAttack = Instantiate(data.attack.attackPrefab,
			attackTransform.position, attackTransform.rotation);

		currentAttackData = currentAttack.GetComponent<AttackController>();

		currentAttackData.setAttack(data);
		currentAttackData.setHitboxSize(attackTransform.localScale);

		currentAttackData.resolve();
	}
	private void attack()
	{
		movement.isAttacking = true;

		lastAttackTime = 0;
		attackCooldown = data.attack.cooldown;
	}
	private void updateAttack()
	{
		if (movement.isDistressed)
			movement.isAttacking = false;

		if (canAttack())
		{
			attack();
		}
	}
}
