using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance { get; private set; }
    public Effects effectsInstance;

	public Transform runtimeGroup;


	private void Awake()
    {
        instance = this;

		// Initialize singletons here, because ScriptableObject that is not attached may not be
        // created and instance would not be set.
		Effects.instance = effectsInstance;


		runtimeGroup = new GameObject("Runtime").transform;
    }

	void Update()
	{
		if (Input.GetKey(KeyCode.T))
			Time.timeScale = 0.2f;
		else
			Time.timeScale = 1f;

		if (Input.GetKeyDown(KeyCode.U))
			Debug.Break();
	}
}
