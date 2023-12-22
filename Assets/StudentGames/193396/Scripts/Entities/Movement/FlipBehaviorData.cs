using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Behavior/Flip")]
	public class FlipBehaviorData : ScriptableObject
	{
		[Space(10)]

		[Tooltip("Minimum time between two consecutive flips")]
		public float cooldown = 0;
	}
}