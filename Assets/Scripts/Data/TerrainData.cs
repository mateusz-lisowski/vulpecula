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
		[Tooltip("Time needed for platform to respawn")]
		public float respawnTime;
	}

	public GroundDropping groundDropping;
}
