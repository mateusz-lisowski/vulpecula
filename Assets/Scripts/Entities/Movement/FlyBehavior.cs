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
	[field: SerializeField, ReadOnly] public Vector2 targetVelocity { get; private set; }

	private FlipBehavior direction;
	private ChaseBehavior chase;
	private SleepBehavior sleep;


	public override void onAwake()
	{
		direction = controller.getBehavior<FlipBehavior>();
		chase = controller.getBehavior<ChaseBehavior>();
		sleep = controller.getBehavior<SleepBehavior>();
	}

	public override void onUpdate()
	{
		isDisturbed = calculateDisturb();

		direction.faceTowards((Vector2)transform.position + targetVelocity);
	}

	public override bool onFixedUpdate()
	{
		targetVelocity = calculateTargetVelocity();

		if (targetVelocity == Vector2.zero)
			return true;

		addSmoothForce(targetVelocity.magnitude, data.accelerationCoefficient, targetVelocity.normalized);

		return true;
	}


	public Vector2 calculateTargetVelocity()
	{
		Vector2 targetVelocity = Vector2.zero;

		bool isChasing = chase != null && chase.isChasing;
		bool isReturning = sleep != null && !isChasing;

		if (isDisturbed)
			targetVelocity -= disturbVec * data.avoidSpeed;

		if (isChasing)
		{
			Vector2 chaseVelocity = (chase.lastTargetPosition - (Vector2)transform.position).normalized * data.flySpeed;
			targetVelocity = targetVelocity + chaseVelocity;

			if (targetVelocity.magnitude < chaseVelocity.magnitude)
				targetVelocity = targetVelocity.normalized * chaseVelocity.magnitude;
		}
		else if (isReturning)
		{
			Vector2 returnVelocity = (sleep.sleepPosition - (Vector2)transform.position).normalized * data.flySpeed;
			targetVelocity = targetVelocity + returnVelocity;

			if (targetVelocity.magnitude < returnVelocity.magnitude)
				targetVelocity = targetVelocity.normalized * returnVelocity.magnitude;
		}

		if ((isDisturbed || (!isChasing && !isReturning)) && targetVelocity.y >= 0f)
			targetVelocity += Vector2.down * data.fallSpeed;

		return targetVelocity;
	}

	private bool calculateDisturb()
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