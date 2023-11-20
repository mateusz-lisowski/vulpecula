using UnityEngine;

public class RunBehavior : EntityBehavior
{
	public RunBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isMoving { get; private set; }
	[field: SerializeField, ReadOnly] public bool isFalling { get; private set; }
	[field: SerializeField, ReadOnly] public bool isGrounded { get; private set; }
	[field: SerializeField, ReadOnly] public bool isFacingWall { get; private set; }
	[field: SerializeField, ReadOnly] public bool isNoGroundAhead { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastGroundedTime { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool currentFrameStopping;

	private FlipBehavior direction;
	private HurtBehavior hurt;

	private Collider2D groundCheck;
	private Collider2D wallCheck;
	private Collider2D fallCheck;

	private Vector2 moveInput;


	public override void onAwake()
	{
		direction = controller.getBehavior<FlipBehavior>();
		hurt = controller.getBehavior<HurtBehavior>();

		groundCheck = transform.Find("Ground Check").GetComponent<Collider2D>();
		wallCheck = transform.Find("Wall Check").GetComponent<Collider2D>();
		fallCheck = transform.Find("Fall Check").GetComponent<Collider2D>();

		lastGroundedTime = float.PositiveInfinity;
	}

	public override void onEvent(string eventName, object eventData)
	{
	}

	public override void onUpdate()
	{
		lastGroundedTime += Time.deltaTime;

		updateCollisions();
		updateInputs();

		updateHurt();
		updateFalling();

		foreach (var param in controller.animator.parameters)
			if (param.name == "isGrounded")
				controller.animator.SetBool("isGrounded", isGrounded);
			else if (param.name == "isFalling")
				controller.animator.SetBool("isFalling", controller.rigidBody.velocity.y < 0);
			else if (param.name == "isMoving")
				controller.animator.SetBool("isMoving", isMoving);
	}

	public override void onFixedUpdate()
	{
		float targetSpeed = moveInput.x * data.maxSpeed;

		if (hurt != null && hurt.isDistressed)
			targetSpeed = moveInput.x * data.knockbackMaxSpeed;

		if (currentFrameStopping)
			targetSpeed = 0;
		currentFrameStopping = false;

		float accelRate = targetSpeed == 0 ? data.decelerationForce : data.accelerationForce;

		float speedDif = targetSpeed - controller.rigidBody.velocity.x;
		float movement = speedDif * accelRate;

		controller.rigidBody.AddForce(movement * Vector2.right, ForceMode2D.Force);
	}


	private bool isFacingSlope()
	{
		Vector2 sourceLow = new Vector2(transform.position.x, transform.position.y - 0.2f);
		Vector2 sourceHigh = new Vector2(transform.position.x, transform.position.y + 0.2f);

		Vector2 dir = direction.isFacingRight ? Vector2.right : Vector2.left;

		RaycastHit2D hitLow = Physics2D.Raycast(sourceLow, dir, Mathf.Infinity, data.groundLayers);
		RaycastHit2D hitHigh = Physics2D.Raycast(sourceHigh, dir, Mathf.Infinity, data.groundLayers);

		return Mathf.Abs(hitHigh.point.x - hitLow.point.x) > 0.1f;
	}
	private void updateCollisions()
	{
		isGrounded = groundCheck.IsTouchingLayers(data.groundLayers);
		isFacingWall = wallCheck.IsTouchingLayers(data.wallLayers);
		isNoGroundAhead = !fallCheck.IsTouchingLayers(data.groundLayers);

		if (isFacingWall && isFacingSlope())
			isFacingWall = false;

		// disable registering wall/fall collision immediately after turning because wallCheck and
		// fallCheck hitboxes need time to get updated
		if (direction.lastTurnFrame >= controller.currentFrame - 1)
		{
			isFacingWall = false;
			isNoGroundAhead = false;
		}

		if (isFacingWall)
			isMoving = false;
	}

	private void updateInputs()
	{
		moveInput = new Vector2();

		if (isFacingWall || (isNoGroundAhead && isGrounded))
			direction.flip();

		if (direction.isFacingRight)
			moveInput.x += 1;
		else
			moveInput.x += -1;

		isMoving = moveInput.x != 0;
	}

	private void updateHurt()
	{
		if (hurt == null)
			return;

		if (hurt.lastHurtTime == 0) 
		{
			float force = data.knockbackForce;
			if (force > controller.rigidBody.velocity.y)
			{
				force -= controller.rigidBody.velocity.y;
				controller.rigidBody.AddForce(force * Vector2.up, ForceMode2D.Impulse);
			}
		}

		if (hurt.isDistressed)
		{
			isGrounded = false;

			moveInput.y = 0;
			moveInput.x = direction.isFacingRight ? -1 : 1;
		}
	}
	private void updateFalling()
	{
		if (!isGrounded && controller.rigidBody.velocity.y <= 0)
		{
			isFalling = true;
		}

		if (isGrounded)
		{
			isFalling = false;
			lastGroundedTime = 0;
		}
	}
}