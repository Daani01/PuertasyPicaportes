using UnityEngine;
using System.Collections.Generic;

public class RushController : MonoBehaviour
{
    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    public float speed = 5f;

    public void SetWaypoints(List<Transform> waypointsToFollow)
    {
        waypoints = waypointsToFollow;
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        // Mover hacia el siguiente punto
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        // Comprobar si ha alcanzado el punto actual y pasar al siguiente
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                // Cuando llega al último punto, destruir o desactivar el enemigo, o reiniciar
                Destroy(gameObject);
            }
        }
    }
}
