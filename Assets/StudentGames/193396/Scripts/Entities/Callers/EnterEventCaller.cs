using UnityEngine;

namespace _193396
{
	public class EnterEventCaller : MonoBehaviour
	{
		public string eventName;
		public string eventData;
		[Space(5)]
		[Tooltip("Layers which receive the event on enter")]
		public RuntimeSettings.LayerMaskInput enterLayers;


		private void OnTriggerEnter2D(Collider2D collision)
		{
			GameObject other = collision.gameObject;

			if (((1 << other.layer) & enterLayers) != 0)
				other.SendMessageUpwards("onMessage", new EntityMessage(eventName, eventData == "" ? null : eventData),
					SendMessageOptions.DontRequireReceiver);
		}
	}
}