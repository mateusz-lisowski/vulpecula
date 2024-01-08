using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "193396/Data/Behavior/Sleep")]
	public class SleepBehaviorData : ScriptableObject
	{
		[Tooltip("Minimum distance from which can be awaken")]
		public float minWakeDistance = 10f;
		[Tooltip("Distance from which can lock into sleep position")]
		public float lockInDistance = 0.1f;
	}
}