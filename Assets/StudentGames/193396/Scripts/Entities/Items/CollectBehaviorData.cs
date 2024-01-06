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

		[Space(5)]

		[Tooltip("Can item activate multiple times")]
		public bool active;
		[Tooltip("Time between two consecutive item activations")]
		public float cooldown;

		[Space(10)]

		[Tooltip("Postfix added to runtime data of object to differentiate between collect datas")]
		public string runtimeDataPostfix;
	}
}