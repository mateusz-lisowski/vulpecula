using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackController : MonoBehaviour
{
	[field: SerializeField, ReadOnly] public bool isVertical { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public bool hitBouncy { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public Bounds hitboxBounds { get; private set; }

	private Collider2D hitbox;

	private Action<AttackController> hitCallback;
	private LayerMask hitLayers;

	private Vector2 velocity;


	public void setAttack(ScriptableObject data)
	{
		if (data is PlayerData)
			hitLayers = ((PlayerData)data).attack.hitLayers;
		else if (data is EnemyData)
			hitLayers = ((EnemyData)data).attack.hitLayers;
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
		transform.parent = null;
		hitboxBounds = hitbox.bounds;

		if (velocity != Vector2.zero)
			StartCoroutine(Effects.instance.frameMove.run(transform, velocity, 2f));

		ContactFilter2D filter = new ContactFilter2D().NoFilter();
		filter.SetLayerMask(hitLayers);
		filter.useLayerMask = true;

		List<Collider2D> contacts = new List<Collider2D>();
		if (hitbox.OverlapCollider(filter, contacts) == 0)
			return;

		hitBouncy = contacts.Any(c => c.tag == "BreakBounce");

		if (hitCallback != null)
			hitCallback(this);

		foreach (Collider2D contact in contacts)
			contact.SendMessageUpwards("hit", this);
	}

	public void halt()
	{
		Destroy(gameObject);
	}
}
