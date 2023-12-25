using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _193396
{
    public class FadeEffect : MonoBehaviour
    {
        public enum Type
        {
            FadeIn, FadeOut
        }
		public enum TriggerType
		{
			OnEnable
		}
        public enum TargetType
        {
            Sprite, UIImage
        }

		public Type type = Type.FadeIn;
		public TriggerType triggerType = TriggerType.OnEnable;
		public TargetType targetType = TargetType.Sprite;

        [Space(5)]

        public float time = 1f;
        public float frameRate = 12f;


		private void OnEnable()
		{
            if (triggerType != TriggerType.OnEnable)
                return;

            StartCoroutine(trigger());
		}


		private void addAlpha(float delta)
		{
			switch (targetType)
			{
				case TargetType.Sprite:
					{
						SpriteRenderer target = GetComponent<SpriteRenderer>();
						Color color = target.color;
						color.a = color.a + delta;
						target.color = color;
					}
					break;
				case TargetType.UIImage:
					{
						Image target = GetComponent<Image>();
						Color color = target.color;
						color.a = color.a + delta;
						target.color = color;
					}
					break;
			}
		}
        private IEnumerator trigger()
        {
			int times = Mathf.RoundToInt(time * frameRate);
			float waitTime = 1f / frameRate;
			float updateColor = 1f / times;

			if (type == Type.FadeOut)
				updateColor = -updateColor;

			for (int i = 0; i < times; i++)
			{
				yield return new WaitForSeconds(waitTime);
				addAlpha(updateColor);
			}
		}
	}
}