using UnityEngine;

public class RunBehavior : EntityBehavior
{
	public RunBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isMoving { get; private set; }
	[field: SerializeField, ReadOnly] public bool isFalling { get; private set; }
	[field: SerializeField, ReadOnly] public bool isGrounded { get; set; }
	[field: SerializeField, ReadOnly] public bool isFacingWall { get; private set; }
	[field: SerializeField, ReadOnly] public bool isNoGroundAhead { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastGroundedTime { get; private set; }

	private FlipBehavior direction;

	private Collider2D groundCheck;
	private Collider2D wallCheck;
	private Collider2D fallCheck;

	private float moveInput = 0.0f;


	public override void onAwake()
	{
		direction = controller.getBehavior<FlipBehavior>();

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

		updateFalling();

		foreach (var param in controller.animator.parameters)
			if (param.name == "isGrounded")
				controller.animator.SetBool("isGrounded", isGrounded);
			else if (param.name == "isFalling")
				controller.animator.SetBool("isFalling", controller.rigidBody.velocity.y < 0);
			else if (param.name == "isMoving")
				controller.animator.SetBool("isMoving", isMoving);
	}

	public override bool onFixedUpdate()
	{
		addSmoothForce(moveInput * data.maxSpeed, data.accelerationCoefficient, Vector2.right);

		return true;
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
		if (direction.lastTurnFixedUpdate >= controller.currentFixedUpdate - 1)
		{
			isFacingWall = false;
			isNoGroundAhead = false;
		}

		if (isFacingWall)
			isMoving = false;
	}

	private void updateInputs()
	{
		if (isFacingWall || (isNoGroundAhead && isGrounded))
			direction.flip();

		if (direction.isFacingRight)
			moveInput = 1;
		else
			moveInput = -1;

		isMoving = moveInput != 0;
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