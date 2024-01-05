using System.Collections;
using UnityEngine;
using static _193396.EntityEventReceiver;

namespace _193396
{
	public class LayerOverlapEventCaller : MonoBehaviour
	{
		public EntityBehaviorController controller;
		[Space(5)]
		public InputtableEvent eventOverlapBegin;
		public InputtableEvent eventOverlapEnd;
		[Space(5)]
		public RuntimeSettings.LayerMaskInput overlapLayers;
		[Space(5)]
		public float minExitTime;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isOverlapping { get; private set; }

		private Collider2D overlapCheck;

		private Coroutine currentExiting;


		private void Awake()
		{
			overlapCheck = transform.GetComponent<Collider2D>();
		}

		private void Update()
		{
			bool wasOverlapping = isOverlapping;

			isOverlapping = overlapCheck.IsTouchingLayers(overlapLayers);

			if (isOverlapping != wasOverlapping)
				if (isOverlapping)
				{
					if (currentExiting == null)
						controller.onEvent(eventOverlapBegin.name, eventOverlapBegin.data);
					else
					{
						StopCoroutine(currentExiting);
						currentExiting = null;
					}
				}
				else
				{
					if (minExitTime == 0f)
						controller.onEvent(eventOverlapEnd.name, eventOverlapEnd.data);
					else
						currentExiting = StartCoroutine(waitExit());
				}
		}
	
	
		private IEnumerator waitExit()
		{
			yield return new WaitForSeconds(minExitTime);

			controller.onEvent(eventOverlapEnd.name, eventOverlapEnd.data);
			currentExiting = null;
		}
	}
}