using UnityEngine;

[RequireComponent(typeof(GroundedBehavior))]
public class JumpBehavior : EntityBehavior
{
	public JumpBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isJumping { get; private set; }

	private FlipBehavior direction;
	private GroundedBehavior ground;


	public override void onAwake()
	{
		direction = controller.getBehavior<FlipBehavior>();
		ground = controller.getBehavior<GroundedBehavior>();
	}

	public override void onUpdate()
	{
	}

	public override bool onFixedUpdate()
	{
		return false;
	}
}
