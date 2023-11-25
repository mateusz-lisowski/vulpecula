using UnityEngine;

public class DestroyOnAnimationEnter : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Destroy(animator.gameObject);
	}
}
