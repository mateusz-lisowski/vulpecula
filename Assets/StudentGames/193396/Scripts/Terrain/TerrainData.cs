using UnityEngine;

[CreateAssetMenu(menuName = "Data/Terrain")]
public class TerrainData : ScriptableObject
{
	[System.Serializable] public struct GroundDropping
	{
		[Tooltip("Layers that prevent ground respawn")]
		public LayerManager.LayerMaskInput collidingLayers;
		[Tooltip("Time before droppable platform breaks")]
		public float shakeTime;
		[Tooltip("Time needed for ground to respawn")]
		public float respawnTime;

		[Space(10)]

		[Tooltip("Effect to play on default")]
		public GameObject idleEffectPrefab;
		[Tooltip("Effect to play on shake")]
		public GameObject shakeEffectPrefab;
		[Tooltip("Effect to play on break")]
		public GameObject breakEffectPrefab;
	}
	[System.Serializable] public struct GroundBreaking
	{
		[Tooltip("Layers that prevent ground respawn")]
		public LayerManager.LayerMaskInput collidingLayers;
		[Tooltip("Time needed for ground to respawn")]
		public float respawnTime;

		[Space(10)]

		[Tooltip("Effect to play on break")]
		public GameObject breakEffectPrefab;
	}

	public GroundDropping groundDropping;
	public GroundBreaking groundBreaking;
}
