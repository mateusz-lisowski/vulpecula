using UnityEngine;

[CreateAssetMenu(menuName = "Data/Behavior/Run")]
public class RunBehaviorData : ScriptableObject
{
	[SerializeField] private HurtBehaviorData associatedHurtData;

	[Space(10)]

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
	[ReadOnly] public float accelerationForce;
	[Tooltip("Calculated deceleration force")]
	[ReadOnly] public float decelerationForce;

	[Space(10)]

	[Tooltip("Maximum knockback speed")]
	public float knockbackMaxSpeed;
	[Tooltip("Knockback height (point on parabola when returned input)")]
	[Range(0.5f, 1.0f)] public float knockbackHeightScale;

	[Space(5)]

	[Tooltip("Calculated maximum reachable height of a knockback")]
	[ReadOnly] public float knockbackHeight;
	[Tooltip("Calculated hit knockback force")]
	[ReadOnly] public float knockbackForce;


	private void OnValidate()
	{
		if (associatedHurtData != null)
		{
			knockbackForce = -Physics2D.gravity.y * associatedHurtData.distressTime / (2 * knockbackHeightScale);
			knockbackHeight = (knockbackForce * knockbackForce) / (2 * -Physics2D.gravity.y);
		}

		float fixedUpdateFrequency = 1f / Time.fixedDeltaTime;

		accelerationForce = (fixedUpdateFrequency * acceleration) / maxSpeed;
		decelerationForce = (fixedUpdateFrequency * deceleration) / maxSpeed;
	}
}
