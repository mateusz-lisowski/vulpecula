using UnityEngine;

[CreateAssetMenu(menuName = "Data/Behavior/MeleeAtack")]
public class MeleeAtackBehaviorData : ScriptableObject
{
	public string attackInstantiateEventName = "attack";
	public string provokeDetectionName = "Attack";
	public string animatorEventName = "isAttacking";

	[Space(10)]

	[Tooltip("Layers that can provoke an attack")]
	public LayerMask provokeLayers;
	[Tooltip("Layers that can be hit by an attack")]
	public LayerMask hitLayers;
	[Tooltip("Object to instantiate on an attack")]
	public GameObject attackPrefab;

	[Space(5)]

	[Tooltip("Minimum time between two consecutive attacks")]
	public float cooldown = 0.3f;

	[Space(10)]

	[Tooltip("Running speed while attacking")]
	public float attackRunSpeed = 0.0f;
	[Tooltip("Lerp between current velocity (0) and attackRunSpeed (1)")]
	[Range(0.0f, 1.0f)] public float accelerationCoefficient = 0.8f;
}
