using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(FlipBehavior))]
public class FlyBehavior : EntityBehavior
{
	public FlyBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isDisturbed { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public Vector2 disturbVec { get; private set; }

	private FlipBehavior direction;
	private ChaseBehavior chase;


	public override void onAwake()
	{
		direction = controller.getBehavior<FlipBehavior>();
		chase = controller.getBehavior<ChaseBehavior>();
	}

	public override void onUpdate()
	{
	}

	public override bool onFixedUpdate()
	{
		Vector2 targetVelocity = Vector2.zero;

		isDisturbed = calculateAverageDisturb();
		bool isChasing = chase != null && chase.isChasing;

		if (isDisturbed)
			targetVelocity -= disturbVec * data.avoidSpeed;

		if (isChasing)
		{
			Vector2 chaseVelocity = (chase.lastTargetPosition - (Vector2)transform.position).normalized * data.flySpeed;
			targetVelocity = targetVelocity + chaseVelocity;

			if (targetVelocity.magnitude < chaseVelocity.magnitude)
				targetVelocity = targetVelocity.normalized * chaseVelocity.magnitude;
		}

		if ((isDisturbed || !isChasing) && targetVelocity.y >= 0f)
			targetVelocity += Vector2.down * data.fallSpeed;

		if (targetVelocity == Vector2.zero)
			return true;

		addSmoothForce(targetVelocity.magnitude, data.accelerationCoefficient, targetVelocity.normalized);

		return true;
	}


	private bool calculateAverageDisturb()
	{
		float minDist = float.PositiveInfinity;

		int rotationCount = 30;
		for (int i = 0; i < rotationCount; i++)
		{
			float delta = 2 * i * Mathf.PI  / rotationCount;
			Vector2 dir = new Vector2(-Mathf.Sin(delta), Mathf.Cos(delta));

			var hit = Physics2D.Raycast(transform.position, dir, data.safeSpaceBig, data.avoidLayers);

			if (!hit)
				continue;

			Vector2 vec = hit.point - (Vector2)transform.position;
			float dist = vec.magnitude;

			if (dist == 0)
				continue;

			if (dist < minDist)
			{
				minDist = dist;
				disturbVec = vec;
			}
		}

		if (minDist == float.PositiveInfinity)
			return false;

		if (minDist <= data.safeSpaceSmall)
			disturbVec = disturbVec.normalized;
		else
			disturbVec = disturbVec.normalized * (data.safeSpaceBig - minDist) 
				/ (data.safeSpaceBig - data.safeSpaceSmall);

		return true;
	}

}