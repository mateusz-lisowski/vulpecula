using TMPro;
using UnityEngine;

namespace _193396
{
	public class UIController : EntityEventReceiver
	{
		public PlayerInfo info;
		public HurtBehavior bossHealth;

		private RectTransform healthTransform;
		private RectTransform healthLevelTransform;

		private RectTransform bossHealthTransform;
		private RectTransform bossHealthLevelTransform;

		private TextMeshProUGUI playtime;
		private TextMeshProUGUI score;


		private void Awake()
		{
			healthTransform = transform.Find("Canvas/Top-Left/health/fluid").GetComponent<RectTransform>();
			healthLevelTransform = transform.Find("Canvas/Top-Left/health/level").GetComponent<RectTransform>();

			bossHealthTransform = transform.Find("Canvas/Top/Boss/health/fluid").GetComponent<RectTransform>();
			bossHealthLevelTransform = transform.Find("Canvas/Top/Boss/health/level").GetComponent<RectTransform>();

			playtime = transform.Find("Canvas/Top-Right/playtime").GetComponent<TextMeshProUGUI>();
			score = transform.Find("Canvas/Top-Right/score").GetComponent<TextMeshProUGUI>();
		}

		private void Update()
		{
			float healthY = healthLevelTransform.anchoredPosition.y - healthLevelTransform.rect.height * (1f - info.healthNormalized);
			healthTransform.anchoredPosition = new Vector2(healthTransform.anchoredPosition.x, healthY);

			float bossHealthX = bossHealthLevelTransform.anchoredPosition.x - bossHealthLevelTransform.sizeDelta.x * (1f - bossHealth.healthNormalized);
			bossHealthTransform.anchoredPosition = new Vector2(bossHealthX, bossHealthTransform.anchoredPosition.y);

			playtime.text = string.Format("{0:0.00}", info.playtime);
			score.text = string.Format("Score: {0}", info.score);
		}


		public override string[] capturableEvents => new string[] { };
		public override void onEvent(string eventName, object eventData)
		{
		}
	}
}