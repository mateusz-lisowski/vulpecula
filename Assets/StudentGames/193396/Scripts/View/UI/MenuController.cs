using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _193396
{
	public class MenuController : MonoBehaviour
	{
		public bool switchable = true;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isActive { get; private set; }
		[field: SerializeField, ReadOnly] public bool isVisible { get; private set; }
		[field: SerializeField, ReadOnly] public bool isNavigating { get; private set; }

		private Animator animator;
		private CanvasGroup group;

		private GameObject navigationController;
		private Selectable navigationControllerSelectable;
		private bool forceNavigation = false;

		private Transform inactivePagesParent;
		private Transform currentPageParent;
		private Transform previousPageParent;

		private MenuPage[] pages;

		private MenuPage currentPage;
		private MenuPage previousPage;


		public void setActive(bool val)
		{
			if (!switchable)
				return;

			bool wasActive = isActive;
			isActive = val;

			if (isActive != wasActive)
			{
				if (isActive)
				{
					isNavigating = false;
					forceNavigation = false;
					pages.All(p => p.lastSelected = null);
					group.gameObject.SetActive(true);
					animator.SetTrigger("onActivate");
					animator.ResetTrigger("onDeactivate");
				}
				else
					animator.SetTrigger("onDeactivate");
			}

			setPage("Main");
		}
		public void setPage(string pageName)
		{
			MenuPage page = pages.First(p => p.name == pageName);

			if (isNavigating)
				forceNavigation = true;
			EventSystem.current.SetSelectedGameObject(null);

			if (currentPage.order == page.order)
				return;

			bool changedBack = page.order < currentPage.order;

			if (!changedBack)
				animator.SetTrigger("onPageChange");
			else
				animator.SetTrigger("onPageChangeReverse");

			if (previousPage != null)
				previousPage.transform.SetParent(inactivePagesParent, false);
			previousPage = currentPage;
			
			if (currentPage != null)
				currentPage.transform.SetParent(previousPageParent, false);
			currentPage = page;
			
			page.transform.SetParent(currentPageParent, false);

			if (!changedBack)
				currentPage.lastSelected = null;
		}
		public void loadLevel()
		{
			MergeController.loadLevel();
		}
		public void loadMenu()
		{
			MergeController.loadMainMenu();
		}
		public void quit()
		{
			QolUtility.Quit();
		}


		private void Awake()
		{
			animator = GetComponent<Animator>();

			group = transform.Find("Canvas").GetComponent<CanvasGroup>();

			navigationController = transform.Find("Canvas/Navigation").gameObject;
			navigationControllerSelectable = navigationController.GetComponent<Selectable>();

			inactivePagesParent = transform.Find("Canvas/Inactive");
			currentPageParent = transform.Find("Canvas/Current");
			previousPageParent = transform.Find("Canvas/Previous");

			pages = transform.GetComponentsInChildren<MenuPage>(includeInactive: true);
			currentPage = pages.First(p => p.name == "Main");

			isActive = group.gameObject.activeSelf;
		}
		private void OnDestroy()
		{
			if (isVisible)
			{
				GameManager.popTimeDisable();
				GameManager.pushCursorHide();
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				setActive(!isActive);

			bool wasVisible = isVisible;
			isVisible = group.gameObject.activeSelf && group.alpha != 0f;

			if (isVisible != wasVisible)
				if (isVisible)
				{
					GameManager.pushTimeDisable();
					GameManager.popCursorHide();
				}
				else
				{
					GameManager.popTimeDisable();
					GameManager.pushCursorHide();
				}
		}
		private void LateUpdate()
		{
			if (!group.gameObject.activeSelf)
				group.alpha = 0f;

			if (isActive)
			{
				if (EventSystem.current.currentSelectedGameObject != null)
				{
					NavigationController pointerUnselect;
					if (EventSystem.current.currentSelectedGameObject.TryGetComponent(out pointerUnselect))
						if (pointerUnselect.checkShouldUnselect())
							EventSystem.current.SetSelectedGameObject(null);
				}

				if (EventSystem.current.currentSelectedGameObject == null)
					if (!forceNavigation)
						EventSystem.current.SetSelectedGameObject(navigationController);
					else
						EventSystem.current.SetSelectedGameObject(currentPage.lastSelected);

				isNavigating = EventSystem.current.currentSelectedGameObject != navigationController;
				forceNavigation = false;

				Selectable selectable;
				if (currentPage.lastSelected != null 
					&& currentPage.lastSelected.TryGetComponent(out selectable))
				{
					Navigation nav = new Navigation();
					nav.mode = Navigation.Mode.Explicit;
					nav.selectOnUp = nav.selectOnDown = nav.selectOnLeft = nav.selectOnRight = selectable;
					navigationControllerSelectable.navigation = nav;
				}
			}
		}

	}
}