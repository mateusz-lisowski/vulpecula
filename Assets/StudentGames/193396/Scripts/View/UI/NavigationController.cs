using UnityEngine;
using UnityEngine.EventSystems;

namespace _193396
{
	public class NavigationController : MonoBehaviour, IPointerDownHandler
	{
		private bool shouldUnselect = false;
		
		public bool checkShouldUnselect()
		{
			bool result = shouldUnselect;
			shouldUnselect = false;
			return result;
		}


		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			shouldUnselect = true;
		}
	}
}