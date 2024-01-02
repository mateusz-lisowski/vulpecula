using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public abstract class BaseAttackBehavior : EntityBehavior
	{
		protected abstract BaseAttackBehaviorData baseData { get; }
		private BaseAttackBehaviorData data => baseData;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isAttacking { get; private set; }
		[field: Space(5)]
		[field: SerializeField, ReadOnly] public float attackCooldown { get; private set; }

		private HurtBehavior hurt;

		private Dictionary<string, Transform> attackTransforms;


		public override void onAwake()
		{
			hurt = controller.getBehavior<HurtBehavior>();

			Transform attackTransform = controller.transform.Find(data.provokeDetectionName);

			attackTransforms = new Dictionary<string, Transform>();

			attackTransforms[data.attackInstantiateEventName] = attackTransform;
			foreach (Transform childTransform in attackTransform)
				attackTransforms[data.attackInstantiateEventName + "." + childTransform.name] = childTransform;
		}

		public override void onDisable()
		{
			isAttacking = false;
		}

		public override string[] capturableEvents => new string[] { "attack", "attackExit" };
		public override void onEvent(string eventName, object eventData)
		{
			string name = eventData == null ? "attack" : eventData as string;

			switch (eventName)
			{
				case "attackExit":
					if (name == data.attackInstantiateEventName)
						isAttacking = false;
					break;
				case "attack":
					if (attackTransforms.ContainsKey(name))
					{
						attackInstantiate(attackTransforms[name]);
						if (name == data.attackInstantiateEventName)
							isAttacking = false;
					}
					break;
			}
		}

		public override void onUpdate()
		{
			attackCooldown -= Time.deltaTime;

			updateAttack();
		}

		public override bool onFixedUpdate()
		{
			if (!isAttacking)
				return false;

			addSmoothForce(data.attackRunSpeed, data.accelerationCoefficient, transform.right);

			return true;
		}


		protected abstract bool canAttack();
		protected abstract void attackInstantiate(Transform attackTransform);

		private void attack()
		{
			isAttacking = true;

			attackCooldown = data.cooldown;

			controller.onEvent("attackBegin", data.attackInstantiateEventName);
		}
		private void updateAttack()
		{
			if (attackCooldown <= 0 && !(hurt != null && hurt.isDistressed) && canAttack())
			{
				attack();
			}
		}
	}
}