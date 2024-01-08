using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "193396/Data/Behavior/Chase")]
	public class ChaseBehaviorData : ScriptableObject
	{
		[Tooltip("Layers to chase")]
		public RuntimeSettings.LayerMaskInput targetLayers;
		[Tooltip("Layers that hide the target")]
		public RuntimeSettings.LayerMaskInput obstructLayers;

		[Space(10)]

		[Tooltip("Maximum distance to detect and chase the target")]
		public float maxDistance = 10f;
		[Tooltip("Minimum distance to detect and chase the target")]
		public float minDistance = 3f;

		[Space(5)]

		[Tooltip("Time to chase a lost target")]
		public float determinationTime = 6f;
		[Tooltip("Lose determination when cought the target")]
		public bool canCatch = true;
	}
}