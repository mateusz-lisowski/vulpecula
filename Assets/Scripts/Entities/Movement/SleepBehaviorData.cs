using UnityEngine;

[CreateAssetMenu(menuName = "Data/Behavior/Sleep")]
public class SleepBehaviorData : ScriptableObject
{
	[Tooltip("Distance from which can fall asleep")]
	public float lockInDistance = 0.1f;
}
