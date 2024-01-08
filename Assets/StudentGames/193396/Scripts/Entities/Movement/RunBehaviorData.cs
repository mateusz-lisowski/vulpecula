using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "193396/Data/Behavior/Run")]
	public class RunBehaviorData : ScriptableObject
	{
		[Space(10)]

		[Tooltip("Maximum running speed")]
		public float maxSpeed = 3.0f;
		[Tooltip("Lerp between current velocity (0) and maxSpeed (1)")]
		[Range(0.0f, 1.0f)] public float accelerationCoefficient = 0.8f;
	}
}