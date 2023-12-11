using System.Collections.Generic;
using UnityEngine;

public class CollectData
{
	public int id;
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


	public override void onAwake()
	{
		collectCheck = transform.Find("CollectCheck").GetComponent<Collider2D>();
	}
	public override void onStart()
	{
		runtimeData = RuntimeDataManager.get<RuntimeData>(name);

		if (runtimeData.isCollected)
			Destroy(gameObject);
	}

	public override void onUpdate()
	{
		if (runtimeData.isCollected)
			return;

		if (collectCheck.IsTouchingLayers(data.collectingLayers))
		{
			runtimeData.isCollected = true;
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

		foreach (Collider2D contact in contacts)
			contact.SendMessageUpwards("onMessage", new EntityMessage("collect", collect));
	}

}
