using UnityEngine;

[ExecuteInEditMode]
public class GroundShadowSource : MonoBehaviour
{
	private CompositeCollider2D _lightCollider;
	public CompositeCollider2D lightCollider
	{
		get
		{
			if (_lightCollider == null)
				_lightCollider = GetComponent<CompositeCollider2D>();
			return _lightCollider;
		}
	}

	private bool initialized = false;
	private uint currentHash;
	public bool shouldReinitialize
	{
		get
		{
			uint hash = lightCollider.GetShapeHash();

			if (initialized && currentHash == hash)
				return false;

			initialized = false;
			currentHash = hash;
			return true;
		}
	}

	private void Awake()
	{
		_lightCollider = GetComponent<CompositeCollider2D>();
	}
}
