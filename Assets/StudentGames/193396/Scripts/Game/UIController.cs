using UnityEngine;

namespace _193396
{
	public class UIController : EntityEventReceiver
	{
		private RectTransform healthTransform;
		private RectTransform healthLevelTransform;
		private float healthLevel = 1f;


		private void Awake()
		{
			healthTransform = transform.Find("Canvas/health/fluid").GetComponent<RectTransform>();
			healthLevelTransform = transform.Find("Canvas/health/level").GetComponent<RectTransform>();
		}

		private void Update()
		{
			float y = healthLevelTransform.anchoredPosition.y 
				- healthLevelTransform.sizeDelta.y * (1f - healthLevel);

			healthTransform.anchoredPosition = new Vector2(healthTransform.anchoredPosition.x, y);
		}


		public override string[] capturableEvents => new string[] { "hurt" };
		public override void onEvent(string eventName, object eventData)
		{
			switch(eventName)
			{
				case "hurt":
					healthLevel = (float)eventData;
					break;
			}
		}
	}
}