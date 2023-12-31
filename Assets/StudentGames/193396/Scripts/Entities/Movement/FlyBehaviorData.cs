using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Behavior/Fly")]
	public class FlyBehaviorData : ScriptableObject
	{
		[Space(10)]

		[Tooltip("Minimum distance to avoid obstacles")]
		public float safeSpaceSmall = 2.5f;
		[Tooltip("Minimum distance to avoid obstacles")]
		public float safeSpaceBig = 2.5f;

		[Space(5)]

		[Tooltip("Flying speed while avoiding obstacles")]
		public float avoidSpeed = 3.0f;
		[Tooltip("Flying speed while falling")]
		public float fallSpeed = 3.0f;
		[Tooltip("General flying speed")]
		public float flySpeed = 3.0f;

		[Space(5)]

		[Tooltip("Lerp between current velocity (0) and maxSpeed (1)")]
		[Range(0.0f, 1.0f)] public float accelerationCoefficient = 0.8f;
		[Tooltip("Constant to counteract fall force")]
		[Range(0.0f, 1.0f)] public float fallRate = 0.3f;
	}
}