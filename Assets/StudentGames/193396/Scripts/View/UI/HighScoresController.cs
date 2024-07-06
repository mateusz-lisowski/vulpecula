using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _193396
{
	public class HighScoresController : MonoBehaviour
	{
		public struct Data : IComparable<Data>
		{
			public int CompareTo(Data other)
			{
				return other.score.CompareTo(score);
			}

			public int score;
			public float time;
		}

		public string sceneName;
		[Space(5)]
		public GameObject scorePrefab;

		private const int maxNumberOfHighScores = 25;
		private const string highScoreKey = "HighScore193396";
		private const string highScoresKey = "HighScores193396";


		public static void addScore(string sceneName, Data data)
		{
			if (sceneName == "193396")
				if (data.score > PlayerPrefs.GetInt(highScoreKey, 0))
					PlayerPrefs.SetInt(highScoreKey, data.score);

			var highScores = readHighScores(sceneName);

			var index = highScores.BinarySearch(data);
			if (index < 0) index = ~index;
			
			if (index + 1 < maxNumberOfHighScores)
			{
				highScores.Insert(index, data);
				if (highScores.Count > maxNumberOfHighScores)
					highScores.RemoveRange(maxNumberOfHighScores, highScores.Count - maxNumberOfHighScores);

				saveHighScores(sceneName, highScores);
			}
		}

		void Start()
		{
			var highScores = readHighScores(sceneName);
			int index = 0;

			foreach (var highScore in highScores)
			{
				++index;

				GameObject element = Instantiate(scorePrefab, transform);
				element.name = index.ToString();
				
				element.transform.Find("index").GetComponent<TextMeshProUGUI>().text = string.Format("{0}.", index);
				element.transform.Find("score").GetComponent<TextMeshProUGUI>().text = string.Format("{0}", highScore.score);
				element.transform.Find("time").GetComponent<TextMeshProUGUI>().text = string.Format("{0:0.00}", highScore.time);
			}
		}


		private static string sceneHighScoresKey(string sceneName)
		{
			if (sceneName == "193396")
				return highScoresKey;
			else
				return string.Format("{0}-{1}", highScoresKey, sceneName);
		}
		private static List<Data> readHighScores(string sceneName)
		{
			string data = PlayerPrefs.GetString(sceneHighScoresKey(sceneName), "");

			List<Data> separatedData = new List<Data>();

			foreach (var element in data.Split(";"))
			{
				string[] subelements = element.Split(",");

				if (subelements.Length < 2)
					continue;

				Data highScore;
				int.TryParse(subelements[0], System.Globalization.NumberStyles.Float, 
					System.Globalization.CultureInfo.InvariantCulture, out highScore.score);
				float.TryParse(subelements[1], System.Globalization.NumberStyles.Float, 
					System.Globalization.CultureInfo.InvariantCulture, out highScore.time);

				separatedData.Add(highScore);
			}

			separatedData.Sort();

			return separatedData;
		}
		private static void saveHighScores(string sceneName, List<Data> separatedData)
		{
			string data = "";

			foreach (var element in separatedData)
				data += string.Format(System.Globalization.CultureInfo.InvariantCulture, 
					"{0},{1:0.00};", element.score, element.time);

			PlayerPrefs.SetString(sceneHighScoresKey(sceneName), data);
		}
	}
}