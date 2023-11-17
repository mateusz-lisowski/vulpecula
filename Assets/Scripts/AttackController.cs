using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
	[field: SerializeField, ReadOnly] public bool isVertical { get; private set; }
	[field: Space(5)]
	[field: SerializeField, ReadOnly] public Bounds hitboxBounds { get; private set; }

	private Collider2D hitbox;

	private Action<AttackController> hitCallback;
	private LayerMask hitLayers;


	public void setAttack(ScriptableObject data)
	{
		if (data is PlayerData)
			hitLayers = ((PlayerData)data).attack.hitLayers;
		else if (data is EnemyData)
			hitLayers = ((EnemyData)data).attack.hitLayers;
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

	void Update()
	{
		//transform.position += new Vector3(velocity.x * Time.deltaTime, 0, 0);	
	}

	public void resolve()
	{
		transform.parent = null;
		hitboxBounds = hitbox.bounds;

		ContactFilter2D filter = new ContactFilter2D().NoFilter();
		filter.SetLayerMask(hitLayers);
		filter.useLayerMask = true;

		List<Collider2D> contacts = new List<Collider2D>();
		if (hitbox.OverlapCollider(filter, contacts) == 0)
			return;

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
