using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Behavior/Jump")]
	public class JumpBehaviorData : ScriptableObject
	{
		[Space(10)]

		[Tooltip("Maximum distance that can jump down")]
		public float maxFall = 3.0f;

		[Tooltip("Height of a longest jump")]
		public float longHeight = 3.0f;
		[Tooltip("Speed of a longest jump")]
		public float longSpeed = 3.0f;

		[Tooltip("Height of a shortest jump")]
		public float shortHeight = 3.0f;
		[Tooltip("Speed of a shortest jump")]
		public float shortSpeed = 3.0f;

		[Tooltip("Minimum time between two consecutive jumps")]
		public float cooldown;

		[Space(10)]

		[Tooltip("Is jumping only at target layers")]
		public bool jumpOnlyAtTargets = false;
		[Tooltip("Layers to jump at")]
		public RuntimeSettings.LayerMaskInput targetLayers;
	}
}