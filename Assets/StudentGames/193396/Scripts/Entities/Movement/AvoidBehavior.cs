using UnityEngine;

[RequireComponent(typeof(FlipBehavior))]
public class AvoidBehavior : EntityBehavior
{
	public AvoidBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isAvoiding { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public Vector2 avoidVec { get; private set; }


	public override void onAwake()
	{
	}

	public override void onUpdate()
	{
		isAvoiding = findAvoidVector();
	}


	private bool findAvoidVector()
	{
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, data.distanceBig, data.avoidLayers);

		Vector2 closestPoint = Vector2.zero;
		float minDist = float.PositiveInfinity;

		foreach (var collider in colliders)
		{
			Vector2 colliderClosestPoint = collider.ClosestPoint(transform.position);
			float dist = Vector2.Distance(transform.position, colliderClosestPoint);

			if (dist < minDist)
			{
				minDist = dist;
				closestPoint = colliderClosestPoint;
			}
		}

		if (minDist == float.PositiveInfinity)
			return false;

		Vector2 dir = (closestPoint - (Vector2)transform.position).normalized * data.strength;

		if (minDist <= data.distanceSmall)
			avoidVec = -dir;
		else
			avoidVec = -dir * (data.distanceBig - minDist) / (data.distanceBig - data.distanceSmall);

		return true;
	}
}
