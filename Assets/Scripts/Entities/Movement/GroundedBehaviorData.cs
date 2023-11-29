using UnityEngine;

[CreateAssetMenu(menuName = "Data/Behavior/Grounded")]
public class GroundBehaviorData : ScriptableObject
{
	[Space(10)]

	[Tooltip("Layers that can be run on")]
	public LayerMask groundLayers;
	[Tooltip("Layers that trigger flip")]
	public LayerMask wallLayers;
	[Tooltip("Layers that can be passed through")]
	public LayerMask passingLayers;
	[Tooltip("Layer detecting whether ground is a slope")]
	public LayerMask slopeLayer;
}
