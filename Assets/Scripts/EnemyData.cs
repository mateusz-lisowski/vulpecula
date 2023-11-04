using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
	public LayerMask groundLayer;
	public LayerMask playerLayer;
	public LayerMask playerAttackLayer;
	public GameObject attackPrefab;

	[Space(5)]

	[Header("Gravity")]
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

	[Space(10)]

	[Header("Run")]
	public bool runEnabled; // 'true' if can run
	public float runMaxSpeed; // maximum running speed
	public float runAcceleration; // acceleration (0 = none, runMaxSpeed = instant)
	public float runDecceleration; // decceleration (0 = none, runMaxSpeed = instant)
	[HideInInspector] public float runAccelAmount; // calculated acceleration force
	[HideInInspector] public float runDeccelAmount; // calculated decceleration force

	[Space(10)]

	[Header("Jump")]
	public bool jumpEnabled; // 'true' if can jump
	public float jumpHeight; // maximum reachable height of a jump
	public float jumpTimeToApex; // time to reach the maximum height of a jump
	public float jumpCooldown; // minimum time between two consecutive jumps
	[HideInInspector] public float jumpForce; // calculated jump force


	private void OnValidate()
	{
		if (jumpEnabled)
		{
			gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
			jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

			gravityScale = gravityStrength / Physics2D.gravity.y;
		}
		else
			gravityScale = 1;

		hurtKnockbackForce = Mathf.Abs(gravityScale * Physics2D.gravity.y) 
			* hurtDistressTime * hurtKnockbackHeightScale;

		if (runEnabled)
		{
			runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
			runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;
		}
		else
		{
			runAccelAmount = runDeccelAmount = 0;
		}
	}
}
