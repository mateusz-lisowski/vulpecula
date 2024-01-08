using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(FlipBehavior))]
	public class ChaseBehavior : EntityBehavior
	{
		public ChaseBehaviorData data;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isChasing { get; private set; }
		[field: Space(5)]
		[field: SerializeField, ReadOnly] public float lastSeenTime { get; private set; }
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public Vector2 lastTargetPosition { get; private set; }

		private FlipBehavior direction;
		private GroundedBehavior ground;


		public override void onAwake()
		{
			direction = controller.getBehavior<FlipBehavior>();
			ground = controller.getBehavior<GroundedBehavior>();

			lastSeenTime = float.PositiveInfinity;
		}

		public override void onUpdate()
		{
			bool wasChasing = isChasing;

			lastSeenTime += Time.deltaTime;

			Vector2 targetPosition;
			bool caught;
			isChasing = tryFindTarget(out targetPosition, out caught);

			if (isChasing)
				lastSeenTime = 0f;

			if (!isChasing && caught && data.canCatch)
				lastSeenTime = data.determinationTime;

			if (!isChasing && lastSeenTime < data.determinationTime)
				isChasing = true;

			if (isChasing)
			{
				lastTargetPosition = targetPosition;

				Debug.DrawLine(transform.position, targetPosition, Color.red);

				if (ground == null || ground.isGrounded)
					direction.faceTowards(targetPosition);
			}
		}


		private bool tryFindTarget(out Vector2 targetPosition, out bool caught)
		{
			Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(
				transform.position, data.maxDistance, data.targetLayers);

			caught = false;

			foreach (Collider2D targetInRange in targetsInRange)
			{
				Vector2 position = targetInRange.attachedRigidbody.position;
				Vector2 targetDir = position - (Vector2)transform.position;

				float distance = targetDir.magnitude;

				if (distance < data.minDistance)
					caught = true;

				if (distance > data.maxDistance || distance < data.minDistance)
					continue;

				if (Physics2D.Raycast(transform.position, targetDir.normalized, distance, data.obstructLayers))
					continue;

				targetPosition = position;
				return true;
			}

			targetPosition = lastTargetPosition;
			return false;
		}
	}
}