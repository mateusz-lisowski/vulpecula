using UnityEngine;

namespace _193396
{
	public class EventTransformer : EntityBehavior
	{
		public string eventName;
		public string eventData;
		[Space(5)]
		public string transformedEventName;
		public string transformedEventData;
		[Space(5)]
		public bool keepData;

		public override string[] capturableEvents => new string[] { eventName };
		public override void onEvent(string eventName, object eventData)
		{
			if (!InputtableEvent.matches(this.eventName, this.eventData, eventName, eventData))
				return;

			object newEventData;
			if (keepData)
				newEventData = eventData;
			else if (transformedEventData != "")
				newEventData = transformedEventData;
			else
				newEventData = null;

			controller.onEvent(transformedEventName, newEventData);
		}
	}
}