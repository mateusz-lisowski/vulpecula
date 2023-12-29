using System.Collections;
using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(AudioSource))]
	public class AudioEventReceiver : EntityEventReceiver
	{
		public enum Type
		{
			Play, Stop, PlayContinue
		}

		public Type type = Type.Play;
		public string eventName;
		public string eventData;
		[Space(5)]
		public float cooldown = 0f;
		[Space(5)]
		public float transitionTime = 0f;

		private AudioSource source;
		private float eventCooldown = 0f;

		private float volume;


		private void Awake()
		{
			source = transform.GetComponent<AudioSource>();

			volume = source.volume;
		}
		private void Update()
		{
			eventCooldown -= Time.deltaTime;
		}

		public override string[] capturableEvents => new string[] { eventName };
		public override void onEvent(string eventName, object eventData)
		{
			if (eventCooldown > 0f)
				return;
			if (!InputtableEvent.matches(this.eventName, this.eventData, eventName, eventData))
				return;

			switch (type)
			{
				case Type.PlayContinue:
				case Type.Play:
					if (type == Type.PlayContinue && source.isPlaying)
						break;
					if (transitionTime == 0f)
						source.Play();
					else
						StartCoroutine(transition(play: true));
					break;
				case Type.Stop:
					if (transitionTime == 0f)
						source.Stop();
					else
						StartCoroutine(transition(play: false));
					break;
			}

			eventCooldown = cooldown;
		}

		
		private IEnumerator transition(bool play)
		{
			float timeLeft = transitionTime;

			if (play)
				source.Play();

			while (timeLeft > 0)
			{
				float vol = timeLeft / transitionTime;
				if (play)
					vol = 1f - vol;

				source.volume = volume * vol;

				yield return null;
				timeLeft -= Time.deltaTime;
			}

			source.volume = volume;

			if (!play)
				source.Stop();
		}
	}
}