using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(FlipBehavior))]
	[RequireComponent(typeof(GroundedBehavior))]
	public class JumpAtBehavior : EntityBehavior
	{
		public JumpAtBehaviorData data;
		public Transform target;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isJumping { get; private set; }
		[field: Space(5)]
		[field: SerializeField, ReadOnly] public float jumpCooldown { get; private set; }
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public float jumpSpeed { get; private set; }

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
		}

		public override bool onFixedUpdate()
		{
			if (!isJumping)
				ground.tryStopSlopeFixedUpdate();

			addSmoothForce(isJumping ? jumpSpeed : 0f, 1f, transform.right);

			return true;
		}


		private bool canJump()
		{
			return jumpCooldown <= 0 && ground.isGrounded && target != null;
		}
		private void jump()
		{
			isJumping = true;
			jumpCooldown = data.cooldown;

			direction.faceTowards(target.position);

			float xt = Mathf.Abs(target.position.x - transform.position.x);
			float yt = target.position.y - transform.position.y;

			float d = Mathf.Max(4 * data.eccentricity * xt - yt, 0);
			float distance = Mathf.Min((4 * data.eccentricity * xt * xt) / d, data.maxDistance);

			float height = distance * data.eccentricity;
			float force = Mathf.Sqrt(2 * -Physics2D.gravity.y * height);

			float t = Mathf.Sqrt(2 * (height - yt) / -Physics2D.gravity.y);
			t *= xt / (xt - 0.5f * distance);
			jumpSpeed = distance / t;

			if (force > controller.rigidBody.velocity.y)
			{
				force -= controller.rigidBody.velocity.y;
				controller.rigidBody.AddForce(force * Vector2.up, ForceMode2D.Impulse);
			}

			controller.onEvent("jumped", null);
		}
		private void updateJump()
		{
			if (ground.isGrounded)
			{
				if (isJumping)
				{
					if (Vector2.Dot(transform.right, target.right) < -0.01f)
						direction.flip();

					if (data.autoResetTarget)
						target = null;
				}

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
}