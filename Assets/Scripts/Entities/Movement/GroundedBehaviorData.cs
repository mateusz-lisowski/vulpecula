using UnityEngine;

[CreateAssetMenu(menuName = "Data/Behavior/Grounded")]
public class GroundBehaviorData : ScriptableObject
{
	[Space(10)]

	[Tooltip("Layers that can be run on")]
	public LayerMask groundLayers;
	[Tooltip("Layers that trigger flip")]
	public LayerMask wallLayers;
}
