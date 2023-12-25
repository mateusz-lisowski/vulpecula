using TMPro;
using UnityEngine;

namespace _193396
{
	public class UIController : EntityEventReceiver
	{
		public PlayerInfo info;

		private RectTransform healthTransform;
		private RectTransform healthLevelTransform;

		private TextMeshProUGUI playtime;
		private TextMeshProUGUI score;


		private void Awake()
		{
			healthTransform = transform.Find("Canvas/Top-Left/health/fluid").GetComponent<RectTransform>();
			healthLevelTransform = transform.Find("Canvas/Top-Left/health/level").GetComponent<RectTransform>();

			playtime = transform.Find("Canvas/Top-Right/playtime").GetComponent<TextMeshProUGUI>();
			score = transform.Find("Canvas/Top-Right/score").GetComponent<TextMeshProUGUI>();
		}

		private void Update()
		{
			float y = healthLevelTransform.anchoredPosition.y 
				- healthLevelTransform.sizeDelta.y * (1f - info.healthNormalized());

			healthTransform.anchoredPosition = new Vector2(healthTransform.anchoredPosition.x, y);

			playtime.text = string.Format("{0:0.00}", info.playtime());
			score.text = string.Format("Score: {0}", info.score());
		}


		public override string[] capturableEvents => new string[] { };
		public override void onEvent(string eventName, object eventData)
		{
		}
	}
}