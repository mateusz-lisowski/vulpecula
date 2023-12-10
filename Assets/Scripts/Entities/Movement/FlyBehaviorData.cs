using UnityEngine;

[CreateAssetMenu(menuName = "Data/Behavior/Fly")]
public class FlyBehaviorData : ScriptableObject
{
	[Tooltip("Layers to fly away from")]
	public LayerMask avoidLayers;

	[Space(10)]

	[Tooltip("Minimum distance to avoid obstacles")]
	public float safeSpace = 2.5f;
	
	[Space(5)]

	[Tooltip("Maximum flying speed")]
	public float maxSpeed = 3.0f;
	[Tooltip("Lerp between current velocity (0) and maxSpeed (1)")]
	[Range(0.0f, 1.0f)] public float accelerationCoefficient = 0.8f;
}
