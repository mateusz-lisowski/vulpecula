using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitData
{
	public Vector3 right;
	public Bounds bounds;
	public bool isVertical;
	public bool bounce;
}

public class ProjectileBehavior : EntityBehavior
{
	private Collider2D hitbox;

	private Action<HitData> hitCallback;
	private LayerMask hitLayers;

	private Vector2 velocity;
	private bool isVertical;


	public void initialize(ScriptableObject data)
	{
		if (data is PlayerData)
			hitLayers = ((PlayerData)data).attack.hitLayers;
		else if (data is MeleeAtackBehaviorData)
			hitLayers = ((MeleeAtackBehaviorData)data).hitLayers;
	}

	public void setVelocity(Vector2 vel)
	{
		velocity = vel;
	}
	public void setVertical(bool val = true)
	{
		isVertical = val;
	}
	public void setHitboxSize(Vector2 size)
	{
		hitbox.transform.localScale = size;

		if (hitbox is CapsuleCollider2D)
		{
			CapsuleCollider2D capsuleHitbox = (CapsuleCollider2D)hitbox;

			if (size.x > size.y)
				capsuleHitbox.direction = CapsuleDirection2D.Horizontal;
			else
				capsuleHitbox.direction = CapsuleDirection2D.Vertical;
		}
	}
	public void setHitCallback(Action<HitData> callback)
	{
		hitCallback = callback;
	}



	public override void onAwake()
	{
		hitbox = controller.hitbox.GetComponent<Collider2D>();
	}

	public override void onEvent(string eventName, object eventData)
	{
		switch (eventName)
		{
			case "resolve": resolve(); break;
			case "halt": halt(); break;
		}
	}


	private void resolve()
	{
		transform.parent = GameManager.instance.runtimeProjectilesGroup;

		if (velocity != Vector2.zero)
			StartCoroutine(Effects.instance.frameMove.run(transform, velocity, 2f));

		ContactFilter2D filter = new ContactFilter2D().NoFilter();
		filter.SetLayerMask(hitLayers);
		filter.useLayerMask = true;

		List<Collider2D> contacts = new List<Collider2D>();
		if (hitbox.OverlapCollider(filter, contacts) == 0)
			return;

		HitData hit = new HitData();
		hit.right = transform.right;
		hit.bounds = hitbox.bounds;
		hit.isVertical = isVertical;
		hit.bounce = contacts.Any(c => c.tag == "BreakBounce");

		if (hitCallback != null)
			hitCallback(hit);

		foreach (Collider2D contact in contacts)
			contact.SendMessageUpwards("onMessage", new EntityMessage("hit", hit));
	}

	public void halt()
	{
		Destroy(gameObject);
	}

}
