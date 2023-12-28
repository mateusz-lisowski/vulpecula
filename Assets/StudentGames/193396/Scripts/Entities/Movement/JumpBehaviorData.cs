using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Behavior/Jump")]
	public class JumpBehaviorData : ScriptableObject
	{
		[Space(10)]

		[Tooltip("Maximum distance that can jump down")]
		public float maxFall = 3.0f;

		[Space(5)]

		[Tooltip("Height of the longest jump")]
		public float longHeight = 3.0f;
		[Tooltip("Speed of the longest jump")]
		public float longSpeed = 3.0f;

		[Tooltip("Height of the shortest jump")]
		public float shortHeight = 3.0f;
		[Tooltip("Speed of the shortest jump")]
		public float shortSpeed = 3.0f;

		[Space(5)]

		[Tooltip("Minimum time between two consecutive jumps")]
		public float cooldown;
	}
}