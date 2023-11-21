using UnityEngine;

[CreateAssetMenu(menuName = "Data/Behavior/Hurt")]
public class HurtBehaviorData : ScriptableObject
{
	[Space(10)]

	[Tooltip("Time of invulnerability after getting hit")]
	public float invulnerabilityTime = 0.0f;
	[Tooltip("Time to ignore input after getting hit")]
	public float distressTime = 0.3f;
}
