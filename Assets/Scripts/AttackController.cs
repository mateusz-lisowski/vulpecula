using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
	public LayerMask hitLayer;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isVertical { get; private set; }

	private Collider2D hitbox;

	private Action<AttackController> hitCallback;


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

	public void setHitCallback(Action<AttackController> callback)
	{
		hitCallback = callback;
	}


	private void Awake()
	{
		hitbox = transform.Find("Hitbox").GetComponent<Collider2D>();
	}

	public void resolve()
	{
		ContactFilter2D filter = new ContactFilter2D().NoFilter();
		filter.SetLayerMask(hitLayer);
		filter.useLayerMask = true;

		List<Collider2D> contacts = new List<Collider2D>();
		if (hitbox.OverlapCollider(filter, contacts) == 0)
			return;

		if (hitCallback != null)
			hitCallback(this);

		foreach (Collider2D contact in contacts)
			contact.SendMessageUpwards("hit", this);
	}
}
