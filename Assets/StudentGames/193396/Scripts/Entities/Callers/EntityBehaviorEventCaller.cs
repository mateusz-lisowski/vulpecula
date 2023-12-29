using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class EntityBehaviorEventCaller : MonoBehaviour
	{
		public EntityBehaviorController controller;

		public void callEvent(string name)
		{
			Debug.Log("Event: " + name);

			int separatorIndex = name.IndexOf(':');

			if (separatorIndex == -1)
				controller.onEvent(name, null);
			else
			{
				string dataString = name.Substring(separatorIndex + 1, name.Length - separatorIndex - 1);
				name = name.Substring(0, separatorIndex);

				int dataInt;
				if (int.TryParse(dataString, out dataInt))
					controller.onEvent(name, dataInt);
				else
					controller.onEvent(name, dataString);
			}
		}
	}
}