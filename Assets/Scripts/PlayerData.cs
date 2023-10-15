using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
	[Header("Gravity")]
	[HideInInspector] public float gravityStrength;
	[HideInInspector] public float gravityScale;

	[Space(5)]
	public float fallGravityMult; // gravityScale multiplier when falling
	public float maxFallSpeed; // maximum fall speed

	[Space(20)]

	[Header("Run")]
	public float runMaxSpeed; // maximum running speed
	public float runAcceleration; // acceleration (0 = none, runMaxSpeed = instant)
	public float runDecceleration; // decceleration (0 = none, runMaxSpeed = instant)
	[HideInInspector] public float runAccelAmount; // calculated acceleration force
	[HideInInspector] public float runDeccelAmount; // calculated decceleration force

	[Space(20)]

	[Header("Jump")]
	public float jumpHeight; // maximum reachable height of a jump
	public float jumpTimeToApex; // time to reach the maximum height of a jump
	public int jumpsCount; // number of jumps (2 = double jump)
	[HideInInspector] public float jumpForce; // calculated jump force

	[Header("Both Jumps")]
	public float jumpCutGravityMult; // gravityScale multiplier when released jump button
	public float jumpHangVelocityThreshold; // y velocity at top of the jump below which to "hang"
	[Range(0f, 1)] public float jumpHangGravityMult; // gravity reduction while "hanging"
	public float jumpHangSpeedMult;  // speed increase while "hanging"

	[Header("Assists")]
	[Range(0.01f, 0.5f)] public float coyoteTime; // time after falling off a platform where you can still jump
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime; // time within which too early jump will still be performed


	private void OnValidate()
	{
		gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
		jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

		gravityScale = gravityStrength / Physics2D.gravity.y;

		runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
		runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;
	}
}