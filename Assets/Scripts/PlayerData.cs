using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
	public LayerMask groundLayer;
	public LayerMask enemyAttackLayer;
	public GameObject attackForwardPrefab;

	[Space(5)]

	[Header("Gravity")]
	public float fallGravityMult; // gravityScale multiplier when falling
	public float maxFallSpeed; // maximum fall speed
	[HideInInspector] public float gravityStrength;
	[HideInInspector] public float gravityScale;

	[Space(10)]

	[Header("Hurt")]
	public float hurtInvulTime; // time of invulnerability after getting hit
	public float hurtDistressTime; // time to ignore input after getting hit
	public float hurtKnockbackMaxSpeed; // maximum knockback speed
	[Range(0.5f, 1.0f)] public float hurtKnockbackHeightScale; // knockback height
	[HideInInspector] public float hurtKnockbackForce; // calculated hit knockback force

	[Space(10)]

	[Header("Attack")]
	public float attackCooldown; // minimum time between two consecutive attacks
	public float attackCastTime; // time needed for the attack to hurt
	public float attackLastTime; // time after the cast the attack can hurt

	[Space(5)]
	public float attackVelocityOffsetScale; // attack offset based on the player velocity
	[Range(0.01f, 0.5f)] public float attackInputBufferTime; // time within which too early attack will still be performed

	[Space(10)]

	[Header("Run")]
	public float runMaxSpeed; // maximum running speed
	public float runAcceleration; // acceleration (0 = none, runMaxSpeed = instant)
	public float runDecceleration; // decceleration (0 = none, runMaxSpeed = instant)
	[HideInInspector] public float runAccelAmount; // calculated acceleration force
	[HideInInspector] public float runDeccelAmount; // calculated decceleration force

	[Space(10)]

	[Header("Jump")]
	public float jumpHeight; // maximum reachable height of a jump
	public float jumpTimeToApex; // time to reach the maximum height of a jump
	public float jumpCooldown; // minimum time between two consecutive jumps
	public int jumpsCount; // number of jumps (2 = double jump)
	[HideInInspector] public float jumpForce; // calculated jump force

	[Space(5)]
	public float jumpCutGravityMult; // gravityScale multiplier when released jump button
	public float jumpHangVelocityThreshold; // y velocity at top of the jump below which to "hang"
	[Range(0f, 1)] public float jumpHangGravityMult; // gravity reduction while "hanging"
	public float jumpHangSpeedMult;  // speed increase while "hanging"

	[Space(5)]
	[Range(0.01f, 0.5f)] public float coyoteTime; // time after falling off a platform where you can still jump
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime; // time within which too early jump will still be performed

	[Space(10)]

	[Header("Wall Slide")]
	public bool wallSlideEnabled; // 'true' if can hold, slide, and jump off walls
	public float wallSlideMaxSpeed; // maximum wall slide speed
	public float wallSlideAcceleration; // acceleration (0 = none, wallSlideMaxSpeed = instant)
	public float wallSlideDecceleration; // decceleration (0 = none, wallSlideMaxSpeed = instant)
	[HideInInspector] public float wallSlideAccelAmount; // calculated acceleration force
	[HideInInspector] public float wallSlideDeccelAmount; // calculated decceleration force

	[Space(5)]

	[Header("Wall Jump")]
	public float wallJumpForce; // force to jump off a wall
	[Range(0f, 1f)] public float wallJumpInputReduction; // input reduction while wall jumping
	public float wallJumpTime; // time to reduce the input while wall jumping
	public float wallJumpMinTime; // minimum time of a wall jump (prevent isGrounded)

	[Space(5)]
	[Range(0.01f, 0.5f)] public float wallJumpCoyoteTime; // time after falling off a wall where you can still wall jump

	[Space(10)]

	[Header("Dash")]
	public float dashDistance; // maximum reachable distance of a dash
	public float dashTime; // time to reach the maximum distance of a dash
	public float dashCooldown; // minimum time between two consecutive dashes
	public int dashesCount; // number of dashes
	[HideInInspector] public float dashForce; // calculated dash force

	[Space(5)]
	[Range(0.01f, 0.5f)] public float dashInputBufferTime; // time within which too early dash will still be performed


	private void OnValidate()
	{
		gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
		jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
		hurtKnockbackForce = Mathf.Abs(gravityStrength) * hurtDistressTime * hurtKnockbackHeightScale;

		dashForce = dashDistance / dashTime;

		gravityScale = gravityStrength / Physics2D.gravity.y;

		runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
		runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

		wallSlideAccelAmount = (50 * wallSlideAcceleration) / wallSlideMaxSpeed;
		wallSlideDeccelAmount = (50 * wallSlideDecceleration) / wallSlideMaxSpeed;
	}
}