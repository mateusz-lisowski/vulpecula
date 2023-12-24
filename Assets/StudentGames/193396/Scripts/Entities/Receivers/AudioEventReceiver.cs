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

		private AudioSource source;
		private float eventCooldown = 0f;


		private void Awake()
		{
			source = transform.GetComponent<AudioSource>();
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
					source.Play();
					break;
				case Type.Stop:
					source.Stop();
					break;
			}

			eventCooldown = cooldown;
		}
	}
}