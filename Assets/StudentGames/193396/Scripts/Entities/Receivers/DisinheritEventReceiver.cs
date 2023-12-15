using UnityEngine;

namespace _193396
{
	public class DisinheritEventReceiver : EntityEventReceiver
	{
		[Tooltip("Time after which the object is destroyed")]
		public float leftTime = 3f;


		public override string[] capturableEvents => new string[] { "destroy" };
		public override void onEvent(string eventName, object eventData)
		{
			transform.parent = GameManager.instance.runtimeGroup[GameManager.RuntimeGroup.Disinherited];
			Destroy(gameObject, leftTime);
		}
	}
}