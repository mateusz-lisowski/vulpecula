using UnityEngine;
using UnityEngine.SceneManagement;

namespace _193396
{
	public class MergeController : MonoBehaviour
	{
		public static bool isMerged()
		{
			return SceneUtility.GetBuildIndexByScenePath("Scenes/Main Menu") >= 0;
		}
		public static void loadMainMenu()
		{
			int sceneIndex = SceneUtility.GetBuildIndexByScenePath("Scenes/Main Menu");
			if (sceneIndex >= 0)
			{
				SceneManager.LoadSceneAsync(sceneIndex);
			}
			else
			{
				SceneManager.LoadSceneAsync("Main Menu");
			}
		}
		public static void loadLevel()
		{
			SceneManager.LoadSceneAsync("193396");
		}
	}
}