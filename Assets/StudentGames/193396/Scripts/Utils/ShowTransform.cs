using UnityEngine;

namespace _193396
{
	public class ShowTransform : MonoBehaviour
	{
		public Color color = Color.green;
		public bool showAlways = false;

		private void draw()
		{
			Gizmos.color = color;
			Gizmos.DrawWireCube(transform.position, transform.lossyScale);
		}

		private void OnDrawGizmosSelected()
		{
			if (!showAlways)
				draw();
		}
		private void OnDrawGizmos()
		{
			if (showAlways)
				draw();
		}
	}
}