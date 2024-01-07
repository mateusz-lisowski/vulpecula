using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Behavior/JumpAt")]
	public class JumpAtBehaviorData : ScriptableObject
	{
		[Space(10)]

		[Tooltip("Maximum reachable height as a scale of the distance")]
		public float eccentricity = 1f;
		[Tooltip("Minimum reachable height of a jump")]
		public float minHeight = 1f;

		[Space(5)]

		[Tooltip("Minimum time between two consecutive jumps")]
		public float cooldown;

		[Space(10)]

		[Tooltip("Is target automatically reset after landing")]
		public bool autoResetTarget = false;
	}
}