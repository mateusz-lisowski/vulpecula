using UnityEngine;
using UnityEngine.Tilemaps;

public class HideOnAwake : MonoBehaviour
{
	private void Awake()
	{
		hide(GetComponent<SpriteRenderer>());
		hide(GetComponent<TilemapRenderer>());
	}

	private void hide(Renderer renderer)
	{
		if (renderer == null)
			return;

		renderer.enabled = false;
	}
}
