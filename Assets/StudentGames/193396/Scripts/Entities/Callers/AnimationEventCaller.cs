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

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (type == StateType.Enter)
				animator.gameObject.GetComponent<EntityBehaviorEventCaller>().callEvent(eventName);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (type == StateType.Exit)
				animator.gameObject.GetComponent<EntityBehaviorEventCaller>().callEvent(eventName);
		}
	}
}