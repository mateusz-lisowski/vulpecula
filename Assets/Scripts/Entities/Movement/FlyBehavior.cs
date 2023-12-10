using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(FlipBehavior))]
public class FlyBehavior : EntityBehavior
{
	public FlyBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isDisturbed { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public Vector2 netDisturb { get; private set; }

	private FlipBehavior direction;


	public override void onAwake()
	{
		direction = controller.getBehavior<FlipBehavior>();
	}

	public override void onUpdate()
	{
	}

	public override bool onFixedUpdate()
	{
		Vector2 targetVelocity = Vector2.zero;

		isDisturbed = calculateAverageDisturb();

		targetVelocity += Vector2.down * data.fallSpeed;

		if (isDisturbed)
			targetVelocity -= netDisturb * data.avoidSpeed;

		if (targetVelocity == Vector2.zero)
			return true;

		addSmoothForce(targetVelocity.magnitude, data.accelerationCoefficient, targetVelocity.normalized);

		return true;
	}


	private bool calculateAverageDisturb()
	{
		netDisturb = Vector2.zero;
		int rotationCount = 30;

		Vector2 center = transform.position;
		center.y -= data.fallSpeed / data.avoidSpeed * data.safeSpaceSmall / 2;

		for (int i = 0; i < rotationCount; i++)
		{
			float delta = 2 * i * Mathf.PI  / rotationCount;
			Vector2 dir = new Vector2(Mathf.Cos(delta), Mathf.Sin(delta));

			var hit = Physics2D.Raycast(center, dir, data.safeSpaceBig, data.avoidLayers);

			if (!hit)
				continue;

			Vector2 vec = hit.point - center;
			float vecMag = vec.magnitude;

			Vector2 contribution;
			if (vecMag == 0)
				contribution = Vector2.down;
			else if (vecMag <= data.safeSpaceSmall)
				contribution = vec.normalized;
			else
				contribution = vec.normalized * ((data.safeSpaceBig - vec.magnitude) 
					/ (data.safeSpaceBig - data.safeSpaceSmall));

			netDisturb += contribution;
		}

		if (netDisturb == Vector2.zero)
			return false;

		if (netDisturb.magnitude > 1)
			netDisturb = netDisturb.normalized;

		return true;
	}

}