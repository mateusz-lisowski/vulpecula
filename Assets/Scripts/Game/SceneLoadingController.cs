using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingController : MonoBehaviour
{
    public SceneAsset scene;

	private bool justTriggered = false;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Player")
			justTriggered = true;
	}

	private void Update()
	{
		if (justTriggered)
		{
			SceneManager.LoadScene(scene.name);
		}

		justTriggered = false;
	}
}
