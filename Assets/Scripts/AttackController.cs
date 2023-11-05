using System;
using System.Collections;
using Unity.VisualScripting;
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


	public void hitboxEnable()
	{
		hitbox.gameObject.SetActive(true);
	}
	public void hitboxDisable()
	{
		hitbox.gameObject.SetActive(false);
	}


	private void Awake()
	{
		hitbox = transform.Find("Hitbox").GetComponent<Collider2D>();
	}

	void Update()
	{
		if (hitbox.IsTouchingLayers(hitLayer))
		{
			if (hitCallback != null)
				hitCallback(this);
		}
	}

}
