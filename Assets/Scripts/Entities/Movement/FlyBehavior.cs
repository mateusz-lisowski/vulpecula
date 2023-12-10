using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(FlipBehavior))]
public class FlyBehavior : EntityBehavior
{
	public FlyBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isFalling { get; private set; }
	[field: SerializeField, ReadOnly] public bool isDisturbed { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public float disturbDistance { get; private set; }
	[field: SerializeField, ReadOnly] public Vector2 disturbDirection { get; private set; }

	private FlipBehavior direction;

	private Collider2D fallCheck;
	private Collider2D disturbCheck;


	public override void onAwake()
	{
		direction = controller.getBehavior<FlipBehavior>();

		fallCheck = transform.Find("Fall Check").GetComponent<Collider2D>();
		disturbCheck = transform.Find("Safe Space Check").GetComponent<Collider2D>();
	}

	public override void onUpdate()
	{
		isFalling = !fallCheck.IsTouchingLayers(data.avoidLayers);

		ContactFilter2D filter = new ContactFilter2D().NoFilter();
		filter.SetLayerMask(data.avoidLayers);
		filter.useLayerMask = true;

		List<Collider2D> contacts = new List<Collider2D>();
		isDisturbed = disturbCheck.OverlapCollider(filter, contacts) != 0;

		if (isDisturbed)
			calculateAverageDisturb(contacts);
	}

	public override bool onFixedUpdate()
	{
		if (isFalling)
			addSmoothForce(data.maxSpeed, data.accelerationCoefficient, Vector2.down);

		if (isDisturbed)
			addSmoothForce(data.maxSpeed, data.accelerationCoefficient, -disturbDirection);

		return true;
	}


	private void calculateAverageDisturb(List<Collider2D> contacts)
	{
		Vector2 netDisturb = Vector2.zero;

		foreach (var contact in contacts)
		{
			Vector2 closestPoint = contact.ClosestPoint(transform.position);

			netDisturb += closestPoint - (Vector2)transform.position;
		}

		disturbDistance = netDisturb.magnitude;
		disturbDirection = netDisturb.normalized;
	}

}