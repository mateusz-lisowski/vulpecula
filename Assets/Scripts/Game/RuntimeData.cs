using System.Collections.Generic;
using UnityEngine;

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


	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}
