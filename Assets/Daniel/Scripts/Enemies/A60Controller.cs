using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A60Controller : Enemie
{
    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    private float speed;
    private bool isMoving = false;

    private bool isInitialized = false;


    private void Awake()
    {
        //damage = float.Parse(CSVManager.Instance.GetSpecificData(enemyName, ExcelValues.Damage.ToString()));
        //dieInfo = CSVManager.Instance.GetSpecificData(enemyName, ExcelValues.DieInfo.ToString());
        //speed = float.Parse(CSVManager.Instance.GetSpecificData(enemyName, ExcelValues.Speed.ToString()));
        //repetitions = int.Parse(CSVManager.Instance.GetSpecificData(enemyName, "Repetitions"));

        damage = 125;
        dieInfo = "ESTE NO AVISA!";
        speed = 25;

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
