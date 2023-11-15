using UnityEngine;

[CreateAssetMenu(menuName = "Data/Terrain")]
public class TerrainData : ScriptableObject
{
	[System.Serializable] public struct GroundDropping
	{
		[Tooltip("Layers that prevent ground respawn")]
		public LayerMask collidingLayers;
		[Tooltip("Time before droppable platform breaks")]
		public float shakeTime;
		[Tooltip("Time needed for ground to respawn")]
		public float respawnTime;
	}
	[System.Serializable] public struct GroundBreaking
	{
		[Tooltip("Layers that prevent ground respawn")]
		public LayerMask collidingLayers;
		[Tooltip("Time needed for ground to respawn")]
		public float respawnTime;
	}

	public GroundDropping groundDropping;
	public GroundBreaking groundBreaking;
}
