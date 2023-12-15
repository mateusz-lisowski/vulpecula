using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(FlipBehavior))]
	public class FlyBehavior : EntityBehavior
	{
		public FlyBehaviorData data;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public Vector2 targetVelocity { get; private set; }

		private FlipBehavior direction;
		private ChaseBehavior chase;
		private SleepBehavior sleep;
		private List<AvoidBehavior> avoids;


		public override void onAwake()
		{
			direction = controller.getBehavior<FlipBehavior>();
			chase = controller.getBehavior<ChaseBehavior>();
			sleep = controller.getBehavior<SleepBehavior>();
			avoids = controller.getBehaviors<AvoidBehavior>();
		}

		public override void onUpdate()
		{
		}

		public override bool onFixedUpdate()
		{
			targetVelocity = calculateTargetVelocity();

			if (targetVelocity == Vector2.zero)
				return true;

			addSmoothForce(targetVelocity.magnitude, data.accelerationCoefficient, targetVelocity.normalized);

			return true;
		}


		private Vector2 findAverageVector(List<Vector2> vectors)
		{
			Vector2 targetVector = Vector2.zero;
			if (vectors.Count == 0)
				return targetVector;

			float maxMagnitude = 0f;

			foreach (var vector in vectors)
			{
				float magnitude = vector.magnitude;

				if (magnitude > maxMagnitude)
					maxMagnitude = magnitude;

				targetVector += vector;
			}

			if (targetVector == Vector2.zero)
				return targetVector;

			targetVector = targetVector.normalized * maxMagnitude;

			return targetVector;
		}
		private Vector2 calculateTargetVelocity()
		{
			bool isChasing = chase != null && chase.isChasing;
			bool isReturning = sleep != null && !isChasing;

			List<Vector2> vectors = new List<Vector2>();

			foreach (var avoid in avoids)
				if (avoid.isAvoiding)
					vectors.Add(avoid.avoidVec);

			if (isChasing)
				vectors.Add((chase.lastTargetPosition - (Vector2)transform.position).normalized * data.flySpeed);
			else if (isReturning)
				vectors.Add((sleep.sleepPosition - (Vector2)transform.position).normalized * data.flySpeed);

			Vector2 targetVelocity = findAverageVector(vectors);

			if (!isChasing && !isReturning)
				targetVelocity += Vector2.down * data.fallSpeed;

			return targetVelocity;
		}

	}
}