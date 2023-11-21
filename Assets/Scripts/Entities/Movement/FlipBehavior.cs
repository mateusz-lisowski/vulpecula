using UnityEngine;

public class FlipBehavior : EntityBehavior
{
	public FlipBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isFacingRight { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastTurnTime { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] private int lastTurnFixedUpdate = -1;


	public void flip()
	{
		lastTurnTime = 0;
		lastTurnFixedUpdate = controller.currentFixedUpdate;

		isFacingRight = !isFacingRight;
		controller.transform.Rotate(0, 180, 0);
	}
	public bool isPhysicsNotUpdatedAfterFlip()
	{
		return lastTurnFixedUpdate >= controller.currentFixedUpdate - 1;
	}

	public override void onAwake()
	{
		isFacingRight = controller.transform.right.x > 0;

		lastTurnTime = float.PositiveInfinity;
	}

	public override void onUpdate()
	{
		lastTurnTime += Time.deltaTime;
	}
}
