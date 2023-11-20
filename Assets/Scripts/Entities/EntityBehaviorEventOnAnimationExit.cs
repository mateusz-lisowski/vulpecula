using UnityEngine;

public class EntityBehaviorEventOnAnimationExit : StateMachineBehaviour
{
	public string eventName;

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.gameObject.GetComponent<EntityBehaviorEventCaller>().callEvent(eventName);
	}
}
