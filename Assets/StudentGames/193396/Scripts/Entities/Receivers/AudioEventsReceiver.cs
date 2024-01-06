using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace _193396
{
	using Type = AudioEventReceiver.Type;

	[RequireComponent(typeof(AudioSource))]
	public class AudioEventsReceiver : EntityEventReceiver
	{
		[Serializable]
		public class EventTransformer
		{
			public string eventName;
			public string eventData;
			public Type type = Type.Play;
			[Space(5)]
			public float transitionStartTime = 0f;
			public float transitionTime = 0f;
		}
		public EventTransformer[] eventTransformers;

		private AudioSource source;

		private float volume;
		private Coroutine startingCoroutine;
		private Coroutine stoppingCoroutine;


		private void Awake()
		{
			source = transform.GetComponent<AudioSource>();

			volume = source.volume;
		}

		public override string[] capturableEvents => eventTransformers.Select(e => e.eventName).ToArray();
		public override void onEvent(string eventName, object eventData)
		{
			foreach (var eventTransformer in eventTransformers)
				if (InputtableEvent.matches(
					eventTransformer.eventName, eventTransformer.eventData, eventName, eventData))
				{
					switch (eventTransformer.type)
					{
						case Type.PlayContinue:
						case Type.Play:
							if (eventTransformer.type == Type.PlayContinue && source.isPlaying
								&& stoppingCoroutine == null)
								break;
							if (stoppingCoroutine != null)
							{
								StopCoroutine(stoppingCoroutine);
								stoppingCoroutine = null;
							}
							if (startingCoroutine != null)
								break;

							startingCoroutine = StartCoroutine(transition(eventTransformer.transitionStartTime,
								eventTransformer.transitionTime, eventTransformer.type));
							break;
						case Type.Stop:
							if (startingCoroutine != null)
							{
								StopCoroutine(startingCoroutine);
								startingCoroutine = null;
							}
							if (stoppingCoroutine != null)
							{
								StopCoroutine(stoppingCoroutine);
								stoppingCoroutine = null;
							}

							stoppingCoroutine = StartCoroutine(transition(eventTransformer.transitionStartTime,
								eventTransformer.transitionTime, eventTransformer.type));
							break;
					}
				}
		}


		private IEnumerator transition(float transitionStartTime, float transitionTime, Type type)
		{
			bool play = type != Type.Stop;

			if (transitionStartTime != 0f)
				yield return new WaitForSeconds(transitionStartTime);

			float volumeDelta = volume / transitionTime;
			if (!play)
				volumeDelta = -volumeDelta;

			if (play && (type == Type.Play || !source.isPlaying))
				source.Play();

			if (transitionTime != 0f)
				while (source.volume != (play ? volume : 0f))
				{
					source.volume = Mathf.Clamp(source.volume + volumeDelta * Time.deltaTime, 0f, volume);

					yield return null;
				}

			source.volume = volume;

			if (!play)
				source.Stop();

			if (play)
				startingCoroutine = null;
			else
				stoppingCoroutine = null;
		}
	}
}