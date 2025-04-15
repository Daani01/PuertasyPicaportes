using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemie;

public class AmbushController : Enemie
{
    private List<Transform> waypoints;
    private int repetitions;
    private int currentRepetition = 0;
    private bool reverse = false;

    private int currentWaypointIndex = 0;
    private bool isMoving = false;

    private bool isInitialized = false;

    private void Awake()
    {
        repetitions = 5;
        repetitions = Random.Range(2, repetitions + 1);
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
        currentRepetition = 0;
        reverse = false;
        isMoving = false;

        if (waypoints != null && waypoints.Count > 0 && !isMoving)
        {
            StartCoroutine(MoveThroughWaypoints());
        }
    }

    private IEnumerator MoveThroughWaypoints()
    {
        isMoving = true;

        while (currentRepetition < repetitions)
        {
            while ((reverse && currentWaypointIndex >= 0) || (!reverse && currentWaypointIndex < waypoints.Count))
            {
                Transform targetWaypoint = waypoints[currentWaypointIndex];

                while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);
                    yield return null;
                }

                if (reverse)
                    currentWaypointIndex--;
                else
                    currentWaypointIndex++;
            }

            reverse = !reverse;

            currentWaypointIndex = reverse ? waypoints.Count - 1 : 0;

            currentRepetition++;
        }
        isMoving = false;
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FirstPersonController player = other.GetComponent<FirstPersonController>();
            if (player != null)
            {
                if (player.currentState == FirstPersonController.PlayerState.Hiding || player.currentState == FirstPersonController.PlayerState.Dead)
                {
                    return;
                }

                if (player.currentState != FirstPersonController.PlayerState.Dead && player.currentState != FirstPersonController.PlayerState.Hiding)
                {
                    player.TakeDamage(damage, gameObject.GetComponent<Enemie>());
                }
            }
        }
    }

}
