using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private static GameManager activeInstance = null;
	public static GameManager instance 
	{ 
		get 
		{
			if (activeInstance == null || activeInstance.gameObject.IsDestroyed())
			{
				GameObject gameObject = GameObject.FindWithTag("GameController");
				activeInstance = gameObject.GetComponent<GameManager>();
				activeInstance.initialize();
			}
			return activeInstance;
		} 
	}

	public RuntimeDataManager runtimeDataInstance = null;
    public Effects effectsInstance;
    public LayerManager layerManager;

	public enum RuntimeGroup { Effects, Enemies, Collectibles, Projectiles, Disinherited }
	public Dictionary<RuntimeGroup, Transform> runtimeGroup { get; private set; }

	
	private void initialize()
	{
		var runtimeDataObject = GameObject.Find("Runtime Data");
		if (runtimeDataObject == null)
		{
			runtimeDataObject = new GameObject("Runtime Data");
			runtimeDataObject.AddComponent<RuntimeDataManager>();
		}
		runtimeDataInstance = runtimeDataObject.GetComponent<RuntimeDataManager>();

		runtimeGroup = new Dictionary<RuntimeGroup, Transform>
		{
			{ RuntimeGroup.Enemies, new GameObject("Enemies").transform },
			{ RuntimeGroup.Collectibles, new GameObject("Collectibles").transform },
			{ RuntimeGroup.Projectiles, new GameObject("Projectiles").transform },
			{ RuntimeGroup.Effects, new GameObject("Effects").transform },
			{ RuntimeGroup.Disinherited, new GameObject("Disinherited").transform }
		};

		Transform runtimeMainGroup = new GameObject("Runtime").transform;
		foreach (var group in runtimeGroup)
			group.Value.parent = runtimeMainGroup;


		layerManager.installCollisionMatrix();

		foreach (GameObject rootObject in SceneManager.GetActiveScene().GetRootGameObjects())
			layerManager.mapTagsToLayers(rootObject);
	}
	private void Awake()
    {
		_ = GameManager.instance;
	}
	private void OnDestroy()
	{
		layerManager.uninstallCollisionMatrix();
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.T))
			Time.timeScale = 0.2f;
		else
			Time.timeScale = 1f;

		if (Input.GetKeyDown(KeyCode.U))
			Debug.Break();

		if (Input.GetKeyDown(KeyCode.R))
		{
			runtimeDataInstance.transform.parent = transform;
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
