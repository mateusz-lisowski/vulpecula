using UnityEngine;

[CreateAssetMenu(menuName = "Data/Behavior/Jump")]
public class JumpBehaviorData : ScriptableObject
{
	[Tooltip("Jump speed")]
	public float jumpSpeed = 3.0f;
	[Tooltip("Maximum reachable height of a jump")]
	public float maxHeight;
	[Tooltip("Minimum time between two consecutive jumps")]
	public float cooldown;

	[Space(5)]

	[Tooltip("Calculated jump force")]
	[field: SerializeField, ReadOnly] public float force;


	private void OnValidate()
	{
		force = Mathf.Sqrt(2.0f * -Physics2D.gravity.y * maxHeight);
	}
}
