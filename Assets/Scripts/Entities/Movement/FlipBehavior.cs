using UnityEngine;

public class FlipBehavior : EntityBehavior
{
	public FlipBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isFacingRight { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float lastTurnTime { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public int lastTurnFrame { get; private set; }

	public void flip()
	{
		lastTurnTime = 0;
		lastTurnFrame = controller.currentFrame;

		isFacingRight = !isFacingRight;
		controller.transform.Rotate(0, 180, 0);
	}

	public override void onAwake()
	{
		isFacingRight = controller.transform.right.x > 0;

		lastTurnTime = float.PositiveInfinity;

		lastTurnFrame = -1;
	}

	public override void onEvent(string eventName, object eventData)
	{
	}

	public override void onUpdate()
	{
		lastTurnTime += Time.deltaTime;
	}

	public override void onFixedUpdate()
	{
	}
}
