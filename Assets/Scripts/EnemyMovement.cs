using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
	public EnemyData data;
	public LayerMask groundLayer;
	public bool awakeMoveRight;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isFacingRight { get; private set; }
	[field: SerializeField, ReadOnly] public bool isMoving { get; private set; }
	[field: SerializeField, ReadOnly] public bool isJumping { get; private set; }
	[field: SerializeField, ReadOnly] public bool isDashing { get; private set; }
	[field: SerializeField, ReadOnly] public bool isFalling { get; private set; }
	[field: SerializeField, ReadOnly] public bool isGrounded { get; private set; }
	[field: SerializeField, ReadOnly] public bool isFacingWall { get; private set; }
	[field: SerializeField, ReadOnly] public bool isNoGroundAhead { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastTurnTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastGroundedTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastJumpInputTime { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public float jumpCooldown { get; private set; }


	private Rigidbody2D rigidBody;
	private Animator animator;

	private BoxCollider2D groundCheck;
	private BoxCollider2D wallCheck;
	private BoxCollider2D fallCheck;

	private Vector2 moveInput;


	private void Awake()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		groundCheck = transform.Find("Ground Check").GetComponent<BoxCollider2D>();
		wallCheck = transform.Find("Wall Check").GetComponent<BoxCollider2D>();
		fallCheck = transform.Find("Fall Check").GetComponent<BoxCollider2D>();

		isFacingRight = data.defaultFacingRight;
		if (awakeMoveRight != isFacingRight)
		{
			Flip();
		}

		lastGroundedTime = float.PositiveInfinity;
		lastJumpInputTime = float.PositiveInfinity;

		jumpCooldown = 0;
	}

	// handle timers and animations
	void Update()
	{
		updateTimers();

		updateCollisions();
		updateInputs();

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
		lastJumpInputTime += Time.deltaTime;

		jumpCooldown -= Time.deltaTime;
	}

	private bool isFacingSlope()
	{
		Vector2 center = (Vector2)transform.position + data.centerOffset;

		Vector2 sourceLow = new Vector2(center.x, center.y - 0.2f);
		Vector2 sourceHigh = new Vector2(center.x, center.y + 0.2f);

		Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;

		RaycastHit2D hitLow = Physics2D.Raycast(sourceLow, dir, Mathf.Infinity, groundLayer.value);
		RaycastHit2D hitHigh = Physics2D.Raycast(sourceHigh, dir, Mathf.Infinity, groundLayer.value);

		return Mathf.Abs(hitHigh.point.x - hitLow.point.x) > 0.1f;
	}
	private void updateCollisions()
	{
		isGrounded = groundCheck.IsTouchingLayers(groundLayer);
		isFacingWall = wallCheck.IsTouchingLayers(groundLayer);
		isNoGroundAhead = !fallCheck.IsTouchingLayers(groundLayer);

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
		Vector3 theScale = transform.localScale;
		theScale.x = -theScale.x;
		transform.localScale = theScale;
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
