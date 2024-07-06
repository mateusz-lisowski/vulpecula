using System.Collections;
using TMPro;
using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(TextMeshProUGUI))]
    public class LabelCounting : MonoBehaviour
    {
		public UIController infoSource;
		public string valueName;
		[Space(5)]
		public float time;
		public bool wholeNumbers = true;

		private TextMeshProUGUI label;
		private string prefix;


		private void Awake()
		{
			label = GetComponent<TextMeshProUGUI>();

			prefix = label.text;
		}
		private void OnEnable()
		{
			float value = infoSource.getValue(valueName);
			StartCoroutine(count(value));
		}

		private void setLabel(float value, float t)
		{
			float val = value * t;
			if (wholeNumbers)
				label.text = prefix + Mathf.FloorToInt(val).ToString();
			else
				label.text = prefix + val.ToString("0.00");
		}
		private IEnumerator count(float value)
		{
			for (float t = 0f; t < time; t += Time.unscaledDeltaTime)
			{
				setLabel(value, t / time);
				yield return null;
			}

			setLabel(value, 1f);
		}
	}
}