using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _193396
{
	public class HitData
	{
		public ProjectileBehavior source;
		public Vector3 right;
		public Bounds bounds;
		public int strength;
		public bool isVertical;
		public bool bounce;
	}

	public class ProjectileBehavior : EntityBehavior
	{
		private Collider2D hitbox;

		private Action<HitData> hitCallback;
		private LayerMask hitLayers;

		private Vector2 frameVelocity;
		private bool isVertical;
		private int strength = 1;
		private bool onFixedUpdateResolve = false;


		public void initialize(ScriptableObject data)
		{
			if (data is PlayerData)
				hitLayers = ((PlayerData)data).attack.hitLayers;
			else if (data is MeleeAttackBehaviorData)
				hitLayers = ((MeleeAttackBehaviorData)data).hitLayers;
			else if (data is RangedAttackBehaviorData)
				hitLayers = ((RangedAttackBehaviorData)data).hitLayers;
			else
				throw new ApplicationException("Unknown projectile data.");
		}

		public void setFrameVelocity(Vector2 vel)
		{
			frameVelocity = vel;
		}
		public void setVertical(bool val = true)
		{
			isVertical = val;
		}
		public void setStrength(int val)
		{
			strength = val;
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
		public void setOnFixedUpdateResolve(bool val = true)
		{
			onFixedUpdateResolve = val;
		}


		public override void onAwake()
		{
			hitbox = controller.hitbox.GetComponent<Collider2D>();
		}

		public override string[] capturableEvents => new string[] { "resolve", "halt" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "resolve": resolve(); break;
				case "halt": halt(); break;
			}
		}

		public override bool onFixedUpdate()
		{
			if (!onFixedUpdateResolve)
				return false;
			resolve();
			return true;
		}


		private void resolve()
		{
			transform.parent = GameManager.instance.runtimeGroup[GameManager.RuntimeGroup.Projectiles];

			if (frameVelocity != Vector2.zero)
			{
				StartCoroutine(Effects.instance.frameMove.run(transform, frameVelocity, float.PositiveInfinity));
				frameVelocity = Vector2.zero;
			}

			ContactFilter2D filter = new ContactFilter2D().NoFilter();
			filter.SetLayerMask(hitLayers);
			filter.useLayerMask = true;

			List<Collider2D> contacts = new List<Collider2D>();
			if (hitbox.OverlapCollider(filter, contacts) == 0)
				return;

			HitData hit = new HitData();
			hit.source = this;
			hit.right = transform.right;
			hit.bounds = hitbox.bounds;
			hit.strength = strength;
			hit.isVertical = isVertical;
			hit.bounce = contacts.Any(c => c.tag == "Breakable Explode");

			if (hitCallback != null)
				hitCallback(hit);

			foreach (Collider2D contact in contacts)
				contact.SendMessageUpwards("onMessage", new EntityMessage("hit", hit),
					SendMessageOptions.DontRequireReceiver);
		}

		public void halt()
		{
			Destroy(gameObject);
		}

	}
}