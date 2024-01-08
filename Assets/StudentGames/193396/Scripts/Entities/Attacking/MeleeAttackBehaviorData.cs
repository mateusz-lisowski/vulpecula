using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "193396/Data/Behavior/MeleeAttack")]
	public class MeleeAttackBehaviorData : BaseAttackBehaviorData
	{
		[Space(10)]

		[Tooltip("Layers that can provoke an attack")]
		public RuntimeSettings.LayerMaskInput provokeLayers;
	}
}