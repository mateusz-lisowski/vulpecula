using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : EntityBehavior
{
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public int score { get; private set; }


	private PlayerMovement movement;
	private PlayerData data;

	private List<int> justCollected = new List<int>();


	public override void onAwake()
	{
		movement = controller.getBehavior<PlayerMovement>();
		data = movement.data;
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
		score++;
	}

}
