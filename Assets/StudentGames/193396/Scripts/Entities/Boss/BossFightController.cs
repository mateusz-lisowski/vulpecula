using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class BossFightController : EntityBehavior
    {
		public RuntimeSettings.LayerMaskInput enemyLayers;

		private Transform boss;
		private FlipBehavior bossDirection;
		private HurtBehavior bossHurt;
		private GroundedBehavior bossGround;
		private RunBehavior bossRun;
		private JumpAtBehavior bossJumpAt;
		private MeleeAtackBehavior bossRunAttack;

		private Collider2D findPlayerRegion;
		private Collider2D findEnemiesRegion;
		private Transform baseLocation;
		private Transform smashLocation;
		private Transform[] enterLocations;

		private Transform jumpAtTarget;

		private int nextSpawn = -1;


		public override void onAwake()
		{
			boss = transform.Find("boss");
			EntityBehaviorController bossController = boss.GetComponent<EntityBehaviorController>();

			bossDirection = bossController.getBehavior<FlipBehavior>();
			bossHurt = bossController.getBehavior<HurtBehavior>();
			bossGround = bossController.getBehavior<GroundedBehavior>();
			bossRun = bossController.getBehavior<RunBehavior>();
			bossJumpAt = bossController.getBehavior<JumpAtBehavior>();

			bossHurt.setDeathCondition(() => bossGround.isGrounded);

			List<MeleeAtackBehavior> bossAttacks = bossController.getBehaviors<MeleeAtackBehavior>();
			bossRunAttack = bossAttacks[0];

			Transform bossRoom = transform.Find("Room");

			baseLocation = (new GameObject("Base")).transform;
			lockTransform(ref baseLocation, boss.transform);
			baseLocation.parent = bossRoom;

			jumpAtTarget = (new GameObject("Target")).transform;
			lockTransform(ref jumpAtTarget, boss.transform);
			jumpAtTarget.parent = bossRoom;

			findPlayerRegion = transform.Find("Room/FindPlayer").GetComponent<Collider2D>();
			findEnemiesRegion = transform.Find("Room/FindEnemies").GetComponent<Collider2D>();
			smashLocation = transform.Find("Room/Center");
			enterLocations = transform.Find("Room/Landing").GetComponentsInChildren<Transform>();
		}

		public override string[] capturableEvents => new string[] {
			 "faceAt", "jumpAt", "teleportAt", "jumpBegin", "spawnRoom", 
			"clearRoom", "spawnEnd", "enable", "disable" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "faceAt":
					bossDirection.faceTowards(stringToTarget((string)eventData).position);
					break;
				case "jumpAt":
					lockTransform(ref jumpAtTarget, stringToTarget((string)eventData));
					break;
				case "teleportAt":
					boss.transform.position = stringToTarget((string)eventData).position;
					break;
				case "jumpBegin":
					jumpBegin();
					break;
				case "spawnRoom":
					StartCoroutine(spawnAndWait());
					break;
				case "clearRoom":
					clearRoom();
					break;
				case "spawnEnd":
					nextSpawn++;
					break;
				case "enable":
					enableComponent((string)eventData, true);
					break;
				case "disable":
					enableComponent((string)eventData, false);
					break;
			}
		}


		private void enableComponent(string name, bool enable)
		{
			switch (name)
			{
				case "run":
					bossRun.enabled = enable;
					bossRunAttack.enabled = enable;
					break;
				default:
					Debug.LogWarning("Unknown boss component: " + name);
					break;
			}
		}

		private void lockTransform(ref Transform toLock, Transform at)
		{
			toLock.position = at.position;
			toLock.rotation = at.rotation;
		}

		private Transform playerLocation()
		{
			ContactFilter2D filter = new ContactFilter2D().NoFilter();
			filter.SetLayerMask(bossRunAttack.data.provokeLayers);
			filter.useLayerMask = true;

			List<Collider2D> contacts = new List<Collider2D>();
			if (findPlayerRegion.OverlapCollider(filter, contacts) == 0)
				return smashLocation;

			return contacts[0].transform;
		}
		private Transform stringToTarget(string name)
		{
			switch (name)
			{
				case "begin":
					return enterLocations[0];
				case "reenter":
					return enterLocations[Random.Range(0, enterLocations.Length)];
				case "base":
					return baseLocation;
				case "smash":
					return smashLocation;
				case "player":
					return playerLocation();
				default:
					Debug.LogWarning("Unknown boss jump target: " + name);
					return null;
			}
		}
		private void jumpBegin()
		{
			bossJumpAt.target = jumpAtTarget;
		}

		private void clearRoom()
		{
			nextSpawn = -1;

			ContactFilter2D filter = new ContactFilter2D().NoFilter();
			filter.SetLayerMask(enemyLayers);
			filter.useLayerMask = true;

			List<Collider2D> contacts = new List<Collider2D>();
			if (findEnemiesRegion.OverlapCollider(filter, contacts) == 0)
				return;

			HitData hitData = new HitData();
			hitData.isVertical = true;
			hitData.right = Vector2.up;
			hitData.strength = 9999;

			foreach (var contact in contacts)
				if (contact.attachedRigidbody.transform != boss)
					contact.attachedRigidbody.SendMessage("onMessage", new EntityMessage("hit", hitData));
		}
		private IEnumerator spawnAndWait()
		{
			for (int i = 0; i < 3; i++)
			{
				nextSpawn = i;

				controller.onEvent("spawnBegin", i);
				while (nextSpawn == i || findEnemiesRegion.IsTouchingLayers(enemyLayers))
					yield return null;

				if (nextSpawn == -1)
					break;
			}
			
			controller.onEvent("roomCleared", null);
		}
	}
}