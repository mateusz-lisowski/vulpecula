using UnityEngine;

[CreateAssetMenu(menuName = "Data/Behavior/Run")]
public class RunBehaviorData : ScriptableObject
{
	[Space(10)]

	[Tooltip("Layers that can be run on")]
	public LayerMask groundLayers;
	[Tooltip("Layers that trigger flip")]
	public LayerMask wallLayers;
	[Tooltip("Maximum running speed")]
	public float maxSpeed = 3.0f;
	[Tooltip("Lerp between current velocity (0) and maxSpeed (1)")]
	[Range(0.0f, 1.0f)] public float accelerationCoefficient = 0.8f;
}
