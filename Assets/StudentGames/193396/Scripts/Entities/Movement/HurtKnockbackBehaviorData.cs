using UnityEngine;

[CreateAssetMenu(menuName = "Data/Behavior/HurtKnockback")]
public class HurtKnockbackBehaviorData : ScriptableObject
{
	[SerializeField] private HurtBehaviorData associatedHurtData;

	[Space(10)]

	[Tooltip("Knockback speed")]
	public float knockbackSpeed = 3.0f;
	[Tooltip("Knockback height (point on parabola when returned input)")]
	[Range(0.5f, 1.0f)] public float knockbackHeightScale = 0.9f;

	[Space(5)]

	[Tooltip("Calculated maximum reachable height of a knockback")]
	[ReadOnly] public float knockbackHeight;
	[Tooltip("Calculated hit knockback force")]
	[ReadOnly] public float knockbackForce;


	private void OnValidate()
	{
		if (associatedHurtData == null)
			return;

		knockbackForce = -Physics2D.gravity.y * associatedHurtData.distressTime / (2 * knockbackHeightScale);
		knockbackHeight = (knockbackForce * knockbackForce) / (2 * -Physics2D.gravity.y);
	}
}
