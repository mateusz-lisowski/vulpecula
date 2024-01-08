using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _193396
{
    public class QualityController : MonoBehaviour
    {
        public TextMeshProUGUI label;


        public void increase()
        {
            QualitySettings.IncreaseLevel();
            updateLabel();
        }
        public void decrease()
        {
			QualitySettings.DecreaseLevel();
			updateLabel();
		}

		private void Awake()
		{
            updateLabel();
		}


		private void updateLabel()
        {
            label.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
        }
    }
}