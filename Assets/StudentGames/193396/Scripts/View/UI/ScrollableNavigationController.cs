using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _193396
{
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollableNavigationController : MonoBehaviour
	{
		private ScrollRect scrollRect;
		private MenuController menuController;

		private void Awake()
		{
			scrollRect = GetComponent<ScrollRect>();
			menuController = GetComponentInParent<MenuController>();
		}

		private void Update()
		{
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				var selected = EventSystem.current.currentSelectedGameObject.transform;

				while (selected != null && selected != transform)
				{
					selected = selected.parent;
				}

				if (selected == transform)
				{
					NavigationController controller;
					if (EventSystem.current.currentSelectedGameObject.TryGetComponent(out controller))
						if (controller.checkShouldScroll())
							scrollToFit(controller);
				}
			}

		}


		private void scrollToFit(NavigationController controller)
		{
			RectTransform selectedRectTransform = controller.GetComponent<RectTransform>();
			RectTransform viewportRectTransform = scrollRect.GetComponent<RectTransform>();
			RectTransform content = scrollRect.content;

			float diff = Mathf.Max(content.rect.height - viewportRectTransform.rect.height, 0);

			float viewportMin = (1 - scrollRect.verticalNormalizedPosition) * diff;
			float viewportMax = viewportMin + viewportRectTransform.rect.height;

			float itemMax = -content.InverseTransformPoint(selectedRectTransform.position).y;
			itemMax += selectedRectTransform.rect.height / 2;
			float itemMin = itemMax - selectedRectTransform.rect.height;

			if (itemMin < viewportMin)
				scrollRect.verticalNormalizedPosition = 1 - itemMin / diff;
			else if (itemMax > viewportMax)
				scrollRect.verticalNormalizedPosition = 1 - (itemMax - viewportRectTransform.rect.height) / diff;
		}
	}
}