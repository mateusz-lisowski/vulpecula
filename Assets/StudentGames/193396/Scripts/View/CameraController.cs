using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _193396
{
	public class CameraController : MonoBehaviour
	{
		public RuntimeSettings.LayerMaskInput hiddenLayers;

		public Transform target;
		public PolygonCollider2D boundary;

		public float xMargin = 1f;
		public float yMargin = 1f;

		public float smoothTime = 0.25f;
		private Vector3 velocity = Vector3.zero;


		private List<Transform> targetStack = new List<Transform>();
		private void setTarget(Transform newTarget)
		{
			if (target != null)
				target.gameObject.SendMessageUpwards("onMessage", new EntityMessage("unfocused", null), 
					SendMessageOptions.DontRequireReceiver);

			target = newTarget;
			target.gameObject.SendMessageUpwards("onMessage", new EntityMessage("focused", null), 
				SendMessageOptions.DontRequireReceiver);
		}
		public void pushTarget(Transform newTarget)
		{
			targetStack.Add(target);
			setTarget(newTarget);
		}
		public void popTarget(Transform oldTarget)
		{
			while (target == oldTarget)
			{
				setTarget(targetStack.Last());
				targetStack.RemoveAt(targetStack.Count - 1);
			}

			targetStack.RemoveAll(t => t == oldTarget);
		}


		private void OnValidate()
		{
			Camera camera = transform.GetComponent<Camera>();

			camera.cullingMask = ~hiddenLayers;
		}

		private void Awake()
		{
			var targetTmp = target;
			target = null;
			setTarget(targetTmp);
		}

		private void Update()
		{
			trackPlayer();
			if (boundary != null)
				keepWithinBoundary();
		}

		private Vector3 marginedTarget()
		{
			Vector3 boundedPosition = transform.position;
			boundedPosition.z = transform.position.z;

			if (Mathf.Abs(transform.position.x - target.position.x) > xMargin)
			{
				if (transform.position.x > target.position.x)
					boundedPosition.x = target.position.x - xMargin;
				else
					boundedPosition.x = target.position.x + xMargin;
			}

			if (Mathf.Abs(transform.position.y - target.position.y) > yMargin)
			{
				if (transform.position.y > target.position.y)
					boundedPosition.y = target.position.y - yMargin;
				else
					boundedPosition.y = target.position.y + yMargin;
			}

			return boundedPosition;
		}
		private void trackPlayer()
		{
			Vector3 targetPosition = marginedTarget();
			transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
		}

		private static Bounds orthographicBounds(Camera camera)
		{
			float screenAspect = (float)Screen.width / (float)Screen.height;
			float cameraHeight = camera.orthographicSize * 2;
			Bounds bounds = new Bounds(
				camera.transform.position,
				new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
			return bounds;
		}
		private bool keepPointWithinBoundary(Vector2 point)
		{
			Vector2 closest = boundary.ClosestPoint(point);

			if (closest == point)
				return true;

			if (Mathf.Abs(closest.x - point.x) < Mathf.Abs(closest.y - point.y))
				closest.x = point.x;
			else
				closest.y = point.y;

			transform.position = new Vector3(
				transform.position.x - (point.x - closest.x),
				transform.position.y - (point.y - closest.y),
				transform.position.z);

			return false;
		}
		private void keepWithinBoundary()
		{
			for (int i = 0; i < 20; ++i)
			{
				Bounds bounds = orthographicBounds(Camera.main);

				if (!keepPointWithinBoundary(new Vector2(bounds.min.x, bounds.min.y)))
					continue;
				if (!keepPointWithinBoundary(new Vector2(bounds.max.x, bounds.min.y)))
					continue;
				if (!keepPointWithinBoundary(new Vector2(bounds.min.x, bounds.max.y)))
					continue;
				if (!keepPointWithinBoundary(new Vector2(bounds.max.x, bounds.max.y)))
					continue;

				return;
			}
		}
	}
}