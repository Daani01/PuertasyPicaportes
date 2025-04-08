using System.Collections;
using UnityEngine;

public class ScreechController : Enemie, IInteractable
{
    public float timeToAppear;
    public float maxTimeNotLookedAt;
    public GameObject screechObj;
    public Transform detectionSphere;
    public float minRadius;
    public float maxRadius;

    private Transform playerTransform;
    private Camera playerCamera;
    private Vector3 randomPosition;
    private Vector3 relativePosition;
    private GameObject player;
    private bool screechKilled;
    private float timeNotLookedAt;
      
    private bool isInitialized = false;
    private AudioSource audioSource;
    private string soundName;

    void Start()
    {
        isInitialized = true;
    }

    void OnEnable()
    {
        if (!isInitialized)
        {
            Start();
        }

        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerCamera = playerTransform.GetComponentInChildren<Camera>();

            if (playerCamera == null)
            {
                Debug.LogError("Player camera not found as a child of the Player object.");
            }
        }
        else
        {
            Debug.LogError("Player object not found in the scene.");
        }

        if (playerTransform != null && playerTransform.GetComponent<FirstPersonController>() != null)
        {
            StartCoroutine(WaitAndActivate());
        }
        else
        {
            Debug.LogWarning("Player or FirstPersonController is not set. Skipping activation.");
        }
    }

    private Vector3 GetRandomPositionInSphere(Vector3 center)
    {
        Vector3 direction = Random.insideUnitSphere.normalized;
        float distance = Random.Range(minRadius, maxRadius);
        Vector3 randomPoint = center + direction * distance;

        randomPoint.y = Mathf.Min(randomPoint.y, 1f);

        return randomPoint;
    }


    private IEnumerator WaitAndActivate()
    {
        soundName = "ScreechSound";
        audioSource = SoundPoolManager.Instance.PlaySound(soundName, gameObject);

        randomPosition = GetRandomPositionInSphere(playerTransform.position);
        relativePosition = randomPosition - playerTransform.position;

        yield return new WaitForSeconds(timeToAppear);

        screechObj.SetActive(true);
        StartCoroutine(CheckPlayerLooking());
    }

    private IEnumerator CheckPlayerLooking()
    {
        while (true)
        {
            if(playerTransform != null)
            {
                transform.position = playerTransform.position + relativePosition;
            }

            if (playerCamera != null)
            {
                transform.LookAt(playerCamera.transform);
            }

            if (screechKilled)
            {
                DeactivateObject();
                yield break;
            }
            else
            {
                timeNotLookedAt += Time.deltaTime;

                if (timeNotLookedAt >= maxTimeNotLookedAt)
                {
                    ApplyDamageToPlayer();
                    DeactivateObject();
                    yield break;
                }
            }

            yield return null;
        }
    }


    public void InteractObj()
    {
        screechKilled = true;
    }    

    private void ApplyDamageToPlayer()
    {
        FirstPersonController player = null;

        if (playerTransform != null) 
        {
            player = playerTransform.GetComponent<FirstPersonController>();
        }

        if (player != null && player.currentHealth > 0)
        {
            player.TakeDamage(damage, gameObject.GetComponent<Enemie>());
            EnemyPool.Instance.ReturnEnemy(gameObject);
        }
    }

    private void DeactivateObject()
    {
        timeNotLookedAt = 0.0f;
        screechObj.SetActive(false);
        screechKilled = false;
        SoundPoolManager.Instance.ReturnToPool(soundName, audioSource);
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (detectionSphere == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionSphere.position, minRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionSphere.position, maxRadius);
    }
}