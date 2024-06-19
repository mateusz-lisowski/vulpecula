using UnityEngine;
using UnityEngine.EventSystems;

namespace _193396
{
	public class MenuPage : MonoBehaviour
	{
		[field: SerializeField] public int order { get; private set; }
		[field: SerializeField] public GameObject firstSelectedGameObject { get; private set; }

		[HideInInspector] public GameObject lastSelected;

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
					lastSelected = EventSystem.current.currentSelectedGameObject;
			}

			if (lastSelected == null)
				lastSelected = firstSelectedGameObject;
		}
	}
}