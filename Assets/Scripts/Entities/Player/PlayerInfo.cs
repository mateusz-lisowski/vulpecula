using UnityEngine;

public class PlayerInfo : EntityBehavior
{
	[field: Space(10)]
	[field: SerializeField, ReadOnly] public int score { get; private set; }


	private PlayerMovement movement;
	private PlayerData data;


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

	
	private void collect(CollectData collect)
	{
		score++;
	}

}
