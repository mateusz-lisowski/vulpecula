using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class GroundShadowCaster : ShadowCaster2D
{
	static FieldInfo shapeFieldInfo = typeof(ShadowCaster2D).GetField("m_ShapePath",
			BindingFlags.NonPublic | BindingFlags.Instance);
	static FieldInfo shapeHashFieldInfo = typeof(ShadowCaster2D).GetField("m_ShapePathHash",
		BindingFlags.NonPublic | BindingFlags.Instance);

	public void setShape(GroundShadowSource source, List<Vector2> vertices)
	{
		Vector3[] shape = vertices.ConvertAll((point) => new Vector3(point.x, point.y)).ToArray();
		shapeFieldInfo.SetValue(this, shape);
		shapeHashFieldInfo.SetValue(this, (int)Random.Range(0f, 10000000f));

		this.selfShadows = true;
	}

}
