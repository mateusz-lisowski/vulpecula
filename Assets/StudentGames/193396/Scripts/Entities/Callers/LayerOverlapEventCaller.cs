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
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isOverlapping { get; private set; }

		private Collider2D overlapCheck;


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
					controller.onEvent(eventOverlapBegin.name, eventOverlapBegin.data);
				else
					controller.onEvent(eventOverlapEnd.name, eventOverlapEnd.data);
		}
	}
}