using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Behavior/Grounded")]
	public class GroundBehaviorData : ScriptableObject
	{
		[Space(10)]

		[Tooltip("Layers that can be run on")]
		public RuntimeSettings.LayerMaskInput groundLayers;
		[Tooltip("Layers that trigger flip")]
		public RuntimeSettings.LayerMaskInput wallLayers;
		[Tooltip("Layers that can be passed through")]
		public RuntimeSettings.LayerMaskInput passingLayers;
		[Tooltip("Layer detecting whether ground is a slope")]
		public RuntimeSettings.LayerMaskInput slopeLayer;
	}
}