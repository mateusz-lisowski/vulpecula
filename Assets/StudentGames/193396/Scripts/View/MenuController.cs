using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace _193396
{
	public class MenuController : MonoBehaviour
	{
		public bool switchable = true;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isActive { get; private set; }
		[field: SerializeField, ReadOnly] public bool isVisible { get; private set; }

		private Animator animator;
		private CanvasGroup group;

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

			setPage("Main");

			bool wasActive = isActive;
			isActive = val;

			if (isActive != wasActive)
			{
				if (isActive)
				{
					group.gameObject.SetActive(true);
					animator.SetTrigger("onActivate");
				}
				else
					animator.SetTrigger("onDeactivate");
			}
		}
		public void setPage(string pageName)
		{
			MenuPage page = pages.First(p => p.name == pageName);

			if (currentPage.order == page.order)
				return;

			if (page.order > currentPage.order)
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

			inactivePagesParent = transform.Find("Canvas/Inactive");
			currentPageParent = transform.Find("Canvas/Current");
			previousPageParent = transform.Find("Canvas/Previous");

			pages = transform.GetComponentsInChildren<MenuPage>(includeInactive: true);
			currentPage = pages.First(p => p.name == "Main");

			isActive = group.gameObject.activeSelf;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				setActive(!isActive);

			bool wasVisible = isVisible;
			isVisible = group.gameObject.activeSelf && group.alpha != 0f;

			if (GameManager.instance != null)
				if (isVisible != wasVisible)
					if (isVisible)
						GameManager.instance.pushTimeDisable();
					else
						GameManager.instance.popTimeDisable();
		}
		private void LateUpdate()
		{
			if (!group.gameObject.activeSelf)
				group.alpha = 0f;
		}

	}
}