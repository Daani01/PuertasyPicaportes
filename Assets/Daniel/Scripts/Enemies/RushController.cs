using UnityEngine;
using System.Collections.Generic;

public class RushController : MonoBehaviour
{
    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    public float speed = 5f;
    public float damageAmount = 20f;

    public void SetWaypoints(List<Transform> waypointsToFollow)
    {
        waypoints = waypointsToFollow;
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FirstPersonController playerHealth = other.GetComponent<FirstPersonController>();
            if (playerHealth != null)
            {
                if(playerHealth.currentState == FirstPersonController.PlayerState.Hiding)
                {
                    Debug.Log("ESCONDIDO.");
                    return;
                }
                playerHealth.KillInstantly();
            }
        }
    }
}
