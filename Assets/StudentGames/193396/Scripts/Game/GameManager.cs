using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _193396
{
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
					if (gameObject == null)
						return null;
					activeInstance = gameObject.GetComponent<GameManager>();
					activeInstance.initialize();
				}
				return activeInstance;
			}
		}

		[HideInInspector]
		public RuntimeDataManager runtimeDataInstance = null;
		public Effects effectsInstance;
		public RuntimeSettings runtimeSettings;

		public enum RuntimeGroup { Effects, Enemies, Collectibles, Projectiles, Disinherited }
		public Dictionary<RuntimeGroup, Transform> runtimeGroup { get; private set; }

		[field: Space(10)]
		[field: SerializeField, ReadOnly] private int currentTimeDisablersCount = 0;


		public void pushTimeDisable() => ++currentTimeDisablersCount;
		public void popTimeDisable() => --currentTimeDisablersCount;


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


			runtimeSettings.install();

			foreach (GameObject rootObject in SceneManager.GetActiveScene().GetRootGameObjects())
				runtimeSettings.mapTagsToLayers(rootObject);
		}
		private void Awake()
		{
			_ = GameManager.instance;
		}
		private void OnDestroy()
		{
			runtimeSettings.uninstall();
		}

		private void Update()
		{
			if (currentTimeDisablersCount != 0)
				Time.timeScale = 0f;
			else if (Input.GetKey(KeyCode.T))
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
}