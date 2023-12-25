using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Behavior/Collect")]
	public class CollectBehaviorData : ScriptableObject
	{
		[Space(10)]

		[Tooltip("Layers that can collect")]
		public RuntimeSettings.LayerMaskInput collectingLayers;
		[Tooltip("Name of the collected item")]
		public string eventName;
	}
}