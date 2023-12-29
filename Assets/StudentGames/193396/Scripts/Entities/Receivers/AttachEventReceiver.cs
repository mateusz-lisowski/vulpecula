using UnityEngine;

namespace _193396
{
	public class AttachEventReceiver : MonoBehaviour
	{
		public EntityBehaviorController attachedEntity;

		private void Start()
		{
			foreach (var receiver in transform.GetComponentsInChildren<EntityEventReceiver>())
				attachedEntity.addEventReceiver(receiver);
		}
		private void OnDestroy()
		{
			foreach (var receiver in transform.GetComponentsInChildren<EntityEventReceiver>())
				attachedEntity.removeEventReceiver(receiver);
		}
	}
}