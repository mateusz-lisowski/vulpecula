using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class GridPreprocessorData
{
	[System.Serializable] public struct BoundedTile
	{
		public BoundsInt offsetBounds
		{
			get
			{
				return new BoundsInt(
					offsetMin.x, offsetMin.y, 0,
					offsetMax.x - offsetMin.x + 1, offsetMax.x - offsetMin.x + 1, 1);
			}
		}

		public Tile tile;
		public Vector2Int offsetMin;
		public Vector2Int offsetMax;
	}
	[System.Serializable] public struct TilePrefabMapping
	{
		public Tile tile;
		public GameObject prefab;
	}

	[System.Serializable] public struct Slopes
	{
		[Tooltip("Tiles that are ground slopes")]
		public BoundedTile[] tiles;
		[Tooltip("The color of Slope tilemap")]
		public Color color;
	}
	[System.Serializable] public struct BounceOnBreak
	{
		[Tooltip("Tiles that are breakable ground that can be bounced off")]
		public BoundedTile[] tiles;
		[Tooltip("The color of BounceOnBreak tilemap")]
		public Color color;

		[Space(5)]

		public Tilemap[] breakableTilemaps;
	}
	[System.Serializable] public struct Enemies
	{
		public TilePrefabMapping[] mapping;
	}
	[System.Serializable] public struct Collectibles
	{
		public TilePrefabMapping[] mapping;
	}

	public TerrainData terrainData;

	[Space(5)]

	public Tile areaTile;
	public string sortingLayer;
	public int sortingOrder;

	[Space(5)]

	public Slopes slopes;
	public BounceOnBreak bounceOnBreak;
	public Enemies enemies;
	public Collectibles collectibles;
}
