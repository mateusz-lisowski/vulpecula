using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(ChaseBehavior))]
	public class RangedAttackBehavior : BaseAttackBehavior
	{
		public RangedAttackBehaviorData data;
		protected override BaseAttackBehaviorData baseData => data;

		private ChaseBehavior chase;


		public override void onAwake()
		{
			base.onAwake();

			chase = controller.getBehavior<ChaseBehavior>();
		}


		protected override bool canAttack()
		{
			return chase.lastSeenTime == 0f;
		}
		private void attackBreak(HitData data)
		{
			data.source.setOnFixedUpdateResolve(false);
			data.source.controller.rigidBody.velocity = Vector2.zero;
			data.source.controller.onEvent("hit", data);
		}
		protected override ProjectileBehavior attackInstantiate(Transform attackTransform)
		{
			GameObject currentAttack = QolUtility.Instantiate(data.attackPrefab,
				attackTransform.position, attackTransform.rotation, transform);

			ProjectileBehavior currentAttackData = currentAttack.GetComponent<ProjectileBehavior>();

			currentAttackData.initialize(controller, data);
			currentAttackData.setStrength(data.strength);
			currentAttackData.setHitboxSize(attackTransform.lossyScale);
			currentAttackData.setHitCallback(attackBreak);
			currentAttackData.setOnFixedUpdateResolve();

			currentAttackData.controller.rigidBody.velocity =
				(chase.lastTargetPosition - (Vector2)transform.position).normalized * data.speed;

			Destroy(currentAttack, data.maxLifetime);

			return currentAttackData;
		}
	}
}