using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class AttachEventReceiver : MonoBehaviour
	{
		public EntityBehaviorController attachedEntity;
		public List<string> receivableEvents;

		private void Start()
		{
			foreach (var receiver in transform.GetComponentsInChildren<EntityEventReceiver>())
				attachedEntity.addEventReceiver(receiver, receivableEvents);
		}
		private void OnDestroy()
		{
			foreach (var receiver in transform.GetComponentsInChildren<EntityEventReceiver>())
				attachedEntity.removeEventReceiver(receiver);
		}
	}
}