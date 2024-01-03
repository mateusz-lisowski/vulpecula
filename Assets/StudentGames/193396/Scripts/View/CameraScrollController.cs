using UnityEngine;

namespace _193396
{
	public class CameraScrollController : MonoBehaviour
	{
		public float speed = 12f;
		public Vector2 direction = Vector2.up;


		private void Update()
		{
			Vector2 position = transform.position;
			position += direction.normalized * (speed * Time.deltaTime);
			transform.position = position;
		}
	}
}