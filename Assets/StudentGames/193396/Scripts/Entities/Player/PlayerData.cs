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

		[Tooltip("Calculated maximum reachable height of a knockback")]
		[field: SerializeField, ReadOnly] public float knockbackHeight;
		[Tooltip("Calculated hit knockback force")]
		[field: SerializeField, ReadOnly] public float knockbackForce;
	}
	[System.Serializable] public struct Attack
	{
		[Tooltip("Layers that can be hit by an attack")]
		public LayerManager.LayerMaskInput hitLayers;
		[Tooltip("Object to instantiate on a forward attack 1")]
		public GameObject attackForward1Prefab;
		[Tooltip("Object to instantiate on a forward attack 2")]
		public GameObject attackForward2Prefab;
		[Tooltip("Object to instantiate on a forward attack 3 (Strong attack)")]
		public GameObject attackForward3Prefab;
		[Tooltip("Object to instantiate on a forward attack while in the air")]
		public GameObject attackForwardAirPrefab;
		[Tooltip("Object to instantiate on a down attack")]
		public GameObject attackDownPrefab;
		
		[Space(5)]
		[Tooltip("Maximum time between consecutive forward attacks in a combo")]
		public float comboResetTime;
		[Tooltip("Minimum time between two consecutive forward attacks (non-reset)")]
		public float forwardCooldown;
		[Tooltip("Minimum time between two consecutive down attacks")]
		public float downCooldown;

		[Space(5)]

		[Tooltip("Strength of a forward attack")]
		public int forwardStrength;
		[Tooltip("Strength of a forward strong attack")]
		public int forwardStrongStrength;
		[Tooltip("Strength of a forward air attack")]
		public int forwardAirStrength;
		[Tooltip("Strength of a down attack")]
		public int downStrength;

		[Space(10)]
		[Tooltip("Maximum moving speed while forward attacking")]
		public float forwardSpeed;
		[Tooltip("Acceleration rate of forward attack (0 = none, forwardSpeed = instant)")]
		public float forwardAcceleration;

		[Space(10)]
		[Tooltip("Vertical slowdown while down attacking")]
		public float attackDownSlowdown;
		[Tooltip("Maximum reachable height of a down hit bounce")]
		public float attackDownBounceHeight;
		[Tooltip("Maximum reachable height of a down hit high bounce")]
		public float attackDownHighBounceHeight;
		[Tooltip("Calculated down hit bounce force")]
		[field: SerializeField, ReadOnly] public float attackDownBounceForce;
		[Tooltip("Calculated down hit high bounce force")]
		[field: SerializeField, ReadOnly] public float attackDownHighBounceForce;

		[Space(10)]
		[Tooltip("Time within which too early attack will still be performed")]
		[Range(0f, 0.5f)] public float inputBufferTime; 
	}
	[System.Serializable] public struct Run
	{
		[Tooltip("Layers that can be run on")]
		public LayerManager.LayerMaskInput groundLayers;
		[Tooltip("Layer detecting whether ground is a slope")]
		public LayerManager.LayerMaskInput slopeLayer;
		[Tooltip("Maximum running speed")]
		public float maxSpeed;
		[Tooltip("Acceleration rate (0 = none, maxSpeed = instant)")]
		public float acceleration;
		[Tooltip("Deceleration rate (0 = none, maxSpeed = instant)")]
		public float deceleration;
		[Tooltip("Stickiness towards ground (to prevent bouncing on slopes)")]
		public float groundStickiness;

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
		public LayerManager.LayerMaskInput layers;
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
		public LayerManager.LayerMaskInput canJumpLayer;
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
		public LayerManager.LayerMaskInput layers;

		[Space(10)]

		[Tooltip("Time within which too early pass will still be performed")]
		[Range(0.01f, 0.5f)] public float inputBufferTime;
	}
	[System.Serializable] public struct Detection
	{
		[Tooltip("Layer detecting whether the ground can damage")]
		public LayerManager.LayerMaskInput canDamageLayer;
		[Tooltip("Layer detecting whether the ground can drop")]
		public LayerManager.LayerMaskInput canDropLayer;
	}

	public Gravity gravity;
	public Hurt hurt;
	public Attack attack;
	public Run run;
	public Jump jump;
	public Wall wall;
	public Dash dash;
	public PlatformPassing platformPassing;
	public Detection detection;

	private void OnValidate()
	{
		gravity.strength = -(2 * jump.maxHeight) / (jump.timeToApex * jump.timeToApex);
		jump.force = -gravity.strength * jump.timeToApex;
		
		attack.attackDownBounceForce = Mathf.Sqrt(2 * -gravity.strength * attack.attackDownBounceHeight);
		attack.attackDownHighBounceForce = Mathf.Sqrt(2 * -gravity.strength * attack.attackDownHighBounceHeight);

		hurt.knockbackForce = -gravity.strength * hurt.distressTime / (2 * hurt.knockbackHeightScale);
		hurt.knockbackHeight = (hurt.knockbackForce * hurt.knockbackForce) / (2 * -gravity.strength);

		gravity.scale = gravity.strength / Physics2D.gravity.y;

		dash.force = dash.distance / dash.time;

		float fixedUpdateFrequency = 1f / Time.fixedDeltaTime;

		run.accelerationForce = (fixedUpdateFrequency * run.acceleration) / run.maxSpeed;
		run.decelerationForce = (fixedUpdateFrequency * run.deceleration) / run.maxSpeed;

		wall.slideAccelerationForce = (fixedUpdateFrequency * wall.slideAcceleration) / wall.slideMaxSpeed;
		wall.slideDecelerationForce = (fixedUpdateFrequency * wall.slideDeceleration) / wall.slideMaxSpeed;
	}
}