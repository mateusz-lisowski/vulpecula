using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AttackController : MonoBehaviour
{
	private Transform sprite;
	private Collider2D hitbox;


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


	private void Awake()
	{
		sprite = transform.Find("Sprite");
		hitbox = transform.Find("Hitbox").GetComponent<Collider2D>();
	}

	// destroy when Sprite is destroyed
	void Update()
	{
		if (sprite.IsDestroyed())
			Destroy(gameObject);
	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(hitbox.transform.position, hitbox.transform.lossyScale);
	}
}
