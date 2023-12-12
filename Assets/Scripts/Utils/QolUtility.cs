using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class QolUtility
{
	public static void setTag(GameObject target, LayerManager.Tag tag)
	{
		LayerManager layerManager = GameManager.instance.layerManager;

		target.tag = layerManager.tagMapping.name(tag);
		layerManager.mapTagsToLayers(target);
	}

	public static bool createIfNotExist(out Transform target, Transform parent, string name)
	{
		target = parent.Find(name);

		if (target == null)
		{
			target = new GameObject(name).transform;
			target.parent = parent;
			return true;
		}
		else
			return false;
	}
	public static bool createIfNotExist(out Transform target, Transform parent, string name, GameObject prefab)
	{
		target = parent.Find(name);

		if (target == null)
		{
			target = QolUtility.Instantiate(prefab, parent).transform;
			target.name = name;
			target.parent = parent;
			return true;
		}
		else
			return false;
	}

	public static GameObject Instantiate(GameObject prefab, Transform parent)
    {
		GameObject gameObject;

#if UNITY_EDITOR
		if (Application.isPlaying)
		{
#endif
			gameObject = Object.Instantiate<GameObject>(prefab, parent);
			GameManager.instance.layerManager.mapTagsToLayers(gameObject);
#if UNITY_EDITOR
		}
		else
		{
			gameObject = PrefabUtility.InstantiatePrefab(prefab, parent).GameObject();
		}
#endif
		return gameObject;
	}
	public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
	{
		GameObject gameObject;

#if UNITY_EDITOR
		if (Application.isPlaying)
		{
#endif
			gameObject = Object.Instantiate<GameObject>(prefab, position, rotation, parent);
			GameManager.instance.layerManager.mapTagsToLayers(gameObject);
#if UNITY_EDITOR
		}
		else
		{
			gameObject = PrefabUtility.InstantiatePrefab(prefab, parent).GameObject();
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
		}
#endif
		return gameObject;
	}

	public static void DestroyExecutableInEditMode(GameObject gameObject)
	{
		if (Application.isPlaying)
			Object.Destroy(gameObject);
		else
			Object.DestroyImmediate(gameObject);
	}
}
