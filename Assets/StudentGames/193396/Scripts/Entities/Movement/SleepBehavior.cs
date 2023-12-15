using UnityEngine;

namespace _193396
{
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
			bool wasSleeping = isSleeping;

			if (chase.isChasing)
				isSleeping = false;
			else if (Vector2.Distance(transform.position, sleepPosition) < data.lockInDistance)
				isSleeping = true;

			if (isSleeping)
				transform.position = sleepPosition;

			if (isSleeping != wasSleeping)
				if (isSleeping)
					controller.onEvent("fellAsleep", null);
				else
					controller.onEvent("awoke", null);
		}

		public override bool onFixedUpdate()
		{
			return isSleeping;
		}

	}
}