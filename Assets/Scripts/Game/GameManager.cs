using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance { get; private set; }
    public Effects effectsInstance;

	public enum RuntimeGroup { Effects, Enemies, Collectibles, Projectiles }
	public Dictionary<RuntimeGroup, Transform> runtimeGroup { get; private set; }


	private void Awake()
    {
        instance = this;

		// Initialize singletons here, because ScriptableObject that is not attached may not be
        // created and instance would not be set.
		Effects.instance = effectsInstance;


		runtimeGroup = new Dictionary<RuntimeGroup, Transform>
		{
			{ RuntimeGroup.Enemies, new GameObject("Enemies").transform },
			{ RuntimeGroup.Collectibles, new GameObject("Collectibles").transform },
			{ RuntimeGroup.Projectiles, new GameObject("Projectiles").transform },
			{ RuntimeGroup.Effects, new GameObject("Effects").transform }
		};

		Transform runtimeMainGroup = new GameObject("Runtime").transform;
		foreach (var group in runtimeGroup)
			group.Value.parent = runtimeMainGroup;
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.T))
			Time.timeScale = 0.2f;
		else
			Time.timeScale = 1f;

		if (Input.GetKeyDown(KeyCode.U))
			Debug.Break();

		if (Input.GetKeyDown(KeyCode.R))
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
