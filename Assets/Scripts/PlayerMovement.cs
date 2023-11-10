using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public PlayerData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isFacingRight { get; private set; }
	[field: SerializeField, ReadOnly] public bool isMoving { get; private set; }
	[field: SerializeField, ReadOnly] public bool isJumping { get; private set; }
	[field: SerializeField, ReadOnly] public bool isDashing { get; private set; }
	[field: SerializeField, ReadOnly] public bool isAttacking { get; set; }
	[field: SerializeField, ReadOnly] public bool isFalling { get; private set; }
	[field: SerializeField, ReadOnly] public bool isGrounded { get; private set; }
	[field: SerializeField, ReadOnly] public bool isDistressed { get; private set; }
	[field: SerializeField, ReadOnly] public bool isFacingWall { get; private set; }
	[field: SerializeField, ReadOnly] public bool isLastFacedWallRight { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public bool canWallJump { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public int jumpsLeft { get; private set; }
	[field: SerializeField, ReadOnly] public int dashesLeft { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastTurnTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastGroundedTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastHurtTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastWallHoldingTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastJumpInputTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastWallJumpTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastDashInputTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastDashTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastPassInputTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastPassTime { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public float hurtCooldown { get; private set; }
	[field: SerializeField, ReadOnly] public float jumpCooldown { get; private set; }
	[field: SerializeField, ReadOnly] public float dashCooldown { get; private set; }


	private Rigidbody2D rigidBody;
    private Animator animator;

	private Transform hitbox;
    private TrailRenderer trail;
    private Collider2D groundCheck;
    private Collider2D wallCheck;

	private LayerMask playerLayer;
	private LayerMask playerInvulnerableLayer;

	private AttackController hitContact = null;
	private Vector2 moveInput;

	private bool currentJumpCuttable = false;
	private bool jumpCutInput = false;
	private bool dashCutInput = false;

	[HideInInspector] public bool registeredDownHitJump;


	private void Awake()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		hitbox = transform.Find("Hitbox").GetComponent<Transform>();
		trail = transform.Find("Dash Trail").GetComponent<TrailRenderer>();
		groundCheck = transform.Find("Ground Check").GetComponent<Collider2D>();
		wallCheck = transform.Find("Wall Check").GetComponent<Collider2D>();

		playerLayer = LayerMask.NameToLayer("Player");
		playerInvulnerableLayer = LayerMask.NameToLayer("Player Invulnerable");

		isFacingRight = true;

		lastTurnTime = float.PositiveInfinity;
		lastGroundedTime = float.PositiveInfinity;
		lastHurtTime = float.PositiveInfinity;
		lastWallHoldingTime = float.PositiveInfinity;
		lastJumpInputTime = float.PositiveInfinity;
		lastWallJumpTime = float.PositiveInfinity;
		lastDashInputTime = float.PositiveInfinity;
		lastDashTime = float.PositiveInfinity;
		lastPassInputTime = float.PositiveInfinity;
		lastPassTime = float.PositiveInfinity;
	}

	// handle inputs, platform passing, dashing, and jumping
	void Update()
	{
		updateTimers();

		updateInputs();
		updateCollisions();
		updateHurt();

		updateWallFacing();

		updatePass();
		updateDash();
		updateJump();

		animator.SetBool("isGrounded", isGrounded);
		animator.SetBool("isDistressed", isDistressed);
		animator.SetBool("isFalling", rigidBody.velocity.y < 0);
		animator.SetBool("isMoving", isMoving);
		animator.SetBool("isWallHolding", lastWallHoldingTime == 0);
	}

	// handle run
	void FixedUpdate()
	{
		if (!isDashing)
		{
			updateRun();

			if (lastWallHoldingTime == 0)
				updateWallSlide();
		}
	}


	private void updateTimers()
	{
		lastTurnTime += Time.deltaTime;
		lastGroundedTime += Time.deltaTime;
		lastHurtTime += Time.deltaTime;
		lastWallHoldingTime += Time.deltaTime;
		lastJumpInputTime += Time.deltaTime;
		lastWallJumpTime += Time.deltaTime;
		lastDashInputTime += Time.deltaTime;
		lastDashTime += Time.deltaTime;
		lastPassInputTime += Time.deltaTime;
		lastPassTime += Time.deltaTime;

		hurtCooldown -= Time.deltaTime;
		jumpCooldown -= Time.deltaTime;
		dashCooldown -= Time.deltaTime;
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

		if (Input.GetKey(KeyCode.RightArrow)) moveInput.x += 1;
		if (Input.GetKey(KeyCode.LeftArrow)) moveInput.x -= 1;
		if (Input.GetKey(KeyCode.UpArrow)) moveInput.y += 1;
		if (Input.GetKey(KeyCode.DownArrow)) moveInput.y -= 1;

		isMoving = moveInput.x != 0;

		if (!isDashing && !isDistressed && !isAttacking)
			if ((moveInput.x > 0 && !isFacingRight) || (moveInput.x < 0 && isFacingRight))
				Flip();

		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z))
			lastJumpInputTime = 0;

		jumpCutInput = !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.Z);

		if (Input.GetKeyDown(KeyCode.C))
			lastDashInputTime = 0;

		dashCutInput = !Input.GetKey(KeyCode.C);

		if (Input.GetKeyDown(KeyCode.DownArrow))
			lastPassInputTime = 0;
	}
	
	public void hit(AttackController contact)
	{
		hitContact = contact;
	}
	private void updateCollisions()
    {
		isGrounded = groundCheck.IsTouchingLayers(data.groundLayer);
		isFacingWall = wallCheck.IsTouchingLayers(data.wallLayer);
		canWallJump = rigidBody.IsTouchingLayers(data.wallJumpLayer);

		// disable registering wall collision immediately after turning because wallCheck's hitbox
		// needs time to get updated
		if (lastTurnTime < 0.1f)
		{
			isFacingWall = false;
		}

		if ((isJumping && !isFalling) || (isFalling && rigidBody.velocity.y < -0.01f))
		{
			isGrounded = false;
		}

		if (isFacingWall)
		{
			moveInput.x = 0;
			isMoving = false;
		}
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
		int layer = val ? playerInvulnerableLayer : playerLayer;

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

		Flip();
	}
	private void updateHurt()
	{
		if (canHurt())
			hurt();
		hitContact = null;

		if (hurtCooldown <= 0)
			setInvulnerability(false);

		if (isDistressed && lastHurtTime >= data.hurtDistressTime)
		{
			Flip();
			isDistressed = false;
		}

		if (isDistressed)
		{
			isDashing = false;
			isJumping = false;
			isGrounded = false;

			if (isFacingWall)
				Flip();

			moveInput.y = 0;
			moveInput.x = isFacingRight ? 1 : -1;
		}
	}

	private void updateWallFacing()
	{
		if (!isGrounded && !isDistressed && isFacingWall && canWallJump)
			lastWallHoldingTime = 0;

		if (isFacingWall)
			isLastFacedWallRight = isFacingRight;
	}

	private bool canPass()
	{
		return lastPassInputTime <= data.passInputBufferTime && isGrounded && !isDistressed;
	}
	private void updatePass()
	{
		if (canPass())
		{
			lastPassTime = 0;
			isGrounded = false;
		}

		int mask = Physics2D.GetLayerCollisionMask(playerLayer);
		int new_mask = mask;

		if (lastPassTime >= data.passTime)
			new_mask |= data.passableLayer;
		else
			new_mask &= ~data.passableLayer;

		if (mask != new_mask)
			Physics2D.SetLayerCollisionMask(playerLayer, new_mask);
	}

	private bool canDash()
	{
		return dashCooldown <= 0 && lastDashInputTime <= data.dashInputBufferTime && dashesLeft > 0
			&& !isDistressed;
	}
	private void dash()
	{
		isDashing = true;
		lastDashTime = 0;
		lastDashInputTime = float.PositiveInfinity;
		dashCooldown = data.dashTime + data.dashCooldown;

		dashesLeft--;

		float forceDirection = isFacingRight ? 1.0f : -1.0f;

		float force = data.dashForce;
		if (Mathf.Sign(forceDirection) == Mathf.Sign(rigidBody.velocity.x))
			force -= Mathf.Abs(rigidBody.velocity.x);

		if (force > 0)
		{
			rigidBody.AddForce(force * forceDirection * Vector2.right, ForceMode2D.Impulse);
		}

		rigidBody.AddForce(-rigidBody.velocity.y * Vector2.up, ForceMode2D.Impulse);
	}
	private void updateDash()
	{
		if (canDash())
		{
			dash();
		}

		if (isDashing)
			if (dashCutInput || lastDashTime >= data.dashTime
				|| isFacingWall || Mathf.Abs(rigidBody.velocity.y) > 0.01f)
			{
				isDashing = false;
			}

		if (isDashing)
		{
			isJumping = false;
			isFalling = false;
			isGrounded = false;
		}
		trail.emitting = isDashing;

		if (isGrounded || lastWallHoldingTime == 0)
		{
			dashesLeft = data.dashesCount;
		}
	}
	
	private bool canJump()
	{
		return jumpCooldown <= 0 && lastJumpInputTime <= data.jumpInputBufferTime 
			&& jumpsLeft > 0 && lastWallJumpTime >= data.wallJumpTime
			&& !isDistressed;
	}
	private bool canJumpCut()
	{
		return jumpCutInput && currentJumpCuttable && !isFalling && !isDistressed;
	}
	private void jump()
	{
		isJumping = true;
		isFalling = false;
		isGrounded = false;
		lastJumpInputTime = float.PositiveInfinity;
		jumpCooldown = data.jumpCooldown;

		currentJumpCuttable = !registeredDownHitJump;

		float force = !registeredDownHitJump ? data.jumpForce : data.attackDownBounceForce;
		if (force > rigidBody.velocity.y)
		{
			force -= rigidBody.velocity.y;
			rigidBody.AddForce(force * Vector2.up, ForceMode2D.Impulse);
		}

		if (lastWallHoldingTime < data.wallJumpCoyoteTime)
		{
			lastWallHoldingTime = float.PositiveInfinity;
			lastWallJumpTime = 0;

			if (isLastFacedWallRight == isFacingRight)
				Flip();

			float forceDirection = isFacingRight ? 1.0f : -1.0f;

			float wallJumpForce = data.wallJumpForce;
			rigidBody.AddForce(wallJumpForce * forceDirection * Vector2.right, ForceMode2D.Impulse);
		}
	}
	private void updateGravityScale()
	{
		if (isDashing)
		{
			rigidBody.gravityScale = 0;
		}
		else if (lastWallHoldingTime == 0)
		{
			rigidBody.gravityScale = 0;
		}
		else if (isJumping && canJumpCut())
		{
			rigidBody.gravityScale = data.gravityScale * data.jumpCutGravityMult;
		}
		else if (isJumping && Mathf.Abs(rigidBody.velocity.y) < data.jumpHangVelocityThreshold)
		{
			rigidBody.gravityScale = data.gravityScale * data.jumpHangGravityMult;
		}
		else if (isFalling)
		{
			rigidBody.gravityScale = data.gravityScale * data.fallGravityMult;
		}
		else
			rigidBody.gravityScale = data.gravityScale;
	}
	private void updateJump()
	{
		if (!isGrounded && lastWallHoldingTime != 0 && rigidBody.velocity.y <= 0)
		{
			isFalling = true;
		}
		if (isFalling && !isJumping && jumpsLeft == data.jumpsCount 
			&& lastGroundedTime >= data.coyoteTime)
		{
			jumpsLeft--;
		}

		if (canJump() || registeredDownHitJump)
		{
			jump();
			if (!registeredDownHitJump)
				jumpsLeft--;
			registeredDownHitJump = false;
		}

		updateGravityScale();

		if ((isGrounded || lastWallHoldingTime == 0) && lastWallJumpTime > data.wallJumpMinTime)
		{
			jumpsLeft = data.jumpsCount;
			isJumping = false;
			isFalling = false;
			lastGroundedTime = 0;
			lastWallJumpTime = float.PositiveInfinity;
		}

		if (rigidBody.velocity.y < -data.maxFallSpeed)
		{
			rigidBody.AddForce(
				(data.maxFallSpeed + rigidBody.velocity.y) * Vector2.down, 
				ForceMode2D.Impulse);
		}
	}

	private void updateRun()
	{
		float targetSpeed = moveInput.x * data.runMaxSpeed;

		if (lastWallJumpTime < data.wallJumpTime)
		{
			targetSpeed = Mathf.Lerp(targetSpeed, rigidBody.velocity.x, data.wallJumpInputReduction);
		}

		if (isDistressed)
		{
			targetSpeed = moveInput.x * data.hurtKnockbackMaxSpeed;
		}

		float accelRate = targetSpeed == 0 ? data.runDeccelAmount : data.runAccelAmount;

		if (isJumping && Mathf.Abs(rigidBody.velocity.y) < data.jumpHangVelocityThreshold)
		{
			targetSpeed *= data.jumpHangSpeedMult;
			accelRate *= data.jumpHangSpeedMult;
		}

		float speedDif = targetSpeed - rigidBody.velocity.x;
		float movement = speedDif * accelRate;

		rigidBody.AddForce(movement * Vector2.right, ForceMode2D.Force);
	}

	private void updateWallSlide()
	{
		float targetSpeed = Mathf.Min(moveInput.y, 0f) * data.wallSlideMaxSpeed;
		float accelRate = targetSpeed == 0 ? data.wallSlideDeccelAmount : data.wallSlideAccelAmount;

		float speedDif = targetSpeed - rigidBody.velocity.y;
		float movement = speedDif * accelRate;

		rigidBody.AddForce(movement * Vector2.up, ForceMode2D.Force);
	}
}
