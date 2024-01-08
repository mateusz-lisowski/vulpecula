using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "193396/Data/Effects")]
	public class Effects : ScriptableObject
	{
		public static Effects instance { get => GameManager.instance.effectsInstance; }

		public Flashing flashing;
		public FrameMove frameMove;
		public Fade fade;


		[Serializable]
		public class Flashing
		{
			[Tooltip("Frequency of a flash")]
			public float frequency;
			[Tooltip("Alpha of an object when flasing")]
			[Range(0f, 1f)] public float alpha;
			[Tooltip("Material of an object on initial flash")]
			public Material burstMaterial;
			[Tooltip("Color of an object on initial flash")]
			public Color burstColor;

			public IEnumerator run(SpriteRenderer target, float time, bool burst = false)
			{
				Color color = target.color;

				int times = (int)Mathf.Round(time * frequency);
				float waitTime = 0.5f / frequency;

				if (burst && times == 0)
					times = 1;

				for (int i = 0; i < times; i++)
				{
					if (i == 0 && burst)
					{
						Material material = target.material;
						target.material = burstMaterial;
						target.color = burstColor;
						yield return new WaitForSeconds(waitTime);
						target.material = material;
					}
					else
					{
						target.color = new Color(color.r, color.g, color.b, alpha);
						yield return new WaitForSeconds(waitTime);
					}

					target.color = new Color(color.r, color.g, color.b, 1f);
					yield return new WaitForSeconds(waitTime);
				}
			}
		}

		[Serializable]
		public class FrameMove
		{
			[Tooltip("Number of frames per second")]
			public float frameRate;

			public IEnumerator run(Transform target, Vector3 speed, float time)
			{
				int times = time == float.PositiveInfinity ? int.MaxValue : Mathf.RoundToInt(time * frameRate);
				float waitTime = 1f / frameRate;
				Vector3 frameSpeed = speed / frameRate;

				for (int i = 0; i < times; i++)
				{
					yield return new WaitForSeconds(waitTime);
					target.position += frameSpeed;
				}
			}
		}

		[Serializable]
		public class Fade
		{
			[Tooltip("Distance before fully fading")]
			public float distance;
			[Tooltip("Time to fade for")]
			public float time;
			[Tooltip("Frequency of fade updates")]
			public float updateFrequency;

			[Space(10)]

			[Tooltip("Direction of fading")]
			public Vector3 direction = Vector3.down;
			[Tooltip("Change of alpha when fully faded")]
			public float alphaDelta = 1f;


			public IEnumerator run(SpriteRenderer target,
								   Func<bool> stop = null, bool revert = false, bool move = true)
			{
				yield return runImpl(target.gameObject, (float delta) => {

					Color color = target.color;
					color.a = color.a + delta;
					target.color = color;

				}, stop, revert, move);
			}
			public IEnumerator run(GameObject target, IList<TilemapHelper.RegionLayer> tilemaps,
								   Func<bool> stop = null, bool revert = false, bool move = true)
			{
				yield return runImpl(target, (float delta) => {

					foreach (TilemapHelper.RegionLayer layer in tilemaps)
					{
						Color color = layer.tilemap.color;
						color.a = color.a + delta;
						layer.tilemap.color = color;
					}

				}, stop, revert, move);
			}
		
			private IEnumerator runImpl(GameObject target, Action<float> changeAlpha, 
										Func<bool> stop, bool revert, bool move)
			{
				int times = (int)Mathf.Round(time * updateFrequency);
				float waitTime = 1f / updateFrequency;

				float updateDistance = (!revert ? distance : -distance) / times;
				float updateColor = (!revert ? alphaDelta : -alphaDelta) / times;

				bool stopping;

				for (int i = 0; i < times; i = stopping ? i - 1 : i + 1)
				{
					yield return new WaitForSeconds(waitTime);
					stopping = stop != null && stop();

					if (stopping && i == 0)
					{
						yield return new WaitWhile(stop);
						stopping = false;
					}

					if (move)
					{
						Vector3 position = target.transform.position;
						position += (!stopping ? updateDistance : -updateDistance) * direction;
						target.transform.position = position;
					}

					changeAlpha((!stopping ? -updateColor : updateColor));
				}
			}
		}
	}
}