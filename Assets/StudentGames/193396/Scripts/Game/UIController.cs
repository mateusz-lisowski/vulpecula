using TMPro;
using UnityEngine;

namespace _193396
{
	public class UIController : EntityEventReceiver
	{
		public PlayerInfo info;

		private RectTransform healthTransform;
		private RectTransform healthLevelTransform;
		private float healthLevel = 1f;

		private TextMeshProUGUI playtime;


		private void Awake()
		{
			healthTransform = transform.Find("Canvas/Top-Left/health/fluid").GetComponent<RectTransform>();
			healthLevelTransform = transform.Find("Canvas/Top-Left/health/level").GetComponent<RectTransform>();

			playtime = transform.Find("Canvas/Top-Right/playtime").GetComponent<TextMeshProUGUI>();
		}

		private void Update()
		{
			float y = healthLevelTransform.anchoredPosition.y 
				- healthLevelTransform.sizeDelta.y * (1f - healthLevel);

			healthTransform.anchoredPosition = new Vector2(healthTransform.anchoredPosition.x, y);

			playtime.text = string.Format("{0:0.00}", info.playtime());
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