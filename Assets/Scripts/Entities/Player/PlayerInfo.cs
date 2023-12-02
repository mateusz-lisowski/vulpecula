using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInfo : EntityBehavior
{
	[SerializeField]
	public class RuntimeData : RuntimeDataManager.Data
	{
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public int score = 0;
	}

	private PlayerMovement movement;
	private PlayerData data;
	private RuntimeData runtimeData;

	private List<int> justCollected = new List<int>();


	public override void onAwake()
	{
		movement = controller.getBehavior<PlayerMovement>();
		data = movement.data;
	}
	public override void onStart()
	{
		runtimeData = RuntimeDataManager.get<RuntimeData>("Player");
	}

	public override void onEvent(string eventName, object eventData)
	{
		switch (eventName)
		{
			case "collect": collect(eventData as CollectData); break;
		}
	}

	public override void onUpdate()
	{
		justCollected.Clear();
	}


	private void collect(CollectData collect)
	{
		if (justCollected.Contains(collect.id))
			return;

		justCollected.Add(collect.id);
		runtimeData.score++;

		Debug.Log("Collected " + runtimeData.score + " collectible at: " + Time.timeSinceLevelLoad);
	}

}
