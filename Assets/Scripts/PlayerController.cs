using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Movement parameters")]
    [Range(0.01f, 20.0f)] [SerializeField] private float moveSpeed = 0.1f; // moving speed of the player
    [Range(0.0f, 1.0f)] [SerializeField] private float frictionCoeff = 0.2f; // ground friction
	[Space(5)]
    [Range(0.01f, 20.0f)] [SerializeField] private float acceleration = 1.0f; // acceleration multiplier
    [Range(0.01f, 20.0f)] [SerializeField] private float decceleration = 1.0f; // decceleration multiplier
	[Header("Jump parameters")]
	[Range(0.01f, 20.0f)] [SerializeField] private float jumpForce = 6.0f; // jump force of the player
    [Range(0.01f, 5.0f)] [SerializeField] private float jumpCooldown = 0.3f; // cooldown between jumps
	// maximum allowed time between jump input and actual jump (if a key is released before can jump)
	[Range(0.01f, 5.0f)] [SerializeField] private float jumpTimeThreshold = 0.1f; 
	[Space(5)]
	[Range(0.0f, 1.0f)] [SerializeField] private float jumpCutMultiplier = 0.4f; // strength of jump cut
	[Range(0.01f, 5.0f)] [SerializeField] private float fallGravityMultiplier = 1.0f; // gravity when falling
	[Space(5)]
	[Range(0.01f, 5.0f)][SerializeField] private float jumpHangVelThreshold = 1.0f; // velocity of jump hang
	[Range(0.01f, 5.0f)][SerializeField] private float jumpHangGravityMultiplier = 1.0f; // gravity when jump hang
    [Space(10)]
	public LayerMask groundLayer;

    private Rigidbody2D rigidBody;
    private Animator animator;

    private BoxCollider2D groundCheck;
    private BoxCollider2D wallCheck;

    private bool isFacingRight = true;
    private bool isMoving = false;
    private bool isGrounded = false;
    private bool isFacingWall = false;

	private float lastJumpTime = 0;
    private float lastJumpInputTime = 0;

	private int jumpsSinceGrounded = 0;

    private int score = 0;

    private void UpdateCollisions()
    {
		isGrounded = groundCheck.IsTouchingLayers(groundLayer);
		isFacingWall = wallCheck.IsTouchingLayers(groundLayer);

		// do not reset jump until falling
        if (isGrounded && rigidBody.velocity.y > 0.0f && jumpsSinceGrounded != 0)
        {
            isGrounded = false;
        }
	}
    
	private void updateMovementControlls(out float moveInput)
	{
		moveInput = 0;
		isMoving = false;

		if (Input.GetKey(KeyCode.RightArrow))
		{
			if (!isFacingWall)
			{
				moveInput = 1;
				isMoving = true;
			}
			if (!isFacingRight)
				Flip();
		}

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			if (!isFacingWall)
			{
				moveInput = -1;
				isMoving = true;
			}
			if (isFacingRight)
				Flip();
		}
	}
	private void updateMovement()
    {
		float moveInput;
		updateMovementControlls(out moveInput);

		// smooth movement
		float targetSpeed = moveInput * moveSpeed;
        float speedDif = targetSpeed - rigidBody.velocity.x;
        float accelRate = moveInput == 0 ? decceleration : acceleration;
        float movement = speedDif * accelRate;

        rigidBody.AddForce(movement * Vector2.right);

		// faster stopping
        if (isGrounded && moveInput == 0)
        {
            float frictionAmount = Mathf.Min(Mathf.Abs(rigidBody.velocity.x), frictionCoeff);
            frictionAmount *= Mathf.Sign(rigidBody.velocity.x);

			rigidBody.AddForce(-frictionAmount * Vector2.right, ForceMode2D.Impulse);
		}
	}
    
	private void updateJumpQol()
    {
		// jump cut
		if (!isGrounded && !Input.GetKey(KeyCode.Z) && rigidBody.velocity.y > 0)
		{
			rigidBody.AddForce(rigidBody.velocity.y * (1 - jumpCutMultiplier) * Vector2.down,
				ForceMode2D.Impulse);
		}

		// gravity scaling
		if (!isGrounded && Mathf.Abs(rigidBody.velocity.y) < jumpHangVelThreshold)
		{
			rigidBody.gravityScale = jumpHangGravityMultiplier;
		}
		else if (!isGrounded && rigidBody.velocity.y < 0)
		{
			rigidBody.gravityScale = fallGravityMultiplier;
		}
		else
		{
			rigidBody.gravityScale = 1.0f;
		}
	}
    private void updateJump()
    {
		if (isGrounded)
		{
			jumpsSinceGrounded = 0;
		}

		if (Input.GetKey(KeyCode.Z) && lastJumpInputTime == 0)
			lastJumpInputTime = Time.time;
		if (!Input.GetKey(KeyCode.Z) && Time.time - lastJumpInputTime > jumpTimeThreshold)
			lastJumpInputTime = 0;

		if (Time.time - lastJumpInputTime <= jumpTimeThreshold)
			if (isGrounded && Time.time - lastJumpTime > jumpCooldown)
			{
				jumpsSinceGrounded = 1;
				lastJumpTime = Time.time;
				rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
				rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
			}

		updateJumpQol();
	}

    private void Flip()
    {
        isFacingRight = !isFacingRight;
		Vector3 theScale = transform.localScale;
        theScale.x = -theScale.x;
        transform.localScale = theScale;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Bonus"))
		{
            score += 1;
            Debug.Log("Score: " + score);

            other.gameObject.SetActive(false);
		}
	}

	private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        groundCheck = transform.Find("Ground Check").GetComponent<BoxCollider2D>();
        wallCheck = transform.Find("Wall Check").GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		// tmp for debugging
        if (Input.GetKey(KeyCode.X))
            Time.timeScale = 0.05f;
        else
			Time.timeScale = 1.0f;

		UpdateCollisions();
        updateMovement();
        updateJump();

		animator.SetBool("isGrounded", isGrounded);
		animator.SetBool("isFalling", rigidBody.velocity.y < 0);
		animator.SetBool("isMoving", isMoving);
	}
}
