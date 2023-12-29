using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class BossFightController : EntityBehavior
    {
		private HurtBehavior bossHurt;
		private RunBehavior bossRun;
		private JumpAtBehavior bossJumpAt;
		private MeleeAtackBehavior bossRunAttack;
		private MeleeAtackBehavior bossSmashAttack;

		private Transform baseLocation;
		private Transform smashLocation;
		private Transform[] enterLocations;

		private Transform jumpAtTarget;


		public override void onAwake()
		{
			Transform boss = transform.Find("boss");
			EntityBehaviorController bossController = boss.GetComponent<EntityBehaviorController>();

			bossHurt = bossController.getBehavior<HurtBehavior>();
			bossRun = bossController.getBehavior<RunBehavior>();
			bossJumpAt = bossController.getBehavior<JumpAtBehavior>();

			List<MeleeAtackBehavior> bossAttacks = bossController.getBehaviors<MeleeAtackBehavior>();
			bossRunAttack = bossAttacks[0];
			bossSmashAttack = bossAttacks[1];

			Transform bossRoom = transform.Find("Room");
			baseLocation = (new GameObject("Base")).transform;
			baseLocation.position = boss.position;
			baseLocation.rotation = boss.rotation;
			baseLocation.parent = bossRoom;

			smashLocation = transform.Find("Room/Center");

			enterLocations = transform.Find("Room/Landing").GetComponentsInChildren<Transform>();
		}

		public override string[] capturableEvents => new string[] { 
			"jumpAt", "jumpBegin", "spawn", "enable", "disable" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "jumpAt":
					jumpAt((string)eventData);
					break;
				case "jumpBegin":
					jumpBegin();
					break;
				case "spawn":
					StartCoroutine(spawnAndWait());
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
				case "smash":
					bossSmashAttack.enabled = enable;
					break;
				default:
					Debug.LogWarning("Unknown boss component: " + name);
					break;
			}
		}

		private void jumpAt(string name)
		{
			Transform target;

			switch(name)
			{
				case "begin":
					target = enterLocations[0];
					break;
				case "reenter":
					target = enterLocations[Random.Range(0, enterLocations.Length)];
					break;
				case "base":
					target = baseLocation;
					break;
				case "smash":
					target = smashLocation;
					break;
				default:
					Debug.LogWarning("Unknown boss jump target: " + name);
					return;
			}

			jumpAtTarget = target;
		}
		private void jumpBegin()
		{
			bossJumpAt.target = jumpAtTarget;
			jumpAtTarget = null;
		}

		private IEnumerator spawnAndWait()
		{
			yield return new WaitForSeconds(5);

			controller.onEvent("roomCleared", null);
		}
	}
}