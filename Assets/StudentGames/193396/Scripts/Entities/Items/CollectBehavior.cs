using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class CollectData
	{
		public int id;
		public string name;
		public Vector2 position;
	}

	public class CollectBehavior : EntityBehavior
	{
		public class RuntimeData : RuntimeDataManager.Data
		{
			public bool isCollected = false;
		}

		public CollectBehaviorData data;
		private RuntimeData runtimeData;

		private Collider2D collectCheck;

		private float cooldown = 0f;


		public override void onAwake()
		{
			collectCheck = transform.Find("Detection/Collect").GetComponent<Collider2D>();
		}
		public override void onStart()
		{
			runtimeData = RuntimeDataManager.get<RuntimeData>(name + data.runtimeDataPostfix);

			if (runtimeData.isCollected)
				Destroy(gameObject);
		}

		public override void onUpdate()
		{
			cooldown -= Time.deltaTime;

			if (runtimeData.isCollected || cooldown > 0f)
				return;

			if (collectCheck.IsTouchingLayers(data.collectingLayers))
			{
				if (!data.active)
					runtimeData.isCollected = true;

				cooldown = data.cooldown;
				triggerCollect();

				controller.onEvent("collected", null);
			}
		}


		private void triggerCollect()
		{
			ContactFilter2D filter = new ContactFilter2D().NoFilter();
			filter.SetLayerMask(data.collectingLayers);
			filter.useLayerMask = true;

			List<Collider2D> contacts = new List<Collider2D>();
			if (collectCheck.OverlapCollider(filter, contacts) == 0)
				return;

			CollectData collect = new CollectData();
			collect.id = gameObject.GetInstanceID();
			collect.name = data.eventName;
			collect.position = transform.position;

			foreach (Collider2D contact in contacts)
				contact.SendMessageUpwards("onMessage", new EntityMessage("collect", collect));
		}

	}
}