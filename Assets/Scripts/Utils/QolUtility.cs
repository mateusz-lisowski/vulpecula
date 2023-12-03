using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class QolUtility
{
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
#if UNITY_EDITOR
        return PrefabUtility.InstantiatePrefab(prefab, parent).GameObject();
#else
        return Object.Instantiate<GameObject>(prefab, parent);
#endif
    }

	public static void DestroyExecutableInEditMode(GameObject gameObject)
	{
		if (Application.isPlaying)
			Object.Destroy(gameObject);
		else
			Object.DestroyImmediate(gameObject);
	}
}
