using UnityEngine;

public class GroundedBehavior : EntityBehavior
{
	public GroundBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isGrounded { get; private set; }
	[field: SerializeField, ReadOnly] public bool isFalling { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] private int lastDisabledUpdate = -1;

	private Collider2D groundCheck;


	public void disableCurrentFrame()
	{
		lastDisabledUpdate = controller.currentUpdate;
	}

	public override void onAwake()
	{
		groundCheck = transform.Find("Ground Check").GetComponent<Collider2D>();
	}

	public override void onUpdate()
	{
		isGrounded = lastDisabledUpdate != controller.currentUpdate 
			&& groundCheck.IsTouchingLayers(data.groundLayers);

		isFalling = !isGrounded && controller.rigidBody.velocity.y <= 0;

		foreach (var param in controller.animator.parameters)
			if (param.name == "isGrounded")
				controller.animator.SetBool("isGrounded", isGrounded);
			else if (param.name == "isFalling")
				controller.animator.SetBool("isFalling", isFalling);
	}
}
