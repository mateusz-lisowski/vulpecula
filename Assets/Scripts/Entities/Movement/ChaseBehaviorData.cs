using UnityEngine;

[CreateAssetMenu(menuName = "Data/Behavior/Chase")]
public class ChaseBehaviorData : ScriptableObject
{
	[Tooltip("Layers to chase")]
	public LayerMask targetLayers;
	[Tooltip("Layers that hide the target")]
	public LayerMask obstructLayers;

	[Space(10)]

	[Tooltip("Maximum distance to detect and chase the target")]
	public float maxDistance = 10f;
	[Tooltip("Minimum distance to detect and chase the target")]
	public float minDistance = 3f;
}
