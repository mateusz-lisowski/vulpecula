using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
	public EnemyData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isFacingRight { get; private set; }
	[field: SerializeField, ReadOnly] public bool isMoving { get; private set; }
	[field: SerializeField, ReadOnly] public bool isJumping { get; private set; }
	[field: SerializeField, ReadOnly] public bool isFalling { get; private set; }
	[field: SerializeField, ReadOnly] public bool isGrounded { get; private set; }
	[field: SerializeField, ReadOnly] public bool isDistressed { get; private set; }
	[field: SerializeField, ReadOnly] public bool isTouchingAttack { get; private set; }
	[field: SerializeField, ReadOnly] public bool isProvoked { get; private set; }
	[field: SerializeField, ReadOnly] public bool isFacingWall { get; private set; }
	[field: SerializeField, ReadOnly] public bool isNoGroundAhead { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastTurnTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastGroundedTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastHurtTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastJumpInputTime { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public float hurtCooldown { get; private set; }
	[field: SerializeField, ReadOnly] public float jumpCooldown { get; private set; }


	private Rigidbody2D rigidBody;
	private Animator animator;
	private Transform center;

	private Transform hitbox;
	private Collider2D attackCheck;
	private Collider2D groundCheck;
	private Collider2D wallCheck;
	private Collider2D fallCheck;

	private LayerMask enemyLayer;
	private LayerMask enemyInvulnerableLayer;

	private Vector2 moveInput;


	private void Awake()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		center = transform.Find("Center").GetComponent<Transform>();

		hitbox = transform.Find("Hitbox").GetComponent<Transform>();
		attackCheck = transform.Find("Attack").GetComponent<Collider2D>();
		groundCheck = transform.Find("Ground Check").GetComponent<Collider2D>();
		wallCheck = transform.Find("Wall Check").GetComponent<Collider2D>();
		fallCheck = transform.Find("Fall Check").GetComponent<Collider2D>();

		enemyLayer = LayerMask.NameToLayer("Enemies");
		enemyInvulnerableLayer = LayerMask.NameToLayer("Enemies Invulnerable");

		isFacingRight = center.transform.right.x > 0;

		lastTurnTime = float.PositiveInfinity;
		lastGroundedTime = float.PositiveInfinity;
		lastHurtTime = float.PositiveInfinity;
		lastJumpInputTime = float.PositiveInfinity;
	}

	// handle timers and animations
	void Update()
	{
		updateTimers();

		updateCollisions();
		updateInputs();
		updateHurt();

		updateJump();

		foreach (AnimatorControllerParameter param in animator.parameters)
			if (param.name == "isGrounded")
				animator.SetBool("isGrounded", isGrounded);
			else if (param.name == "isFalling") 
				animator.SetBool("isFalling", rigidBody.velocity.y < 0);
			else if (param.name == "isMoving")
				animator.SetBool("isMoving", isMoving);
	}

	// handle run
	void FixedUpdate()
    {
		updateRun();
	}


	private void updateTimers()
	{
		lastTurnTime += Time.deltaTime;
		lastGroundedTime += Time.deltaTime;
		lastHurtTime += Time.deltaTime;
		lastJumpInputTime += Time.deltaTime;

		hurtCooldown -= Time.deltaTime;
		jumpCooldown -= Time.deltaTime;
	}

	private bool isFacingSlope()
	{
		Vector2 sourceLow = new Vector2(center.position.x, center.position.y - 0.2f);
		Vector2 sourceHigh = new Vector2(center.position.x, center.position.y + 0.2f);

		Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;

		RaycastHit2D hitLow = Physics2D.Raycast(sourceLow, dir, Mathf.Infinity, data.groundLayer.value);
		RaycastHit2D hitHigh = Physics2D.Raycast(sourceHigh, dir, Mathf.Infinity, data.groundLayer.value);

		return Mathf.Abs(hitHigh.point.x - hitLow.point.x) > 0.1f;
	}
	private void updateCollisions()
	{
		isGrounded = groundCheck.IsTouchingLayers(data.groundLayer);
		isFacingWall = wallCheck.IsTouchingLayers(data.groundLayer);
		isTouchingAttack = rigidBody.IsTouchingLayers(data.playerAttackLayer);
		isNoGroundAhead = !fallCheck.IsTouchingLayers(data.groundLayer);
		isProvoked = attackCheck.IsTouchingLayers(data.playerLayer);

		if (isFacingWall && isFacingSlope())
		{
			isFacingWall = false;
		}

		// disable registering wall/fall collision immediately after turning because wallCheck and
		// fallCheck hitboxes need time to get updated
		if (lastTurnTime < 0.1f)
		{
			isFacingWall = false;
			isNoGroundAhead = false;
		}

		if (isJumping && !isFalling)
		{
			isGrounded = false;
		}

		if (isFacingWall)
		{
			isMoving = false;
		}
	}

	private void Flip()
	{
		lastTurnTime = 0;

		isFacingRight = !isFacingRight;
		transform.Rotate(0, 180, 0);
	}
	private void updateInputs()
	{
		moveInput = new Vector2();

		if (isFacingWall || (isNoGroundAhead && isGrounded))
		{
			Flip();
		}

		if (isFacingRight)
			moveInput.x += 1;
		else
			moveInput.x += -1;

		isMoving = moveInput.x != 0;

		if (data.jumpEnabled && jumpCooldown <= 0)
			lastJumpInputTime = 0;
	}

	private bool canHurt()
	{
		return isTouchingAttack && hurtCooldown <= 0;
	}
	private void setDistressDirection()
	{
		ContactFilter2D filter = new ContactFilter2D().NoFilter();
		filter.SetLayerMask(data.playerAttackLayer);
		filter.useLayerMask = true;

		Collider2D[] contact = new Collider2D[1];
		if (rigidBody.OverlapCollider(filter, contact) == 0)
			return;

		Transform other = contact[0].transform;
		AttackController otherData = other.GetComponent<AttackController>();

		if (otherData == null)
			return;

		if (otherData.isVertical)
			return;

		// if attack faces the same direction
		if (Vector2.Dot(transform.right, other.right) > 0)
			Flip();
	}
	private void setInvulnerability(bool val)
	{
		int layer = val ? enemyInvulnerableLayer : enemyLayer;

		foreach (Transform child in hitbox)
			child.gameObject.layer = layer;

		hitbox.gameObject.layer = layer;
	}
	private void hurt()
	{
		isDistressed = true;
		lastHurtTime = 0;
		hurtCooldown = data.hurtInvulTime;

		setDistressDirection();
		setInvulnerability(true);
		StartCoroutine(Effects.instance.Flashing(gameObject, data.hurtInvulTime));

		float force = data.hurtKnockbackForce;
		if (force > rigidBody.velocity.y)
		{
			force -= rigidBody.velocity.y;
			rigidBody.AddForce(force * Vector2.up, ForceMode2D.Impulse);
		}
	}
	private void updateHurt()
	{
		if (canHurt())
		{
			hurt();
		}

		if (hurtCooldown <= 0)
			setInvulnerability(false);

		if (isDistressed && lastHurtTime >= data.hurtDistressTime)
		{
			isDistressed = false;
		}

		if (isDistressed)
		{
			isJumping = false;
			isGrounded = false;

			moveInput.y = 0;
			moveInput.x = isFacingRight ? -1 : 1;
		}
	}

	private bool canJump()
	{
		return jumpCooldown <= 0 && lastJumpInputTime == 0;
	}
	private void jump()
	{
		isJumping = true;
		isFalling = false;
		isGrounded = false;
		lastJumpInputTime = float.PositiveInfinity;
		jumpCooldown = data.jumpCooldown;

		float force = data.jumpForce;
		if (force > rigidBody.velocity.y)
		{
			force -= rigidBody.velocity.y;
			rigidBody.AddForce(force * Vector2.up, ForceMode2D.Impulse);
		}
	}
	private void updateGravityScale()
	{
		rigidBody.gravityScale = data.gravityScale;
	}
	private void updateJump()
	{
		if (!isGrounded && rigidBody.velocity.y <= 0)
		{
			isFalling = true;
		}

		if (canJump())
		{
			jump();
		}

		updateGravityScale();

		if (isGrounded)
		{
			isJumping = false;
			isFalling = false;
			lastGroundedTime = 0;
		}
	}

	private void updateRun()
	{
		float targetSpeed = moveInput.x * data.runMaxSpeed;

		float accelRate = targetSpeed == 0 ? data.runDeccelAmount : data.runAccelAmount;

		float speedDif = targetSpeed - rigidBody.velocity.x;
		float movement = speedDif * accelRate;

		rigidBody.AddForce(movement * Vector2.right, ForceMode2D.Force);
	}
}
