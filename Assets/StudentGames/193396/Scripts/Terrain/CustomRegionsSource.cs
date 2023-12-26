using UnityEngine.Tilemaps;

namespace _193396
{
	public class CustomRegionSource : GroundController
	{
		public TerrainData data;
		public override Tilemap[] tilemaps { get => targettedTilemaps; }

		public Tilemap[] targettedTilemaps;
	}
}