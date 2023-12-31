using System;
using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class StateMachineCaller : StateMachineBehaviour
	{
		public enum StateType
		{
			Enter = 0, Exit = 1
		}
		public enum CallType
		{
			SetTrigger, ResetTrigger, SetBool, ClearBool, SetInteger, AddInteger, SetIntegerRandom,
			SetFloat
		}
		[Serializable]
		public class Data
		{
			public int intValue = 0;
			public float floatValue = 0;
			[Space(5)]
			public float waitFor = 0f;
		}

		public StateType type = StateType.Enter;
		public CallType callType = CallType.SetTrigger;
		public string parameterName;
		[Space(5)]
		public Data data;

		private int waitIndex = -1;
		private List<int> lastRandoms;


		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (type == StateType.Enter)
				animator.gameObject.GetComponent<StateMachineHelper>()
					.run(() => action(animator), data.waitFor, out waitIndex);
		}
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (type == StateType.Exit)
				action(animator);
			else
				animator.gameObject.GetComponent<StateMachineHelper>().halt(waitIndex);
		}

		private void action(Animator animator)
		{
			switch (callType)
			{
				case CallType.SetTrigger:
					animator.SetTrigger(parameterName);
					break;
				case CallType.ResetTrigger:
					animator.ResetTrigger(parameterName);
					break;
				case CallType.SetBool:
					animator.SetBool(parameterName, true);
					break;
				case CallType.ClearBool:
					animator.SetBool(parameterName, false);
					break;
				case CallType.SetInteger:
					animator.SetInteger(parameterName, data.intValue);
					break;
				case CallType.AddInteger:
					animator.SetInteger(parameterName, animator.GetInteger(parameterName) + data.intValue);
					break;
				case CallType.SetIntegerRandom:
					animator.SetInteger(parameterName, getRandomInteger());
					break;
				case CallType.SetFloat:
					animator.SetFloat(parameterName, data.floatValue);
					break;
			}
		}

		// Prevent repeating of the same numbers 
		private int getRandomInteger()
		{
			if (lastRandoms == null)
				lastRandoms = new List<int>();

			if (lastRandoms.Count >= data.intValue)
				lastRandoms.Clear();

			int val;
			do
			{
				val = UnityEngine.Random.Range(0, data.intValue);
			}
			while (lastRandoms.Contains(val));

			lastRandoms.Add(val);

			return val;
		}

	}
}