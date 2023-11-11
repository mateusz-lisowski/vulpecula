using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
	public EnemyData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isFacingRight { get; private set; }
	[field: SerializeField, ReadOnly] public bool isMoving { get; private set; }
	[field: SerializeField, ReadOnly] public bool isFalling { get; private set; }
	[field: SerializeField, ReadOnly] public bool isGrounded { get; private set; }
	[field: SerializeField, ReadOnly] public bool isDistressed { get; private set; }
	[field: SerializeField, ReadOnly] public bool isProvoked { get; private set; }
	[field: SerializeField, ReadOnly] public bool isFacingWall { get; private set; }
	[field: SerializeField, ReadOnly] public bool isNoGroundAhead { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastTurnTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastGroundedTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastHurtTime { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public float hurtCooldown { get; private set; }


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

	private AttackController hitContact = null;
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

		enemyLayer = LayerMask.NameToLayer("Enemy");
		enemyInvulnerableLayer = LayerMask.NameToLayer("Enemy Invulnerable");

		isFacingRight = center.transform.right.x > 0;

		lastTurnTime = float.PositiveInfinity;
		lastGroundedTime = float.PositiveInfinity;
		lastHurtTime = float.PositiveInfinity;
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
			else if (param.name == "isDistressed")
				animator.SetBool("isDistressed", isDistressed);
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

		hurtCooldown -= Time.deltaTime;
	}

	public void hit(AttackController contact)
	{
		hitContact = contact;
	}
	private bool isFacingSlope()
	{
		Vector2 sourceLow = new Vector2(center.position.x, center.position.y - 0.2f);
		Vector2 sourceHigh = new Vector2(center.position.x, center.position.y + 0.2f);

		Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;

		RaycastHit2D hitLow = Physics2D.Raycast(sourceLow, dir, Mathf.Infinity, data.run.groundLayers);
		RaycastHit2D hitHigh = Physics2D.Raycast(sourceHigh, dir, Mathf.Infinity, data.run.groundLayers);

		return Mathf.Abs(hitHigh.point.x - hitLow.point.x) > 0.1f;
	}
	private void updateCollisions()
	{
		isGrounded = groundCheck.IsTouchingLayers(data.run.groundLayers);
		isFacingWall = wallCheck.IsTouchingLayers(data.run.groundLayers);
		isNoGroundAhead = !fallCheck.IsTouchingLayers(data.run.groundLayers);
		isProvoked = attackCheck.IsTouchingLayers(data.attack.provokeLayers);

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
	}

	private bool canHurt()
	{
		return hitContact != null && hurtCooldown <= 0;
	}
	private void setDistressDirection()
	{
		if (hitContact.isVertical)
			return;

		// if attack faces the same direction
		if (Vector2.Dot(transform.right, hitContact.transform.right) > 0)
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
		hurtCooldown = data.hurt.invulnerabilityTime;

		setDistressDirection();
		setInvulnerability(true);
		StartCoroutine(Effects.instance.Flashing(gameObject, data.hurt.invulnerabilityTime));

		float force = data.hurt.knockbackForce;
		if (force > rigidBody.velocity.y)
		{
			force -= rigidBody.velocity.y;
			rigidBody.AddForce(force * Vector2.up, ForceMode2D.Impulse);
		}
	}
	private void updateHurt()
	{
		if (canHurt())
			hurt();
		hitContact = null;

		if (hurtCooldown <= 0)
			setInvulnerability(false);

		if (isDistressed && lastHurtTime >= data.hurt.distressTime)
		{
			isDistressed = false;
		}

		if (isDistressed)
		{
			isGrounded = false;

			moveInput.y = 0;
			moveInput.x = isFacingRight ? -1 : 1;
		}
	}

	private void updateGravityScale()
	{
		rigidBody.gravityScale = data.gravity.scale;
	}
	private void updateJump()
	{
		if (!isGrounded && rigidBody.velocity.y <= 0)
		{
			isFalling = true;
		}

		updateGravityScale();

		if (isGrounded)
		{
			isFalling = false;
			lastGroundedTime = 0;
		}
	}

	private void updateRun()
	{
		float targetSpeed = moveInput.x * data.run.maxSpeed;

		if (isDistressed)
		{
			targetSpeed = moveInput.x * data.hurt.knockbackMaxSpeed;
		}

		float accelRate = targetSpeed == 0 ? data.run.decelerationForce : data.run.accelerationForce;

		float speedDif = targetSpeed - rigidBody.velocity.x;
		float movement = speedDif * accelRate;

		rigidBody.AddForce(movement * Vector2.right, ForceMode2D.Force);
	}
}
