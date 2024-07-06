using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _193396
{
	public class LevelsMenuController : MonoBehaviour
	{
		public struct Data : IComparable<Data>
		{
			public int CompareTo(Data other)
			{
				return index.CompareTo(other.index);
			}

			public int index;
			public int buildIndex;
			public string name;
			public string sceneName;
		}

		public MenuController menu;
		public Transform pagesParent;
		[Space(5)]
		public GameObject levelUiPrefab;
		public GameObject levelPagePrefab;


		void Start()
		{
			var levels = readLevels();

			foreach (var level in levels)
			{
				GameObject page = Instantiate(levelPagePrefab, pagesParent);
				page.name = string.Format("Level-{0}", level.sceneName);

				page.transform.Find("ui-nested-scrollable-region/label").GetComponent<TextMeshProUGUI>().text = level.name;

				page.transform.GetComponentInChildren<HighScoresController>().sceneName = level.sceneName;

				page.transform.Find("Back").GetComponent<Button>().onClick.AddListener(
					delegate { menu.setPage("Levels"); });

				page.transform.Find("Play").GetComponent<Button>().onClick.AddListener(
					delegate { menu.setPage("Loading"); menu.loadLevel(level.buildIndex); });

				GameObject element = Instantiate(levelUiPrefab, transform);
				element.name = level.sceneName;
				
				element.transform.Find("index").GetComponent<TextMeshProUGUI>().text = string.Format("{0}.", level.index);
				element.transform.Find("name").GetComponent<TextMeshProUGUI>().text = string.Format("{0}", level.name);

				element.transform.Find("enter").GetComponent<Button>().onClick.AddListener(
					delegate { menu.setPage(page.name); });
			}
		}


		public static List<Data> readLevels()
		{
			List<Data> separatedData = new List<Data>();

			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				string path = SceneUtility.GetScenePathByBuildIndex(i);
				string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);

				var match = Regex.Match(sceneName, "([0-9]+)-(.*)");
				if (!match.Success)
					continue;

				Data level;
				level.buildIndex = i;
				level.sceneName = sceneName;

				level.index = int.Parse(match.Groups[1].ToString());
				level.name = match.Groups[2].ToString();

				separatedData.Add(level);
			}

			separatedData.Sort();

			return separatedData;
		}
	}
}