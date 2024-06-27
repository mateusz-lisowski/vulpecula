using UnityEngine;
using UnityEngine.SceneManagement;

namespace _193396
{
    public class SaveScore : MonoBehaviour
    {
		public UIController infoSource;

		public void save()
        {
            HighScoresController.Data newScore;
            newScore.score = (int)infoSource.getValue("score");
            newScore.time = infoSource.getValue("playtime");

			HighScoresController.addScore(SceneManager.GetActiveScene().name, newScore);
        }
    }
}