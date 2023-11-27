using System.Collections.Generic;
using UnityEngine;

public class CollectData
{
	public int id;
}

public class CollectBehavior : EntityBehavior
{
	public CollectBehaviorData data;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isCollected { get; private set; }


	private Collider2D collectCheck;


	public override void onAwake()
	{
		collectCheck = transform.Find("CollectCheck").GetComponent<Collider2D>();
	}

	public override void onUpdate()
	{
		if (isCollected)
			return;

		if (collectCheck.IsTouchingLayers(data.collectingLayers))
		{
			isCollected = true;
			triggerCollect();

			foreach (var param in controller.animator.parameters)
				if (param.name == "onCollect")
					controller.animator.SetTrigger("onCollect");
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