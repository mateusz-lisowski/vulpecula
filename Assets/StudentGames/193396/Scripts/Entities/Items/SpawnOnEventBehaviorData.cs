using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Behavior/SpawnOnEvent")]
	public class SpawnOnEventBehaviorData : ScriptableObject
	{
		[System.Serializable]
		public class Spawnable
		{
			public GameManager.RuntimeGroup group = GameManager.RuntimeGroup.Effects;
			public GameObject prefab;
		}

		[Space(10)]

		[Tooltip("Name of the event triggering spawning")]
		public string eventName;
		[Tooltip("Objects to spawn on death")]
		public Spawnable[] objects;
	}
}