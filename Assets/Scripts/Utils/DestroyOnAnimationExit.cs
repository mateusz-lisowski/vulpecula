using UnityEngine;

public class DestroyOnAnimationExit : StateMachineBehaviour
{
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Destroy(animator.gameObject);
	}
}
