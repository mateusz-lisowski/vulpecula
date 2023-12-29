using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

namespace _193396
{
	public class EnableEventReceiver : EntityEventReceiver
	{
		public enum Type
		{
			Enable, Disable
		}

		public Type type = Type.Enable;
		public string eventName;
		public string eventData;
		[Space(5)]
		public string targetPath;
		[Space(5)]
		public float timeOffset = 0f;

		private Transform target;


		private void Awake()
		{
			target = transform.Find(targetPath);
		}

		public override string[] capturableEvents => new string[] { eventName };
		public override void onEvent(string eventName, object eventData)
		{
			if (!InputtableEvent.matches(this.eventName, this.eventData, eventName, eventData))
				return;

			switch (type)
			{
				case Type.Enable:
					if (timeOffset == 0f)
						target.gameObject.SetActive(true);
					else
						StartCoroutine(activate(true));
					break;
				case Type.Disable:
					if (timeOffset == 0f)
						target.gameObject.SetActive(false);
					else
						StartCoroutine(activate(false));
					break;
			}
		}


		private IEnumerator activate(bool active)
		{
			yield return new WaitForSeconds(timeOffset);

			target.gameObject.SetActive(active);
		}
	}
}