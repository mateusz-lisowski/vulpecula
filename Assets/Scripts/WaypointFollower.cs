using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    public Transform[] waypoints;     // An array to store the path waypoints.
    public float speed = 5.0f;                          // Speed at which the platform moves.
    private int currentWaypoint = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (waypoints.Length == 0)
        {
            return; // No waypoints, do nothing.
        }

        // Calculate the direction to the current waypoint.
        Vector3 direction = waypoints[currentWaypoint].position - transform.position;
        direction.Normalize();

        // Move the platform in the calculated direction.
        transform.Translate(direction * speed * Time.deltaTime);

        // Check if the platform has reached the current waypoint.
        if (Vector2.Distance(transform.position, waypoints[currentWaypoint].position) < 0.1f)
        {
            currentWaypoint++;

            // If we've reached the end of the path, loop back to the start.
            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
            }
        }
    }
}
