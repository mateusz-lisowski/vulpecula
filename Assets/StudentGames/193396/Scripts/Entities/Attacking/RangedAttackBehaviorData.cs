using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Behavior/RangedAttack")]
	public class RangedAttackBehaviorData : BaseAttackBehaviorData
	{
		[Space(10)]

		[Tooltip("Speed of an attack")]
		public float speed = 3f;
		[Tooltip("Time after which the projectile is destroyed")]
		public float maxLifetime = 30f;
	}
}