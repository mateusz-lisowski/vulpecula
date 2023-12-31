using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class StateMachineHelper : MonoBehaviour
	{
		private int coroutinesIndex = 0;
		private HashSet<int> activeCoroutines = new HashSet<int>();


		public void run(Action action, float waitFor, out int waitIndex)
		{
			waitIndex = -1;

			if (waitFor == 0f)
				action();
			else
			{
				waitIndex = ++coroutinesIndex;
				activeCoroutines.Add(waitIndex);
				StartCoroutine(runWaited(action, waitFor, waitIndex));
			}
		}
		public void halt(int waitIndex)
		{
			if (waitIndex == -1)
				return;

			activeCoroutines.Remove(waitIndex);
		}

		private IEnumerator runWaited(Action action, float waitFor, int waitIndex)
		{
			yield return new WaitForSeconds(waitFor);

			if (activeCoroutines.Remove(waitIndex))
				action();
		}
	}
}