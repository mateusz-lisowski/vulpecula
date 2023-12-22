using System;
using System.Linq;
using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(AudioSource))]
	public class AudioEventReceiver : EntityEventReceiver
	{
		public InputtableEvent eventPlay;

		private AudioSource source;


		private void Awake()
		{
			source = transform.GetComponent<AudioSource>();
		}

		public override string[] capturableEvents => new string[] { eventPlay.name };
		public override void onEvent(string eventName, object eventData)
		{
			if (eventPlay.matches(eventName, eventData))
				source.Play();
		}
	}
}