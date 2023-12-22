using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(TrailRenderer))]
	public class EmitterEventReceiver : EntityEventReceiver
	{
		public InputtableEvent eventEnable;
		public InputtableEvent eventDisable;

		private TrailRenderer trail;


		private void Awake()
		{
			trail = transform.GetComponent<TrailRenderer>();
		}

		public override string[] capturableEvents => new string[] { eventEnable.name, eventDisable.name };
		public override void onEvent(string eventName, object eventData)
		{
			if (eventEnable.matches(eventName, eventData))
				trail.emitting = true;
			else if (eventDisable.matches(eventName, eventData))
				trail.emitting = false;
		}
	}
}