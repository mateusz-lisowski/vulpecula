using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Data/Player")]
public class PlayerData : ScriptableObject
{
	[System.Serializable] public struct Gravity
	{
		[Tooltip("Gravity multiplier when falling")]
		public float fallMultiplier;
		[Tooltip("Maximum falling speed")]
		public float maxFallSpeed;

		[Space(5)]

		[Tooltip("Acceleration of gravity")]
		[field: SerializeField, ReadOnly] public float strength;
		[Tooltip("Gravity as a multiple of Physics2D.gravity.y")]
		[field: SerializeField, ReadOnly] public float scale;
	}
	[System.Serializable] public struct Hurt
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

		[Tooltip("Calculated hit knockback force")]
		[field: SerializeField, ReadOnly] public float knockbackForce;
	}
	[System.Serializable] public struct Attack
	{
		[Tooltip("Layers that can be hit by an attack")]
		public LayerMask hitLayers;
		[Tooltip("Object to instantiate on a forward attack")]
		public GameObject attackForwardPrefab;
		[Tooltip("Object to instantiate on a down attack")]
		public GameObject attackDownPrefab;
		
		[Space(5)]
		[Tooltip("Minimum time between two consecutive attacks")]
		public float cooldown; 

		[Space(10)]
		[Tooltip("Maximum reachable height of a down hit bounce")]
		public float attackDownBounceHeight;
		[Tooltip("Calculated down hit bounce force")]
		[field: SerializeField, ReadOnly] public float attackDownBounceForce;

		[Space(10)]
		[Tooltip("Offset of attack hitboxes based on the current velocity")]
		public float hitboxOffsetScale;
		[Tooltip("Time within which too early attack will still be performed")]
		[Range(0f, 0.5f)] public float inputBufferTime; 
	}
	[System.Serializable] public struct Run
	{
		[Tooltip("Layers that can be run on")]
		public LayerMask groundLayers;
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
	[System.Serializable] public struct Jump
	{
		[Tooltip("Maximum reachable height of a jump")]
		public float maxHeight;
		[Tooltip("Time to reach the maximum height of a jump")]
		public float timeToApex;
		[Tooltip("Minimum time between two consecutive jumps")]
		public float cooldown;
		[Tooltip("Number of jumps (2 = double jump)")]
		public int jumpsCount;

		[Space(5)]

		[Tooltip("Calculated jump force")]
		[field: SerializeField, ReadOnly] public float force;

		[Space(10)]
		[Tooltip("Gravity multiplier released jump button")]
		public float jumpCutGravityMultiplier;
		[Tooltip("Y velocity at the top of the jump below which to \"hang\"")]
		public float hangingVelocityThreshold;
		[Tooltip("Gravity multiplier while \"hanging\"")]
		[Range(0f, 1)] public float hangingGravityMultiplier;
		[Tooltip("Speed multiplier while \"hanging\"")]
		public float hangingSpeedMultiplier;

		[Space(10)]
		[Tooltip("Time after falling off a platform where you can still jump")]
		[Range(0.01f, 0.5f)] public float coyoteTime;
		[Tooltip("Time within which too early jump will still be performed")]
		[Range(0.01f, 0.5f)] public float inputBufferTime;
	}
	[System.Serializable] public struct Wall
	{
		[Tooltip("Layers that can be slid and jumped from")]
		public LayerMask layers;
		[Tooltip("Maximum wall slide speed")]
		public float slideMaxSpeed;
		[Tooltip("Acceleration rate (0 = none, wallSlideMaxSpeed = instant)")]
		public float slideAcceleration;
		[Tooltip("Deceleration rate (0 = none, wallSlideMaxSpeed = instant)")]
		public float slideDeceleration;

		[Space(5)]

		[Tooltip("Calculated acceleration force")]
		[field: SerializeField, ReadOnly] public float slideAccelerationForce;
		[Tooltip("Calculated deceleration force")]
		[field: SerializeField, ReadOnly] public float slideDecelerationForce;

		[Space(5)]

		[Header("Wall Jump")]
		[Tooltip("Layer detecting whether can wall jump")]
		public LayerMask canJumpLayer;
		[Tooltip("Force to jump off a wall")]
		public float jumpForce;
		[Tooltip("Input reduction after wall jumping")]
		[Range(0f, 1f)] public float jumpInputReduction;
		[Tooltip("Time to reduce the input wall jumping")]
		public float jumpTime;
		[Tooltip("Minimum time of a wall jump (prevent isGrounded)")]
		public float jumpMinTime;

		[Space(10)]
		[Tooltip("Time after falling off a wall where you can still wall jump")]
		[Range(0.01f, 0.5f)] public float jumpCoyoteTime;
	}
	[System.Serializable] public struct Dash
	{
		[Tooltip("Maximum reachable distance of a dash")]
		public float distance;
		[Tooltip("Time to reach the maximum distance of a dash")]
		public float time;
		[Tooltip("Minimum time between two consecutive dashes")]
		public float cooldown;
		[Tooltip("Number of dashes")]
		public int dashesCount;

		[Space(5)]

		[Tooltip("Calculated dash force")]
		[field: SerializeField, ReadOnly] public float force;

		[Space(10)]

		[Tooltip("Time within which too early dash will still be performed")]
		[Range(0.01f, 0.5f)] public float inputBufferTime;
	}
	[System.Serializable] public struct PlatformPassing
	{
		[Tooltip("Layers that can be passed through")]
		public LayerMask layers;

		[Space(10)]

		[Tooltip("Time within which too early pass will still be performed")]
		[Range(0.01f, 0.5f)] public float inputBufferTime;
	}
	[System.Serializable] public struct GroundDropping
	{
		[Tooltip("Layer detecting whether can ground drop")]
		public LayerMask canDropLayer;
	}

	public Gravity gravity;
	public Hurt hurt;
	public Attack attack;
	public Run run;
	public Jump jump;
	public Wall wall;
	public Dash dash;
	public PlatformPassing platformPassing;
	public GroundDropping groundDropping;

	private void OnValidate()
	{
		gravity.strength = -(2 * jump.maxHeight) / (jump.timeToApex * jump.timeToApex);
		jump.force = -gravity.strength * jump.timeToApex;
		attack.attackDownBounceForce = Mathf.Sqrt(2 * -gravity.strength * attack.attackDownBounceHeight);

		hurt.knockbackForce = -gravity.strength * hurt.distressTime / (2 * hurt.knockbackHeightScale);

		gravity.scale = gravity.strength / Physics2D.gravity.y;

		dash.force = dash.distance / dash.time;

		float fixedUpdateFrequency = 1f / Time.fixedDeltaTime;

		run.accelerationForce = (fixedUpdateFrequency * run.acceleration) / run.maxSpeed;
		run.decelerationForce = (fixedUpdateFrequency * run.deceleration) / run.maxSpeed;

		wall.slideAccelerationForce = (fixedUpdateFrequency * wall.slideAcceleration) / wall.slideMaxSpeed;
		wall.slideDecelerationForce = (fixedUpdateFrequency * wall.slideDeceleration) / wall.slideMaxSpeed;
	}
}