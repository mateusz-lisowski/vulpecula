using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(HurtBehavior))]
	public class HurtKnockbackBehavior : EntityBehavior
	{
		public HurtKnockbackBehaviorData data;

		private HurtBehavior hurt;
		private GroundedBehavior ground;


		public override void onAwake()
		{
			hurt = controller.GetComponent<HurtBehavior>();
			ground = controller.GetComponent<GroundedBehavior>();
		}

		public override void onUpdate()
		{
			if (hurt.lastHurtTime == 0)
			{
				float force = data.knockbackForce;
				if (force > controller.rigidBody.velocity.y)
				{
					force -= controller.rigidBody.velocity.y;
					controller.rigidBody.AddForce(force * Vector2.up, ForceMode2D.Impulse);
				}
			}

			if (ground != null && hurt.isDistressed)
				ground.disableGroundedThisFrame();

		}

		public override bool onFixedUpdate()
		{
			if (!hurt.isDistressed)
				return false;

			addSmoothForce(data.knockbackSpeed, 1f, -transform.right);

			return true;
		}
	}
}