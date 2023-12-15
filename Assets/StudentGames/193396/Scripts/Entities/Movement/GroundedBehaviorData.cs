using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Behavior/Grounded")]
	public class GroundBehaviorData : ScriptableObject
	{
		[Space(10)]

		[Tooltip("Layers that can be run on")]
		public LayerManager.LayerMaskInput groundLayers;
		[Tooltip("Layers that trigger flip")]
		public LayerManager.LayerMaskInput wallLayers;
		[Tooltip("Layers that can be passed through")]
		public LayerManager.LayerMaskInput passingLayers;
		[Tooltip("Layer detecting whether ground is a slope")]
		public LayerManager.LayerMaskInput slopeLayer;
	}
}