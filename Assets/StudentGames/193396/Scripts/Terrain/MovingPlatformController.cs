using System;
using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class MovingPlatformController : MonoBehaviour
	{
		public TerrainData data;
		public int startingWaypointIndex = 0;

		private Collider2D stickyCheck;

		private PathData path;
		private List<Transform> waypoints;

		private int currentWaypointTargetIndex;

		private void Awake()
		{
			stickyCheck = transform.Find("Detection/Sticky").GetComponent<Collider2D>();

			path = transform.parent.GetComponent<PathData>();

			waypoints = new List<Transform>();
			foreach (Transform child in transform.parent)
				if (child.name == "waypoint")
					waypoints.Add(child);

			if (waypoints.Count < 2)
				throw new ApplicationException("Path must have at least 2 waypoints.");

			currentWaypointTargetIndex = startingWaypointIndex;
		}

		void FixedUpdate()
		{
			if (reachedTarget())
			{
				if (currentWaypointTargetIndex + 1 == waypoints.Count)
					advanceTarget();
				else
					currentWaypointTargetIndex++;
			}

			Transform target = waypoints[currentWaypointTargetIndex];

			Vector2 newPosition = Vector2.MoveTowards(
				transform.position, target.position, path.speed * Time.deltaTime);

			Vector2 delta = (Vector2)transform.position - newPosition;
			slideDetected(delta);

			transform.position = newPosition;
		}


		private bool reachedTarget()
		{
			Transform target = waypoints[currentWaypointTargetIndex];

			return Vector2.Distance(transform.position, target.position) == 0f;
		}

		private void advanceTarget()
		{
			switch (path.type)
			{
				case PathData.Type.Cycle:
					currentWaypointTargetIndex = 0;
					break;
				case PathData.Type.Repeat:
					transform.position = waypoints[0].position;
					currentWaypointTargetIndex = 1;
					break;
				case PathData.Type.Bounce:
					waypoints.Reverse();
					currentWaypointTargetIndex = 1;
					break;
			}
		}


		private void slideDetected(Vector2 delta)
		{
			ContactFilter2D filter = new ContactFilter2D().NoFilter();
			filter.SetLayerMask(data.groundMoving.slidingLayers);
			filter.useLayerMask = true;

			List<Collider2D> contacts = new List<Collider2D>();
			if (stickyCheck.OverlapCollider(filter, contacts) == 0)
				return;

			foreach (Collider2D contact in contacts)
				contact.SendMessageUpwards("onMessage", new EntityMessage("slide", -delta),
					SendMessageOptions.DontRequireReceiver);
		}
	}
}