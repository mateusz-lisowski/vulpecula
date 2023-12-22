using System;
using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class MovingPlatformController : MonoBehaviour
	{
		public int startingWaypointIndex = 0;

		private PathData path;
		private List<Transform> waypoints;

		private int currentWaypointTargetIndex;

		private void Awake()
		{
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

	}
}