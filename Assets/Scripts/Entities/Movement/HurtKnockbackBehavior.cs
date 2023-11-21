using UnityEngine;

[RequireComponent(typeof(HurtBehavior))]
public class HurtKnockbackBehavior : EntityBehavior
{
	public HurtKnockbackBehaviorData data;

	private HurtBehavior hurt;


	public override void onAwake()
	{
		hurt = controller.GetComponent<HurtBehavior>();
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
	}

	public override bool onFixedUpdate()
	{
		if (!hurt.isDistressed)
			return false;

		addSmoothForce(data.knockbackSpeed, 1f, -transform.right);

		return true;
	}
}
