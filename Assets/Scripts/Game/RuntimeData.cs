using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RuntimeDataManager : MonoBehaviour
{
	public abstract class Data { }
	private Dictionary<string, Data> runtimeData = new Dictionary<string, Data>();

	public static T get<T>(string name) where T : Data, new()
	{
		var runtimeData = GameManager.instance.runtimeDataInstance.runtimeData;

		if (runtimeData.ContainsKey(name))
			return runtimeData[name] as T;
		else
		{
			var newData = new T();
			runtimeData.Add(name, newData);
			return newData;
		}
	}

	public static void setUniqueName(ref GameObject gameObject, GameObject prefab, Tilemap source, Vector3Int coord)
	{
		gameObject.name = string.Format("{0} ({1}, {2}): \"{3}\" ({4})",
			prefab == null ? gameObject.name : prefab.name, 
			coord.x, coord.y, source.name, source.gameObject.scene.name);
	}

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}
