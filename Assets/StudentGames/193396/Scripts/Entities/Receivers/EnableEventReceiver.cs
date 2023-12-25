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
					target.gameObject.SetActive(true);
					break;
				case Type.Disable:
					target.gameObject.SetActive(false);
					break;
			}
		}
	}
}