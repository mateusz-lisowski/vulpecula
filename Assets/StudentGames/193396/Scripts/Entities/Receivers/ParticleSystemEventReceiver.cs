using System;
using System.Linq;
using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticleSystemEventReceiver : EntityEventReceiver
	{
		[Serializable]
		public class EventTransformer
		{
			public enum Type
			{
				Play, Stop
			}

			public string eventName;
			public string eventData;
			public Type type;
		}
		public EventTransformer[] eventTransformers;

		private ParticleSystem particles;


		private void Awake()
		{
			particles = transform.GetComponent<ParticleSystem>();
		}

		public override string[] capturableEvents => eventTransformers.Select(e => e.eventName).ToArray();
		public override void onEvent(string eventName, object eventData)
		{
			foreach (var eventTransformer in eventTransformers)
				if (InputtableEvent.matches(
					eventTransformer.eventName, eventTransformer.eventData, eventName, eventData))
					switch (eventTransformer.type)
					{
						case EventTransformer.Type.Play:
							particles.Play();
							break;
						case EventTransformer.Type.Stop:
							particles.Stop();
							break;
					}
		}
	}
}