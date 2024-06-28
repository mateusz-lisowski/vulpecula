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


		public float getValue(string name)
		{
			switch (name)
			{
				case "collectibles": return info.collectibles;
				case "collectibles2": return info.collectibles2;
				case "damageTaken": return info.damageTaken;
				case "killCount": return info.killCount;
				case "deathCount": return info.deathCount;
				case "playtime": return info.playtime;
				case "score": return info.score();
				default: return -1;
			}
		}

		private void Awake()
		{
			healthTransform = transform.Find("Canvas/Top-Left/health/fluid").GetComponent<RectTransform>();
			healthLevelTransform = transform.Find("Canvas/Top-Left/health/level").GetComponent<RectTransform>();

			if (bossHealth != null)
			{
				bossHealthTransform = transform.Find("Canvas/Top/Boss/health/fluid").GetComponent<RectTransform>();
				bossHealthLevelTransform = transform.Find("Canvas/Top/Boss/health/level").GetComponent<RectTransform>();
			}

			playtime = transform.Find("Canvas/Top-Right/playtime").GetComponent<TextMeshProUGUI>();

			GameManager.pushCursorHide();
		}
		private void OnDestroy()
		{
			GameManager.popCursorHide();
		}

		private void Update()
		{
			float healthY = healthLevelTransform.anchoredPosition.y 
				- healthLevelTransform.rect.height * (1f - info.healthNormalized);
			healthTransform.anchoredPosition = new Vector2(healthTransform.anchoredPosition.x, healthY);

			if (bossHealth != null)
			{
				float bossHealthX = bossHealthLevelTransform.anchoredPosition.x 
					- bossHealthLevelTransform.rect.width * (1f - bossHealth.healthNormalized);
				bossHealthTransform.anchoredPosition = new Vector2(bossHealthX, bossHealthTransform.anchoredPosition.y);
			}

			playtime.text = string.Format("{0:0.00}", info.playtime);
		}
	}
}