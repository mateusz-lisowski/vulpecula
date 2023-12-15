using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Behavior/Collect")]
	public class CollectBehaviorData : ScriptableObject
	{
		[Space(10)]

		[Tooltip("Layers that can collect")]
		public LayerManager.LayerMaskInput collectingLayers;
	}
}