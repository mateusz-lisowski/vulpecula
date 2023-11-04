using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	private EnemyData data;


	public void launch(EnemyData initData)
    {
		data = initData;

		GameObject currentAttack = Instantiate(data.attackPrefab, transform.position, transform.rotation);
		AttackController currentAttackData = currentAttack.GetComponent<AttackController>();
		
		currentAttackData.setCollisionTime(data.attackCastTime, data.attackLastTime);
		currentAttackData.setHitboxSize(transform.localScale);
    }


	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, transform.lossyScale);
	}
}
