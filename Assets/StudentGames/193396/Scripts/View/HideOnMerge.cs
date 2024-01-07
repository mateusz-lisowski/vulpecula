using UnityEngine;

namespace _193396
{
	public class HideOnMerge : MonoBehaviour
	{
		private void OnEnable()
		{
			if (MergeController.isMerged())
				gameObject.SetActive(false);
		}
	}
}