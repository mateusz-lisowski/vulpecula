using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "193396/Data/Behavior/Sleep")]
	public class SleepBehaviorData : ScriptableObject
	{
		[Tooltip("Minimum distance from which can be awaken")]
		public float minWakeDistance = 10f;
	}
}