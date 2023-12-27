using UnityEngine;

namespace _193396
{
	public class FlipBehavior : EntityBehavior
	{
		public FlipBehaviorData data;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isFacingRight { get; private set; }
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public float turnCooldown { get; private set; }
		[field: Space(10)]
		[field: SerializeField, ReadOnly] private int lastDisabledUpdate = -1;
		[field: SerializeField, ReadOnly] private int lastTurnFixedUpdate = -1;


		public void disableFlipNextFrame()
		{
			lastDisabledUpdate = controller.currentUpdate + 1;
		}

		public void flip()
		{
			if (turnCooldown > 0 || lastDisabledUpdate >= controller.currentUpdate)
				return;

			turnCooldown = data.cooldown;
			lastTurnFixedUpdate = controller.currentFixedUpdate;

			isFacingRight = !isFacingRight;
			controller.transform.Rotate(0, 180, 0);

			controller.onEvent("flipped", null);
		}
		public void faceTowards(Vector2 point)
		{
			Vector2 dir = point - (Vector2)transform.position;

			if ((isFacingRight && dir.x < 0) || (!isFacingRight && dir.x > 0))
				flip();
		}
		public bool isPhysicsNotUpdatedAfterFlip()
		{
			return lastTurnFixedUpdate >= controller.currentFixedUpdate - 1;
		}

		public override void onStart()
		{
			isFacingRight = controller.transform.right.x > 0;
		}

		public override void onUpdate()
		{
			turnCooldown -= Time.deltaTime;
		}
	}
}