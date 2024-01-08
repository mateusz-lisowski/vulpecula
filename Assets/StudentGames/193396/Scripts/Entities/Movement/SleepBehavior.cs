using System.Collections;
using System.Diagnostics;
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

		private Coroutine returningCoroutine;
		private bool startSleeping = false;


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

			if (startSleeping)
			{
				isSleeping = true;
				startSleeping = false;
			}

			if (isSleeping && chase.isChasing 
				&& Vector2.Distance(transform.position, chase.lastTargetPosition) <= data.minWakeDistance)
			{
				if (returningCoroutine != null)
					StopCoroutine(returningCoroutine);
				returningCoroutine = null;
				StopCoroutine(Effects.instance.fade.run(controller.spriteRenderer, 
					revert: true, move: false));

				isSleeping = false;
			}

			if (!isSleeping && !chase.isChasing && returningCoroutine == null)
				returningCoroutine = StartCoroutine(startReturning());

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


		private IEnumerator startReturning()
		{
			yield return Effects.instance.fade.run(controller.spriteRenderer, move: false);

			returningCoroutine = null;

			if (chase.isChasing)
				yield break;

			startSleeping = true;
			transform.position = sleepPosition;

			yield return Effects.instance.fade.run(controller.spriteRenderer,
				revert: true, move: false);
		}
	}
}