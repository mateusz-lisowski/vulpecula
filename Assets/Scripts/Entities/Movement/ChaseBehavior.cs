using UnityEngine;

public class ChaseBehavior : EntityBehavior
{
	public ChaseBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isChasing { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float targetDistance { get; private set; }
	[field: SerializeField, ReadOnly] public Vector2 targetDirection { get; private set; }

	private FlipBehavior direction;
	private GroundedBehavior ground;


	public override void onAwake()
	{
		direction = controller.getBehavior<FlipBehavior>();
		ground = controller.getBehavior<GroundedBehavior>();
	}

	public override void onUpdate()
	{
		Vector2 targetPosition;
		isChasing = tryFindTarget(out targetPosition);

		if (isChasing)
		{
			Vector2 targetDir = targetPosition - (Vector2)transform.position;

			targetDistance = targetDir.magnitude;
			targetDirection = targetDir.normalized;

			bool targetOtherDirection = (direction.isFacingRight && targetDirection.x < 0)
				|| (!direction.isFacingRight && targetDirection.x > 0);

			Debug.DrawLine(transform.position, targetPosition, Color.red);

			if (targetOtherDirection && ground.isGrounded)
				direction.flip();
		}
	}


	private bool tryFindTarget(out Vector2 targetPosition)
	{
		Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(
			transform.position, data.maxDistance, data.targetLayers);

		foreach (Collider2D targetInRange in targetsInRange)
		{
			Vector2 position = targetInRange.attachedRigidbody.position;
			Vector2 targetDir = position - (Vector2)transform.position;

			float distance = targetDir.magnitude;

			if (distance > data.maxDistance || distance < data.minDistance)
				continue;

			if (Physics2D.Raycast(transform.position, targetDir.normalized, distance, ground.data.groundLayers))
				continue;

			targetPosition = position;
			return true;
		}

		targetPosition = Vector2.zero;
		return false;
	}
}
