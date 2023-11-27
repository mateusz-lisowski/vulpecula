using UnityEngine;

public class SpawnOnEventBehavior : EntityBehavior
{
	public SpawnOnEventBehaviorData data;


	public override void onEvent(string eventName, object eventData)
	{
		if (eventName != data.eventName)
			return;

		foreach (var toSpawn in data.objects)
		{
			Instantiate(toSpawn.prefab, transform.position, Quaternion.identity, 
				GameManager.instance.runtimeGroup[toSpawn.group]);
		}
	}
}
