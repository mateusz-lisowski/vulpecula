using UnityEngine;
using UnityEngine.EventSystems;

namespace _193396
{
	public class NavigationController : MonoBehaviour, IPointerDownHandler, ISelectHandler
	{
		private bool shouldUnselect = false;
		private bool shouldScroll = false;
		
		public bool checkShouldUnselect()
		{
			bool result = shouldUnselect;
			shouldUnselect = false;
			return result;
		}
		public bool checkShouldScroll()
		{
			bool result = shouldScroll;
			shouldScroll = false;
			return result;
		}


		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			shouldUnselect = true;
			shouldScroll = false;
		}

		public void OnSelect(BaseEventData eventData)
		{
			shouldScroll = true;
		}
	}
}