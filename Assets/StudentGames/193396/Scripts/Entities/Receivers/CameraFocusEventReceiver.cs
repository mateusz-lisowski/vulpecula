using _193396;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class CameraFocusEventReceiver : EntityEventReceiver
	{
		public CameraController focusCamera;

		[Space(5)]

		public InputtableEvent eventFocus;
		public InputtableEvent eventUnfocus;


		public override string[] capturableEvents => new string[] { eventFocus.name, eventUnfocus.name };
		public override void onEvent(string eventName, object eventData)
		{
			if (eventFocus.matches(eventName, eventData))
			{
				focusCamera.pushTarget(transform);
			}
			else if (eventUnfocus.matches(eventName, eventData))
			{
				focusCamera.popTarget(transform);
			}
		}
	}
}