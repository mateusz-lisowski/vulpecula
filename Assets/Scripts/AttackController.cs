using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AttackController : MonoBehaviour
{
	public LayerMask hitLayer;
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public bool isVertical { get; private set; }

	private Transform sprite;
	private Collider2D hitbox;

	private Action<AttackController> hitCallback;


	public void setVertical(bool val = true)
	{
		isVertical = val;
	}

	private IEnumerator hitboxEnabler(float start, float lasts)
	{
		yield return new WaitForSeconds(start);

		hitbox.enabled = true;
		yield return new WaitForSeconds(lasts);

		hitbox.enabled = false;
	}
	public void setCollisionTime(float start, float lasts)
	{
		hitbox.enabled = false;
		StartCoroutine(hitboxEnabler(start, lasts));
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
		sprite = transform.Find("Sprite");
		hitbox = transform.Find("Hitbox").GetComponent<Collider2D>();
	}

	void Update()
	{
		if (hitbox.IsTouchingLayers(hitLayer))
		{
			if (hitCallback != null)
				hitCallback(this);
		}

		if (sprite.IsDestroyed())
			Destroy(gameObject);
	}
}
