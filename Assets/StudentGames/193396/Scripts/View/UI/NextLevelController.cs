using UnityEngine;

namespace _193396
{
	public class NextLevelController : MonoBehaviour
	{
		public MenuController controller;

		private void Awake()
		{
			if (!controller.canLoadLevelRelative(1))
				gameObject.SetActive(false);
		}
	}
}