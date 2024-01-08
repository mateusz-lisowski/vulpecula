using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "193396/Data/Behavior/Avoid")]
	public class AvoidBehaviorData : ScriptableObject
	{
		[Tooltip("Layers to avoid")]
		public RuntimeSettings.LayerMaskInput avoidLayers;

		[Space(10)]

		[Tooltip("Strength of avoiding")]
		public float strength = 1f;

		[Space(5)]

		[Tooltip("Maximum distance to avoid at full strength")]
		public float distanceSmall = 5f;
		[Tooltip("Maximum distance to avoid")]
		public float distanceBig = 10f;
	}
}