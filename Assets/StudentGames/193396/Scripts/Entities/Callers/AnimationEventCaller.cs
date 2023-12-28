using UnityEngine;

namespace _193396
{
	public class AnimationEventCaller : StateMachineBehaviour
	{
		public enum StateType
		{
			Enter = 0, Exit = 1
		}

		public StateType type = StateType.Enter;
		public string eventName;
		[Space(5)]
		public float waitFor = 0f;

		private int waitIndex = -1;


		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (type == StateType.Enter)
				animator.gameObject.GetComponent<EntityBehaviorEventCaller>()
					.callEventWaited(eventName, waitFor, out waitIndex);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (type == StateType.Exit)
				animator.gameObject.GetComponent<EntityBehaviorEventCaller>().callEvent(eventName);
			else
				animator.gameObject.GetComponent<EntityBehaviorEventCaller>().haltEventWaited(waitIndex);
		}
	}
}