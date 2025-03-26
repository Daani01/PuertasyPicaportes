using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RushController : Enemie
{
    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    private float speed;
    private bool isMoving = false;

    private bool isInitialized = false;
    private AudioSource audioSource;
    private string soundName;

    private void Awake()
    {
        damage = float.Parse(CSVManager.Instance.GetSpecificData(enemyName, ExcelValues.Damage.ToString()));
        dieInfo = CSVManager.Instance.GetSpecificData(enemyName, ExcelValues.DieInfo.ToString());
        speed = float.Parse(CSVManager.Instance.GetSpecificData(enemyName, ExcelValues.Speed.ToString()));
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
        soundName = "RushSound";
        audioSource = SoundPoolManager.Instance.PlaySound(soundName, gameObject);
        audioSource.loop = true;

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

        // Detener y devolver el sonido a la pool
        SoundPoolManager.Instance.ReturnToPool(soundName, audioSource);

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
