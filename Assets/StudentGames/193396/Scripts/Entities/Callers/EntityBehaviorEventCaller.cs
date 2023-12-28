using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class EntityBehaviorEventCaller : MonoBehaviour
	{
		public EntityBehaviorController controller;

		private int coroutinesIndex = 0;
		private HashSet<int> activeCoroutines = new HashSet<int>();


		public void callEvent(string name)
		{
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
		public void callEventWaited(string name, float waitFor, out int waitIndex)
		{
			waitIndex = -1;

			if (waitFor == 0f)
				callEvent(name);
			else
			{
				waitIndex = ++coroutinesIndex;
				activeCoroutines.Add(waitIndex);
				StartCoroutine(callEventWaitedCoroutine(name, waitFor, waitIndex));
			}
		}
		public void haltEventWaited(int waitIndex)
		{
			if (waitIndex == -1)
				return;

			activeCoroutines.Remove(waitIndex);
		}


		private IEnumerator callEventWaitedCoroutine(string name, float waitFor, int waitIndex)
		{
			yield return new WaitForSeconds(waitFor);

			if (activeCoroutines.Remove(waitIndex))
				callEvent(name);
		}
	}
}