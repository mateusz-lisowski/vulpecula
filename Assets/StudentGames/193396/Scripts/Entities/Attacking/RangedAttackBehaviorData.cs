using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Behavior/RangedAttack")]
	public class RangedAttackBehaviorData : ScriptableObject
	{
		[Tooltip("Name of the event instantiating the attack")]
		public string attackInstantiateEventName = "attack";
		[Tooltip("Name of the child object detecting provokation")]
		public string provokeDetectionName = "Attack";

		[Space(10)]

		[Tooltip("Layers that can be hit by an attack")]
		public RuntimeSettings.LayerMaskInput hitLayers;
		[Tooltip("Object to instantiate on an attack")]
		public GameObject attackPrefab;

		[Space(5)]

		[Tooltip("Minimum time between two consecutive attacks")]
		public float cooldown = 0.3f;
		[Tooltip("Speed of an attack")]
		public float speed = 3f;
		[Tooltip("Time after which the projectile is destroyed")]
		public float maxLifetime = 30f;

		[Space(5)]

		[Tooltip("Strength of an attack")]
		public int strength = 1;

		[Space(10)]

		[Tooltip("Running speed while attacking")]
		public float attackRunSpeed = 0.0f;
		[Tooltip("Lerp between current velocity (0) and attackRunSpeed (1)")]
		[Range(0.0f, 1.0f)] public float accelerationCoefficient = 0.8f;
	}
}