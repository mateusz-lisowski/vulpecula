using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace _193396
{
	[RequireComponent(typeof(Slider))]
    public class SliderAudioController : MonoBehaviour
    {
        public AudioMixer mixer;
		public string variable;

		private Slider slider;


		public void setLevel()
		{
			float val = slider.normalizedValue * 0.999f + 0.001f;

			mixer.SetFloat(variable, Mathf.Log10(val) * 20f);	
		}

		private void Awake()
		{
			slider = GetComponent<Slider>();

			float normalizedValue;
			mixer.GetFloat(variable, out normalizedValue);
			normalizedValue = Mathf.Clamp(Mathf.Pow(10f, normalizedValue / 20f) / 0.999f - 0.001f, 0f, 1f);

			slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, normalizedValue);
		}
	}
}