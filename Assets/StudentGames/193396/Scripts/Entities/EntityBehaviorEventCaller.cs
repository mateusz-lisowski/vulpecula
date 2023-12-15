using UnityEngine;

namespace _193396
{
	public class EntityBehaviorEventCaller : MonoBehaviour
	{
		public EntityBehaviorController controller;

		public void callEvent(string name)
		{
			int separatorIndex = name.IndexOf(':');

			if (separatorIndex == -1)
				controller.onEvent(name, null);
			else
				controller.onEvent(name.Substring(0, separatorIndex),
					name.Substring(separatorIndex + 1, name.Length - separatorIndex - 1));
		}
	}
}