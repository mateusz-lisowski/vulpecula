using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Boss")]
	public class BossData : ScriptableObject
	{
		[System.Serializable]
		public struct Hurt
		{
			[Tooltip("Time of invulnerability after getting hit")]
			public float invulnerabilityTime;

			[Space(5)]

			[Tooltip("Cumulative strength of received attacks resulting in death")]
			public int health;
		}

		public Hurt hurt;
	}
}