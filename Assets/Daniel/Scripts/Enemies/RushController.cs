using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RushController : Enemie
{
    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    public float speed = 5f;
    public float damageAmount = 20f;
    private bool isMoving = false;

    private bool isInitialized = false;

    private void Awake()
    {
        enemyName = "Rush";
        dieInfo = "Has muerto por Rush\n\nPrueba a esconderte antes de que te alcance";

    }

    private void Start()
    {
        isInitialized = true;
    }

    private void OnEnable()
    {
        if (!isInitialized)
        {
            Start();
        }
    }

    public void SetWaypoints(List<Transform> waypointsToFollow)
    {
        waypoints = waypointsToFollow;
        currentWaypointIndex = 0;
        if (waypoints != null && waypoints.Count > 0 && !isMoving)
        {
            StartCoroutine(MoveThroughWaypoints());
        }
    }

    private IEnumerator MoveThroughWaypoints()
    {
        isMoving = true;
        while (currentWaypointIndex < waypoints.Count)
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);
                yield return null;
            }

            currentWaypointIndex++;
        }

        isMoving = false;
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FirstPersonController playerHealth = other.GetComponent<FirstPersonController>();
            if (playerHealth != null)
            {
                if (playerHealth.currentState == FirstPersonController.PlayerState.Hiding)
                {
                    Debug.Log("ESCONDIDO.");
                    return;
                }
                playerHealth.KillInstantly(gameObject.GetComponent<Enemie>());
            }
        }
    }
}
