using UnityEngine;

namespace _193396
{
	public class ShowCursor : MonoBehaviour
	{
		private void OnEnable()
		{
			GameManager.popCursorHide();
		}
		private void OnDisable()
		{
			GameManager.pushCursorHide();
		}
	}
}