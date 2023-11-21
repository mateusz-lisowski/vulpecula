using UnityEngine;

[RequireComponent(typeof(GroundedBehavior))]
public class JumpBehavior : EntityBehavior
{
	public JumpBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isJumping { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public float jumpCooldown { get; private set; }

	private FlipBehavior direction;
	private GroundedBehavior ground;


	public override void onAwake()
	{
		direction = controller.getBehavior<FlipBehavior>();
		ground = controller.getBehavior<GroundedBehavior>();
	}

	public override void onUpdate()
	{
		jumpCooldown -= Time.deltaTime;

		updateJump();

		foreach (var param in controller.animator.parameters)
			if (param.name == "isJumping")
				controller.animator.SetBool("isJumping", isJumping);
	}

	public override bool onFixedUpdate()
	{
		addSmoothForce(isJumping ? data.jumpSpeed : 0f, 1f, transform.right);

		return true;
	}


	private bool canJump()
	{
		return jumpCooldown <= 0 && ground.isGrounded;
	}
	private void jump()
	{
		isJumping = true;
		jumpCooldown = data.cooldown;

		float force = data.force;

		if (force > controller.rigidBody.velocity.y)
		{
			force -= controller.rigidBody.velocity.y;
			controller.rigidBody.AddForce(force * Vector2.up, ForceMode2D.Impulse);
		}
	}
	private void updateJump()
	{
		if (ground.isGrounded)
		{
			isJumping = false;
		}

		if (canJump())
		{
			jump();
		}

		if (isJumping && !ground.isFalling)
			ground.disableGroundedNextFrame();
	}

}
