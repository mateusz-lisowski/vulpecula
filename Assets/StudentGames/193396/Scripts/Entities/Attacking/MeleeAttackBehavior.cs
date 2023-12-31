using UnityEngine;

namespace _193396
{
	public class MeleeAtackBehavior : BaseAttackBehavior
	{
		public MeleeAttackBehaviorData data;
		protected override BaseAttackBehaviorData baseData => data;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isProvoked { get; private set; }

		private Collider2D attackCheck;


		public override void onAwake()
		{
			base.onAwake();

			Transform attackTransform = controller.transform.Find(data.provokeDetectionName);
			attackCheck = attackTransform.GetComponent<Collider2D>();
		}
		
		public override void onUpdate()
		{
			updateCollisions();

			base.onUpdate();
		}


		private void updateCollisions()
		{
			if (attackCheck != null)
				isProvoked = attackCheck.IsTouchingLayers(data.provokeLayers);
			else
				isProvoked = false;
		}

		protected override bool canAttack()
		{
			return isProvoked;
		}
		protected override void attackInstantiate(Transform attackTransform)
		{
			GameObject currentAttack = QolUtility.Instantiate(data.attackPrefab,
				attackTransform.position, attackTransform.rotation, transform);

			ProjectileBehavior currentAttackData = currentAttack.GetComponent<ProjectileBehavior>();

			currentAttackData.initialize(controller, data);
			currentAttackData.setStrength(data.strength);
			currentAttackData.setHitboxSize(attackTransform.lossyScale);
		}
	}
}