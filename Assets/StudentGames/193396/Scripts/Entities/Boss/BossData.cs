using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Boss")]
	public class BossData : ScriptableObject
	{
		[Tooltip("Layers to chase")]
		public RuntimeSettings.LayerMaskInput targetLayers;

		[Tooltip("Heal cooldown when not active")]
		public float passiveHealCooldown = 0.1f;
		[Tooltip("Rest time between attacks")]
		public float restTime = 5f;
	}
}