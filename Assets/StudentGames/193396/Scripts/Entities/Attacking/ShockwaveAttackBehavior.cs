using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace _193396
{
	public class ShockwaveAttackBehavior : BaseAttackBehavior
	{
		public ShockwaveAttackBehaviorData data;
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
		
		private IEnumerator waveSpawn(ProjectileBehavior projectile, Transform attackTransform)
		{
			while (true)
			{
				yield return new WaitForSeconds(1f / data.spawnFrequency);

				if (attackTransform.IsDestroyed())
					yield break;

				GameObject currentAttack = QolUtility.Instantiate(data.wavePrefab,
					projectile.transform.position, projectile.transform.rotation,
					GameManager.instance.runtimeGroup[GameManager.RuntimeGroup.Projectiles]);

				ProjectileBehavior currentAttackData = currentAttack.GetComponent<ProjectileBehavior>();

				currentAttackData.initialize(controller, data);
				currentAttackData.setStrength(data.strength);
				currentAttackData.setHitboxSize(attackTransform.lossyScale);
			}
		}
		protected override ProjectileBehavior attackInstantiate(Transform attackTransform)
		{
			foreach (Vector2 direction in data.directions)
			{
				GameObject currentAttack = QolUtility.Instantiate(data.attackPrefab,
					attackTransform.position, attackTransform.rotation, 
					GameManager.instance.runtimeGroup[GameManager.RuntimeGroup.Projectiles]);

				ProjectileBehavior currentAttackData = currentAttack.GetComponent<ProjectileBehavior>();

				currentAttackData.initialize(controller, data);
				currentAttackData.setStrength(data.strength);

				currentAttackData.controller.rigidBody.velocity = direction.normalized * data.speed;

				currentAttackData.StartCoroutine(waveSpawn(currentAttackData, attackTransform));

				Destroy(currentAttack, data.maxLifetime);
			}

			return null;
		}
	}
}