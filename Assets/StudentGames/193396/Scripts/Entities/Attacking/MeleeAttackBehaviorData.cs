using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Behavior/MeleeAttack")]
	public class MeleeAttackBehaviorData : BaseAttackBehaviorData
	{
		[Space(10)]

		[Tooltip("Layers that can provoke an attack")]
		public RuntimeSettings.LayerMaskInput provokeLayers;
	}
}