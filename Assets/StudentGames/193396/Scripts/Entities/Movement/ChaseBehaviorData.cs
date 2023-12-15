using UnityEngine;

[CreateAssetMenu(menuName = "Data/Behavior/Chase")]
public class ChaseBehaviorData : ScriptableObject
{
	[Tooltip("Layers to chase")]
	public LayerManager.LayerMaskInput targetLayers;
	[Tooltip("Layers that hide the target")]
	public LayerManager.LayerMaskInput obstructLayers;

	[Space(10)]

	[Tooltip("Maximum distance to detect and chase the target")]
	public float maxDistance = 10f;
	[Tooltip("Minimum distance to detect and chase the target")]
	public float minDistance = 3f;

	[Space(5)]

	[Tooltip("Time to chase a lost target")]
	public float determinationTime = 6f;
}
