using UnityEngine;

[CreateAssetMenu(menuName = "Data/Behavior/MeleeAtack")]
public class MeleeAtackBehaviorData : ScriptableObject
{
	public string attackInstantiateEventName = "attack";

	[Space(5)]

	[Tooltip("Layers that can provoke an attack")]
	public LayerMask provokeLayers;
	[Tooltip("Layers that can be hit by an attack")]
	public LayerMask hitLayers;
	[Tooltip("Object to instantiate on an attack")]
	public GameObject attackPrefab;

	[Space(5)]

	[Tooltip("Minimum time between two consecutive attacks")]
	public float cooldown;
}
