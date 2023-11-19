using UnityEngine;

[CreateAssetMenu(menuName = "Data/Behavior/Hurt")]
public class HurtBehaviorData : ScriptableObject
{
	[Tooltip("Time of invulnerability after getting hit")]
	public float invulnerabilityTime;
	[Tooltip("Time to ignore input after getting hit")]
	public float distressTime;
}
