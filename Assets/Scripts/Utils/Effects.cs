using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(menuName = "Data/Effects")]
public class Effects : ScriptableObject
{
	public static Effects instance;
	
	public Flashing flashing;
	public Fade fade;


	[Serializable] public struct Flashing
	{
		[Tooltip("Frequency of a flash")]
		public float frequency;
		[Tooltip("Alpha of an object when flasing")]
		[Range(0f, 1f)] public float alpha;

		public IEnumerator run(SpriteRenderer target, float time)
		{
			Color color = target.color;

			int times = (int)Mathf.Round(time * frequency);
			float waitTime = 0.5f / frequency;

			for (int i = 0; i < times; i++)
			{
				target.color = new Color(color.r, color.g, color.b, alpha);
				yield return new WaitForSeconds(waitTime);

				target.color = color;
				yield return new WaitForSeconds(waitTime);
			}
		}
	}

	[Serializable] public struct Fade
	{
		[Tooltip("Distance before fully fading")]
		public float distance;
		[Tooltip("Time to fade for")]
		public float time;
		[Tooltip("Frequency of fade updates")]
		public float updateFrequency;

		public IEnumerator run(GameObject target, IList<Tilemap> tilemaps, bool revert = false)
		{
			int times = (int)Mathf.Round(time * updateFrequency);
			float waitTime = 1f / updateFrequency;

			float updateDistance = distance / times;
			float updateColor = 1f / times;

			if (revert)
			{
				updateDistance = -updateDistance;
				updateColor = -updateColor;
			}

			for (int i = 0; i < times; i++)
			{
				yield return new WaitForSeconds(waitTime);

				Vector3 position = target.transform.position;
				position.y -= updateDistance;

				target.transform.position = position;

				foreach (Tilemap tilemap in tilemaps)
				{
					Color color = tilemap.color;
					color.a = color.a - updateColor;
					tilemap.color = color;
				}
			}
		}
	}

	
	public struct Tiles
	{
		public struct Data
		{
			public Tilemap parent;
			public Vector3Int coord;
			public TileBase tile;
		}
		public struct Instance
		{
			public Instance(GameObject instance)
			{
				gameObject = instance;

				tilemaps = new Tilemap[instance.transform.childCount];
				tilemapRenderers = new TilemapRenderer[instance.transform.childCount];

				int i = 0;
				foreach (Transform child in instance.transform)
				{
					tilemaps[i] = child.GetComponent<Tilemap>();
					tilemapRenderers[i] = child.GetComponent<TilemapRenderer>();

					i++;
				}
			}

			public GameObject gameObject;
			public Tilemap[] tilemaps;
			public TilemapRenderer[] tilemapRenderers;
		}

		private static Tilemap copyTilemap(GameObject instance, Tilemap parent)
		{
			GameObject child = new GameObject("Tilemap");
			child.transform.parent = instance.transform;
			child.layer = parent.gameObject.layer;

			Tilemap newTilemap = child.AddComponent<Tilemap>();

			TilemapRenderer newTilemapRenderer = child.AddComponent<TilemapRenderer>();
			TilemapRenderer oldTilemapRenderer = parent.gameObject.GetComponent<TilemapRenderer>();

			newTilemapRenderer.sortingLayerID = oldTilemapRenderer.sortingLayerID;
			newTilemapRenderer.sortingOrder = oldTilemapRenderer.sortingOrder;

			return newTilemap;
		}
		public static Instance instantiate(IEnumerable<Data> tiles, Grid grid)
		{
			GameObject instance = new GameObject("TileEffect");
			instance.transform.parent = grid.transform;

			Dictionary<Tilemap, Tilemap> dict = new Dictionary<Tilemap, Tilemap>();
			
			foreach (Data tile in tiles)
			{
				if (!dict.ContainsKey(tile.parent))
					dict.Add(tile.parent, copyTilemap(instance, tile.parent));

				Tilemap tilemap = dict[tile.parent];

				tilemap.SetTile(tilemap.WorldToCell(tile.parent.CellToWorld(tile.coord)), tile.tile);
			}

			return new Instance(instance);
		}
	}
}
