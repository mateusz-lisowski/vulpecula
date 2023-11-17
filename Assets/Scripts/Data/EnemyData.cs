using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy")]
public class EnemyData : ScriptableObject
{
	[System.Serializable]
	public struct Gravity
	{
		[Tooltip("Acceleration of gravity")]
		[field: SerializeField, ReadOnly] public float strength;
		[Tooltip("Gravity as a multiple of Physics2D.gravity.y")]
		[field: SerializeField, ReadOnly] public float scale;
	}
	[System.Serializable]
	public struct Hurt
	{
		[Tooltip("Time of invulnerability after getting hit")]
		public float invulnerabilityTime;
		[Tooltip("Time to ignore input after getting hit")]
		public float distressTime;
		[Tooltip("Maximum knockback speed")]
		public float knockbackMaxSpeed;
		[Tooltip("Knockback height (point on parabola when returned input)")]
		[Range(0.5f, 1.0f)] public float knockbackHeightScale;

		[Space(5)]

		[Tooltip("Calculated maximum reachable height of a knockback")]
		[field: SerializeField, ReadOnly] public float knockbackHeight;
		[Tooltip("Calculated hit knockback force")]
		[field: SerializeField, ReadOnly] public float knockbackForce;
	}
	[System.Serializable]
	public struct Attack
	{
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
	[System.Serializable]
	public struct Run
	{
		[Tooltip("Layers that can be run on")]
		public LayerMask groundLayers;
		[Tooltip("Layers that trigger flip")]
		public LayerMask wallLayers;
		[Tooltip("Maximum running speed")]
		public float maxSpeed;
		[Tooltip("Acceleration rate (0 = none, maxSpeed = instant)")]
		public float acceleration;
		[Tooltip("Deceleration rate (0 = none, maxSpeed = instant)")]
		public float deceleration;

		[Space(5)]

		[Tooltip("Calculated acceleration force")]
		[field: SerializeField, ReadOnly] public float accelerationForce;
		[Tooltip("Calculated deceleration force")]
		[field: SerializeField, ReadOnly] public float decelerationForce;
	}

	public Gravity gravity;
	public Hurt hurt;
	public Attack attack;
	public Run run;

	private void OnValidate()
	{
		gravity.strength = Physics2D.gravity.y;

		hurt.knockbackForce = -gravity.strength * hurt.distressTime / (2 * hurt.knockbackHeightScale);
		hurt.knockbackHeight = (hurt.knockbackForce * hurt.knockbackForce) / (2 * -gravity.strength);

		gravity.scale = gravity.strength / Physics2D.gravity.y;

		float fixedUpdateFrequency = 1f / Time.fixedDeltaTime;

		run.accelerationForce = (fixedUpdateFrequency * run.acceleration) / run.maxSpeed;
		run.decelerationForce = (fixedUpdateFrequency * run.deceleration) / run.maxSpeed;
	}
}
