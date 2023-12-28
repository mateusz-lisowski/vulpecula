using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(FlipBehavior))]
	[RequireComponent(typeof(GroundedBehavior))]
	public class JumpBehavior : EntityBehavior
	{
		private const float deltaTime = 0.1f;

		public JumpBehaviorData data;
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
			return jumpCooldown <= 0 && ground.isGrounded;
		}
		private void jump()
		{
			jumpCooldown = data.cooldown;

			float height, targetJumpSpeed;
			if (findTarget(out height, out targetJumpSpeed))
			{
				isJumping = true;
				jumpSpeed = targetJumpSpeed;
				float force = Mathf.Sqrt(2 * -Physics2D.gravity.y * height);

				if (force > controller.rigidBody.velocity.y)
				{
					force -= controller.rigidBody.velocity.y;
					controller.rigidBody.AddForce(force * Vector2.up, ForceMode2D.Impulse);
				}

				controller.onEvent("jumped", null);
			}
			else
				direction.flip();
		}
		private void updateJump()
		{
			if (ground.isGrounded)
				isJumping = false;

			if (canJump())
			{
				jump();
			}

			if (isJumping && !ground.isFalling)
				ground.disableGroundedNextFrame();
		}

		private Bounds hitboxBounds()
		{
			Collider2D[] colliders = controller.hitbox.GetComponents<Collider2D>();

			Bounds b = colliders[0].bounds;
			foreach (Collider2D collider in colliders)
				b.Encapsulate(collider.bounds);

			b.min -= transform.position;
			b.max -= transform.position;

			return b;
		}
		private bool tryFindTarget(Vector2 position, Bounds bounds, float height, float speed)
		{
			Vector2 velocity = new Vector2(speed * transform.right.x, Mathf.Sqrt(2.0f * -Physics2D.gravity.y * height));
			Vector2 acceleration = Physics2D.gravity;

			Vector2 currentPosition = new Vector2(position.x, position.y);
			Vector2 nextPosition;

			float currentTime = deltaTime;
			float riseTime = -velocity.y / acceleration.y;

			RaycastHit2D hit;

			while (true)
			{
				nextPosition = position + (velocity + 0.5f * acceleration * currentTime) * currentTime;

				Vector2 dir = nextPosition - currentPosition;
				bool isRising = currentTime - deltaTime <= riseTime;

				Vector2 hitboxSize = bounds.size;
				if (currentPosition != position)
					hitboxSize.y += 0.5f;
				
				hit = Physics2D.BoxCast(currentPosition, hitboxSize, 0f,
					dir.normalized, dir.magnitude,
					isRising ? ground.data.wallLayers : ground.data.groundLayers);
				if (hit)
					break;

				currentPosition = nextPosition;
				currentTime += deltaTime;

				if (currentPosition.y < position.y - data.maxFall)
					return false;
			}

			if (hit.normal.y > 0.4f)
				return true;
			else
				return false;
		}
		private bool findTarget(out float height, out float speed)
		{
			Bounds bounds = hitboxBounds();

			int times = Mathf.RoundToInt((data.longHeight - data.shortHeight) / 0.5f) + 1;

			for (int i = 0; i < times; i++)
			{
				float lerp = times > 1 ? i / (float)(times - 1) : 0;

				speed = Mathf.Lerp(data.longSpeed, data.shortSpeed, lerp);
				height = Mathf.Lerp(data.longHeight, data.shortHeight, lerp);

				if (tryFindTarget(transform.position + bounds.center, bounds, height, speed))
					return true;
			}

			height = 0f;
			speed = 0f;
			return false;
		}

		private void drawParabola(Vector2 position, Vector2 velocity, Vector2 acceleration, float time)
		{
			Vector2 startPosition = position;

			for (float t = deltaTime; t < time + deltaTime / 2; t += deltaTime)
			{
				Vector2 nextPosition = startPosition + (velocity + 0.5f * acceleration * t) * t;

				Debug.DrawLine(position, nextPosition, Color.red, data.cooldown);

				position = nextPosition;
			}
		}
	}
}