using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(FlipBehavior))]
	[RequireComponent(typeof(GroundedBehavior))]
	public class RunBehavior : EntityBehavior
	{
		public RunBehaviorData data;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isFacingWall { get; private set; }
		[field: SerializeField, ReadOnly] public bool isNoGroundAhead { get; private set; }

		private FlipBehavior direction;
		private GroundedBehavior ground;

		private Collider2D wallCheck;
		private Collider2D fallCheck;


		public override void onAwake()
		{
			direction = controller.getBehavior<FlipBehavior>();
			ground = controller.getBehavior<GroundedBehavior>();

			wallCheck = transform.Find("Detection/Wall").GetComponent<Collider2D>();
			fallCheck = transform.Find("Detection/Fall").GetComponent<Collider2D>();
		}

		public override void onUpdate()
		{
			updateCollisions();
			updateInputs();
		}

		public override bool onFixedUpdate()
		{
			addSmoothForce(data.maxSpeed, data.accelerationCoefficient, transform.right);

			return true;
		}


		private bool isFacingSlope()
		{
			Vector2 sourceLow = new Vector2(transform.position.x, transform.position.y - 0.2f);
			Vector2 sourceHigh = new Vector2(transform.position.x, transform.position.y + 0.2f);

			Vector2 dir = direction.isFacingRight ? Vector2.right : Vector2.left;

			RaycastHit2D hitLow = Physics2D.Raycast(sourceLow, dir, Mathf.Infinity, ground.data.groundLayers);
			RaycastHit2D hitHigh = Physics2D.Raycast(sourceHigh, dir, Mathf.Infinity, ground.data.groundLayers);

			return Mathf.Abs(hitHigh.point.x - hitLow.point.x) > 0.1f;
		}
		private void updateCollisions()
		{
			isFacingWall = wallCheck.IsTouchingLayers(ground.data.wallLayers);
			isNoGroundAhead = !fallCheck.IsTouchingLayers(ground.data.groundLayers);

			if (isFacingWall && isFacingSlope())
				isFacingWall = false;

			if (direction.isPhysicsNotUpdatedAfterFlip())
			{
				isFacingWall = false;
				isNoGroundAhead = false;
			}
		}

		private void updateInputs()
		{
			if (isFacingWall || (isNoGroundAhead && ground.isGrounded))
				direction.flip();
		}
	}
}