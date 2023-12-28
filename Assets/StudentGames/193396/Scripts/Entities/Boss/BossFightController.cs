using System.Collections;
using UnityEngine;

namespace _193396
{
	public class BossFightController : EntityBehavior
    {
		private Animator stateMachine;

		private HurtBehavior bossHurt;
		private RunBehavior bossRun;
		private JumpAtBehavior bossJumpAt;

		private Transform baseLocation;
		private Transform smashLocation;
		private Transform[] enterLocations;

		private Transform jumpAtTarget;


		public override void onAwake()
		{
			stateMachine = GetComponent<Animator>();

			Transform boss = transform.Find("boss");
			bossHurt = boss.GetComponent<HurtBehavior>();
			bossRun = boss.GetComponent<RunBehavior>();
			bossJumpAt = boss.GetComponent<JumpAtBehavior>();

			Transform bossRoom = transform.Find("Room");
			baseLocation = (new GameObject("Base")).transform;
			baseLocation.position = boss.position;
			baseLocation.rotation = boss.rotation;
			baseLocation.parent = bossRoom;

			smashLocation = transform.Find("Room/Center");

			enterLocations = transform.Find("Room/Landing").GetComponentsInChildren<Transform>();
		}

		public override string[] capturableEvents => new string[] { 
			"jumpAt", "jumpBegin", "spawnAndWait", 
			"enable", "disable", "setT", "clearT", "set", "clear", "getRandom" };
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
				case "spawnAndWait":
					StartCoroutine(spawnAndWait((string)eventData));
					break;
				case "enable":
					enableComponent((string)eventData, true);
					break;
				case "disable":
					enableComponent((string)eventData, false);
					break;
				case "setT":
					stateMachine.SetTrigger((string)eventData);
					break;
				case "clearT":
					stateMachine.ResetTrigger((string)eventData);
					break;
				case "set":
					stateMachine.SetBool((string)eventData, true);
					break;
				case "clear":
					stateMachine.SetBool((string)eventData, false);
					break;
				case "getRandom":
					stateMachine.SetInteger("rand", Random.Range(0, (int)eventData));
					break;
			}
		}


		private void enableComponent(string name, bool enable)
		{
			switch (name)
			{
				case "run":
					bossRun.enabled = enable;
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

		private IEnumerator spawnAndWait(string trigger)
		{
			yield return new WaitForSeconds(1);

			stateMachine.SetTrigger(trigger);
		}
	}
}