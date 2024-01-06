using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _193396
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class SliderLabel : MonoBehaviour
    {
		public Slider target;
		public string format = "{0}";

		private TextMeshProUGUI label;


		private void Awake()
		{
			label = GetComponent<TextMeshProUGUI>();
		}

		private void Update()
		{
			label.text = string.Format(format, target.value);
		}
	}
}