using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapHelper
{
	public struct TileData
	{
		public Tilemap parent;
		public Vector3Int coord;
		public TileBase tile;
		public Color color;
		public Matrix4x4 transform;
		public Sprite sprite;
		public Vector2 offset;

		public TileData(Tilemap tilemap, Vector3Int triggeredCoord, TileBase tileBase = null)
		{
			parent = tilemap;
			coord = triggeredCoord;
			tile = tileBase != null ? tileBase : tilemap.GetTile(triggeredCoord);
			color = tilemap.GetColor(triggeredCoord);
			transform = tilemap.GetTransformMatrix(triggeredCoord);
			sprite = tilemap.GetSprite(triggeredCoord);
			offset = transform.GetT();
		}
	}

	public static List<Vector3Int> findTriggeredWithinBounds(Tilemap tilemap, Bounds bounds)
	{
		List<Vector3Int> triggeredCoords = new List<Vector3Int>();

		Vector3Int min = tilemap.WorldToCell(bounds.min);
		Vector3Int max = tilemap.WorldToCell(bounds.max) + (Vector3Int)Vector2Int.one;

		for (int i = min.x; i <= max.x; i++)
			for (int j = min.y; j < max.y; j++)
			{
				Vector3Int coord = new Vector3Int(i, j, min.z);

				if (tilemap.GetTile(coord) != null)
					triggeredCoords.Add(coord);
			}

		return triggeredCoords;
	}
	public static List<Vector3Int> getTriggeredTiles(Tilemap tilemap, Bounds triggerBounds)
	{
		return getTriggeredTiles(tilemap, findTriggeredWithinBounds(tilemap, triggerBounds));
	}
	
	public static List<Vector3Int> getTriggeredTiles(Tilemap tilemap, List<Vector3Int> triggeredCoords)
	{
		for (int i = 0; i < triggeredCoords.Count; i++)
		{
			Vector3Int triggeredCoord = triggeredCoords[i];

			foreach (Vector3Int adjacentCoord in new Vector3Int[]{
				triggeredCoord + Vector3Int.up,
				triggeredCoord + Vector3Int.down,
				triggeredCoord + Vector3Int.left,
				triggeredCoord + Vector3Int.right,
			})
			{
				if (tilemap.GetTile(adjacentCoord) == null)
					continue;

				if (triggeredCoords.Contains(adjacentCoord))
					continue;

				triggeredCoords.Add(adjacentCoord);
			}
		}

		return triggeredCoords;
	}
	public static List<Vector3Int> extendToAdjacent(List<Vector3Int> coords)
	{
		List<Vector3Int> newCoords = new List<Vector3Int>(coords);

		foreach (var coord in coords)
			foreach (Vector3Int adjacentCoord in new Vector3Int[]{
				coord + Vector3Int.up,
				coord + Vector3Int.down,
				coord + Vector3Int.left,
				coord + Vector3Int.right,
			})
			{
				if (newCoords.Contains(adjacentCoord))
					continue;

				newCoords.Add(adjacentCoord);
			}

		return newCoords;
	}

	public static List<TileData> getAllTiles(IEnumerable<Tilemap> tilemaps, List<Vector3Int> triggeredCoords)
	{
		List<TileData> tiles = new List<TileData>();

		foreach (Tilemap tilemap in tilemaps)
			foreach (Vector3Int triggeredCoord in triggeredCoords)
			{
				TileBase tileBase = tilemap.GetTile(triggeredCoord);

				if (tileBase == null)
					continue;

				tiles.Add(new TileData(tilemap, triggeredCoord, tileBase));
			}

		return tiles;
	}

	public static bool isOverlappingLayers(IEnumerable<TileData> tiles, LayerMask layerMask)
	{
		foreach (TileData tile in tiles)
		{
			Vector2 min = tile.parent.CellToWorld(tile.coord);
			Vector2 max = min + (Vector2)tile.parent.cellSize;

			if (Physics2D.OverlapArea(min, max, layerMask))
				return true;
		}

		return false;
	}

	public static void setTile(Tilemap tilemap, TileData tile)
	{
		TileChangeData copiedTile = new TileChangeData
		{
			position = tilemap.WorldToCell(tile.parent.CellToWorld(tile.coord)),
			tile = tile.tile,
			color = tile.color,
			transform = tile.transform
		};

		tilemap.SetTile(copiedTile, true);
	}

	public static int hash(List<Vector3Int> coords)
	{
		return coords.Aggregate(487, (a, b) => a * 31 + b.GetHashCode());
	}
	public static int hash(List<Vector2> path)
	{
		return path.Aggregate(487, (a, b) => a * 31 + b.GetHashCode());
	}


	public struct RegionLayer
	{
		public GameObject gameObject;
		public Tilemap tilemap;
		public List<TileData> tiles;
		public bool alreadyExists;

		public RegionLayer(GameObject region, Tilemap parent)
		{
			tiles = new List<TileData>();
			
			Transform transform = region.transform.Find(parent.name);
			alreadyExists = transform != null;

			if (alreadyExists)
			{
				gameObject = transform.gameObject;
				tilemap = gameObject.GetComponent<Tilemap>();
				return;
			}

			gameObject = new GameObject(parent.name);

			gameObject.transform.parent = region.transform;
			gameObject.layer = parent.gameObject.layer;

			tilemap = gameObject.AddComponent<Tilemap>();

			tilemap.color = parent.color;

			TilemapRenderer tilemapRenderer = gameObject.AddComponent<TilemapRenderer>();
			TilemapRenderer oldTilemapRenderer = parent.gameObject.GetComponent<TilemapRenderer>();
			
			tilemapRenderer.sortingLayerID = oldTilemapRenderer.sortingLayerID;
			tilemapRenderer.sortingOrder = oldTilemapRenderer.sortingOrder;
		}

		public void finalize()
		{
			tilemap.CompressBounds();
		}
	}

	public class Region
	{
		public GameObject gameObject;
		public List<RegionLayer> layers;
		public List<Vector3Int> coords;

		public Region(GameObject baseObject, IEnumerable<TileData> tilesE, List<Vector3Int> triggeredCoords)
		{
			gameObject = baseObject;
			coords = triggeredCoords;

			Dictionary<Tilemap, RegionLayer> dict = new Dictionary<Tilemap, RegionLayer>();
			layers = new List<RegionLayer>();

			foreach (TileData tile in tilesE)
			{
				RegionLayer layer;

				if (!dict.ContainsKey(tile.parent))
				{
					layer = new RegionLayer(gameObject, tile.parent);
					layers.Add(layer);
					dict.Add(tile.parent, layer);
				}
				else
					layer = dict[tile.parent];

				layer.tiles.Add(tile);

				if (!layer.alreadyExists)
					TilemapHelper.setTile(layer.tilemap, tile);
			}

			foreach (var layer in layers)
				layer.finalize();
		}

		public bool contains(List<Vector3Int> triggeredCoords)
		{
			if (triggeredCoords.Count == 0)
				return false;

			return coords.Contains(triggeredCoords[0]);
		}
	
	
		public void emit(ParticleSystem source, float count)
		{
			foreach (var layer in layers)
				foreach (var tile in layer.tiles)
				{
					Rect textureRect = tile.sprite.textureRect;
					Vector2 scale = textureRect.size / tile.sprite.pixelsPerUnit;

					for (float p = count * scale.x * scale.y; p >= Random.Range(0f, 1f); p -= 1)
					{
						Vector2 randPos = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

						Vector2 randOffset = randPos * scale;
						randOffset += tile.sprite.textureRectOffset / tile.sprite.pixelsPerUnit;
						randOffset += 0.5f * Vector2.one;
						randOffset -= tile.sprite.pivot / tile.sprite.pixelsPerUnit;
						randOffset += tile.offset;

						ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams
						{
							position = (Vector2)tile.parent.CellToWorld(tile.coord) + randOffset,
							startColor = tile.sprite.texture.GetPixel(
								Mathf.RoundToInt(textureRect.x + randPos.x * textureRect.width),
								Mathf.RoundToInt(textureRect.y + randPos.y * textureRect.height))
						};

						source.Emit(ep, 1);
					}
				}
		}
	}
}
