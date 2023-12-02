using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingController : MonoBehaviour
{
    public SceneAsset scene;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Player")
			SceneManager.LoadScene(scene.name);
	}
}
