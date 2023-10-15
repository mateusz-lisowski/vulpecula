using System.ComponentModel;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
	public PlayerData data;
	public LayerMask groundLayer;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isFacingRight { get; private set; }
	[field: SerializeField, ReadOnly] public bool isMoving { get; private set; }
	[field: SerializeField, ReadOnly] public bool isJumping { get; private set; }
	[field: SerializeField, ReadOnly] public bool isFalling { get; private set; }
	[field: SerializeField, ReadOnly] public bool isGrounded { get; private set; }
	[field: SerializeField, ReadOnly] public bool isFacingWall { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public int jumpsLeft { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastGroundedTime { get; private set; }
	[field: SerializeField, ReadOnly] public float lastJumpInputTime { get; private set; }


	private Rigidbody2D rigidBody;
    private Animator animator;

    private BoxCollider2D groundCheck;
    private BoxCollider2D wallCheck;

	private Vector2 moveInput;

	[field: SerializeField, ReadOnly] public bool jumpCutInput { get; private set; }


	private void Awake()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		groundCheck = transform.Find("Ground Check").GetComponent<BoxCollider2D>();
		wallCheck = transform.Find("Wall Check").GetComponent<BoxCollider2D>();

		isFacingRight = true;

		lastGroundedTime = float.PositiveInfinity;
		lastJumpInputTime = float.PositiveInfinity;
	}

	// handle inputs and jumping
	void Update()
	{
		// tmp for debugging
		if (Input.GetKey(KeyCode.X))
			Time.timeScale = 0.05f;
		else
			Time.timeScale = 1.0f;

		updateTimers();

		updateInputs();
		updateCollisions();

		updateJump();

		animator.SetBool("isGrounded", isGrounded);
		animator.SetBool("isFalling", rigidBody.velocity.y < 0);
		animator.SetBool("isMoving", isMoving);
	}

	// handle run
	void FixedUpdate()
	{
		updateRun();
	}


	private void updateTimers()
	{
		lastGroundedTime += Time.deltaTime;
		lastJumpInputTime += Time.deltaTime;
	}

	private void Flip()
	{
		isFacingRight = !isFacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x = -theScale.x;
		transform.localScale = theScale;
	}
	private void updateInputs()
	{
		moveInput = new Vector2();

		if (Input.GetKey(KeyCode.RightArrow)) moveInput.x += 1;
		if (Input.GetKey(KeyCode.LeftArrow)) moveInput.x -= 1;
		if (Input.GetKey(KeyCode.UpArrow)) moveInput.y += 1;
		if (Input.GetKey(KeyCode.DownArrow)) moveInput.y -= 1;

		isMoving = moveInput.x != 0;

		if ((moveInput.x > 0 && !isFacingRight) || (moveInput.x < 0 && isFacingRight))
			Flip();

		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z))
			lastJumpInputTime = 0;

		jumpCutInput = !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.Z);
	}
	
	private void updateCollisions()
    {
		isGrounded = groundCheck.IsTouchingLayers(groundLayer);
		isFacingWall = wallCheck.IsTouchingLayers(groundLayer);

		if (isJumping && !isFalling)
		{
			isGrounded = false;
		}

		if (isFacingWall)
		{
			moveInput.x = 0;
		}
	}
    
	private bool canJump()
	{
		return lastJumpInputTime < data.jumpInputBufferTime && jumpsLeft > 0;
	}
	private bool canJumpCut()
	{
		return jumpCutInput && !isFalling;
	}
	private void jump()
	{
		isJumping = true;
		isFalling = false;
		isGrounded = false;
		lastJumpInputTime = float.PositiveInfinity;

		jumpsLeft--;

		float force = data.jumpForce;
		if (force > rigidBody.velocity.y)
		{
			force -= rigidBody.velocity.y;
			rigidBody.AddForce(force * Vector2.up, ForceMode2D.Impulse);
		}
	}
	private void updateGravityScale()
	{
		if (isJumping && canJumpCut())
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
		if (!isGrounded && rigidBody.velocity.y <= 0)
		{
			isFalling = true;
		}
		if (isFalling && !isJumping && jumpsLeft == data.jumpsCount 
			&& lastGroundedTime >= data.coyoteTime)
		{
			jumpsLeft--;
		}

		if (canJump())
		{
			jump();
		}

		updateGravityScale();

		if (isGrounded)
		{
			jumpsLeft = data.jumpsCount;
			isJumping = false;
			isFalling = false;
			lastGroundedTime = 0;
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
}
