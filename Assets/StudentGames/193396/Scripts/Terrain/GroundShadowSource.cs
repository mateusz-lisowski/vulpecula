using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(CompositeCollider2D))]
public class GroundShadowSource : MonoBehaviour
{
	private CompositeCollider2D lightCollider;
	private uint currentHash;

	private void Awake()
	{
		lightCollider = GetComponent<CompositeCollider2D>();
	}
	private void Update()
	{
		uint hash = lightCollider.GetShapeHash();
		if (hash == currentHash && transform.childCount >= lightCollider.pointCount)
			return;
		currentHash = hash;

		HashSet<GameObject> shadowCasters = new HashSet<GameObject>();

		foreach (var shadowCaster in transform.GetComponentsInChildren<GroundShadowCaster>())
			shadowCasters.Add(shadowCaster.gameObject);

		for (int i = 0; i < lightCollider.pathCount; i++)
		{
			List<Vector2> points = new List<Vector2>();
			lightCollider.GetPath(i, points);

			Transform newShadowCaster;
			if (!QolUtility.createIfNotExist(out newShadowCaster, transform,
				RuntimeDataManager.getUniqueName(gameObject, TilemapHelper.hash(points))))
			{
				shadowCasters.Remove(newShadowCaster.gameObject);
				continue;
			}

			GroundShadowCaster caster = newShadowCaster.AddComponent<GroundShadowCaster>();
			caster.setShape(this, points);
		}

		foreach (var shadowCaster in shadowCasters)
			QolUtility.DestroyExecutableInEditMode(shadowCaster);
	}
}
