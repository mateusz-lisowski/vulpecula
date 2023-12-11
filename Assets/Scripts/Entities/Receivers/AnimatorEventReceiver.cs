using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorEventReceiver : EntityEventReceiver
{
	[Serializable]
	public class EventTransformer
	{
		public enum Type
		{
			Trigger, Boolean
		}

		public string eventName;
		public string eventData;
		public string animatorName;
		public Type type;

		[Space(5)]

		public bool setBoolean;
	}
	public EventTransformer[] eventTransformers;

    private Animator animator;


    private void Awake()
    {
        animator = transform.GetComponent<Animator>();
    }

	public override string[] capturableEvents => eventTransformers.Select(e => e.eventName).ToArray();
	public override void onEvent(string eventName, object eventData)
	{
		foreach (var eventTransformer in eventTransformers)
			if (eventTransformer.eventName == eventName && (eventTransformer.eventData == ""
				|| (eventData != null && eventTransformer.eventData == (eventData as string))))
				switch (eventTransformer.type)
				{
					case EventTransformer.Type.Trigger:
						animator.SetTrigger(eventTransformer.animatorName);
						break;
					case EventTransformer.Type.Boolean:
						animator.SetBool(eventTransformer.animatorName, eventTransformer.setBoolean);
						break;
				}
	}

}
