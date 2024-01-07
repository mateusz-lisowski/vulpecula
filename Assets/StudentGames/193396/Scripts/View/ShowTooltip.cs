using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class ShowTooltip : MonoBehaviour
	{
		[Space(10)]
		public float maxAlpha = 1f;
		public float alphaChangeSpeed = 1f;
		[Space(5)]
		public RuntimeSettings.LayerMaskInput overlapLayers;

		private CanvasGroup group;

		private Collider2D innerCheck;
		private Collider2D outerCheck;


		private void Awake()
		{
			group = transform.Find("Body").GetComponent<CanvasGroup>();

			innerCheck = transform.Find("Detection/Inner").GetComponent<Collider2D>();
			outerCheck = transform.Find("Detection/Outer").GetComponent<Collider2D>();
		}

		private void Update()
		{
			float alpha = group.alpha;
			float targetAlpha = getAlphaScale() * maxAlpha;

			float maxChange = Time.deltaTime * alphaChangeSpeed;

			if (alpha > targetAlpha)
				alpha = Mathf.Max(alpha - maxChange, targetAlpha);
			else if (alpha < targetAlpha)
				alpha = Mathf.Min(alpha + maxChange, targetAlpha);

			group.alpha = alpha;
		}


		private float getAlphaScale()
		{
			bool isOuterOverlapping = outerCheck.IsTouchingLayers(overlapLayers);

			if (!isOuterOverlapping)
				return 0f;

			bool isInnerOverlapping = innerCheck.IsTouchingLayers(overlapLayers);

			if (isInnerOverlapping)
				return 1f;

			ContactFilter2D filter = new ContactFilter2D().NoFilter();
			filter.SetLayerMask(overlapLayers);
			filter.useLayerMask = true;

			List<Collider2D> contacts = new List<Collider2D>();
			if (outerCheck.OverlapCollider(filter, contacts) == 0)
				return 0f;

			float maxValue = 0f;

			foreach (var contact in contacts)
			{
				float innerDist = contact.Distance(innerCheck).distance;
				float outerDist = Mathf.Abs(contact.Distance(outerCheck).distance);

				float value = outerDist / (innerDist + outerDist);

				if (value > maxValue)
					maxValue = value;
			}

			return maxValue;
		}

	}
}