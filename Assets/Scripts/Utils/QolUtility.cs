using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class QolUtility
{
    public static GameObject Instantiate(GameObject prefab, Transform parent)
    {
#if UNITY_EDITOR
        return PrefabUtility.InstantiatePrefab(prefab, parent).GameObject();
#else
        return Object.Instantiate<GameObject>(prefab, parent);
#endif
    }
}
