using UnityEngine;

public class GroundedBehavior : EntityBehavior
{
	public GroundBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isGrounded { get; private set; }
	[field: SerializeField, ReadOnly] public bool isFalling { get; private set; }
	[field: SerializeField, ReadOnly] public bool isOnSlope { get; private set; }
	[field: SerializeField, ReadOnly] public bool isSlopeGrounded { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] private int lastDisabledUpdate = -1;

	private Collider2D groundCheck;


	public void disableGroundedThisFrame()
	{
		lastDisabledUpdate = controller.currentUpdate;
	}
	public void disableGroundedNextFrame()
	{
		lastDisabledUpdate = controller.currentUpdate + 1;
	}

	public bool tryStopSlopeFixedUpdate()
	{
		if (isOnSlope && isSlopeGrounded)
		{
			controller.rigidBody.AddForce(-Physics2D.gravity, ForceMode2D.Force);
			return true;
		}
		else
			return false;
	}

	public override void onAwake()
	{
		groundCheck = transform.Find("Ground Check").GetComponent<Collider2D>();
	}

	public override void onUpdate()
	{
		if (lastDisabledUpdate < controller.currentUpdate)
		{ 
			isGrounded = groundCheck.IsTouchingLayers(data.groundLayers);
			isSlopeGrounded = groundCheck.IsTouchingLayers(data.groundLayers & ~data.passingLayers);
			isOnSlope = groundCheck.IsTouchingLayers(data.slopeLayer);
		}
		else
		{
			isGrounded = false;
			isSlopeGrounded = false;
			isOnSlope = false;
		}

		bool wasFalling = isFalling;
		isFalling = !isGrounded && controller.rigidBody.velocity.y <= 0;

		foreach (var param in controller.animator.parameters)
			if (param.name == "isGrounded")
				controller.animator.SetBool("isGrounded", isGrounded);
			else if (param.name == "isFalling")
				controller.animator.SetBool("isFalling", isFalling);

		if (wasFalling && !isFalling)
			controller.onEvent("fell", null);
	}
}
