using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace _193396
{
	[RequireComponent(typeof(Image))]
	public class UIAnimation : MonoBehaviour
	{
		public Sprite[] frames;
		public float frameRate = 12;

		private Image image;

		private int currentFrame = 0;
		private float lastFrameChangeTime;


		private void Awake()
		{
			image = GetComponent<Image>();
			lastFrameChangeTime = 0f;
		}

		private void Update()
		{
			lastFrameChangeTime += Time.deltaTime;
			
			while (lastFrameChangeTime > 1f / frameRate)
			{
				lastFrameChangeTime -= 1f / frameRate;
				
				currentFrame++;
				if (currentFrame >= frames.Length)
					currentFrame = 0;

				if (frames.Length != 0)
					image.sprite = frames[currentFrame];
			}
		}
	}
}