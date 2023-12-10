using UnityEngine;

[RequireComponent(typeof(ChaseBehavior))]
public class SleepBehavior : EntityBehavior
{
	public SleepBehaviorData data;

	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isSleeping { get; private set; }
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public Vector2 sleepPosition { get; private set; }

	private ChaseBehavior chase;


	public override void onAwake()
	{
		chase = controller.getBehavior<ChaseBehavior>();

		isSleeping = true;
	}
	public override void onStart()
	{
		sleepPosition = transform.position;
	}

	public override void onUpdate()
	{
		if (chase.isChasing)
			isSleeping = false;
		else if (Vector2.Distance(transform.position, sleepPosition) < data.lockInDistance)
			isSleeping = true;

		if (isSleeping)
			transform.position = sleepPosition;

		foreach (var param in controller.animator.parameters)
			if (param.name == "isSleeping")
				controller.animator.SetBool("isSleeping", isSleeping);
	}

	public override bool onFixedUpdate()
	{
		return isSleeping;
	}

}