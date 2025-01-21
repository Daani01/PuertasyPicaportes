using System.Collections;
using UnityEngine;

public class ScreechController : MonoBehaviour, IInteractable
{
    private Transform playerTransform;
    private Camera playerCamera;
    private Vector3 randomPosition;
    private Vector3 relativePosition;
    private GameObject player;

    private bool killScreech;

    private float timeNotLookedAt;
    public float maxTimeNotLookedAt;
    public float lookThreshold;

    public float timeToAppear;
    public float damageAmount;

    public GameObject screechObj;
    public Transform detectionSphere;
    public Transform visionSphere;
    public float detectionRadius;
    public float visionRadius;

    void Start()
    {
        player = GameObject.Find("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerCamera = player.GetComponentInChildren<Camera>();            

            if (playerCamera == null)
            {
                Debug.LogError("Player camera not found as a child of the Player object.");
            }
        }
        else
        {
            Debug.LogError("Player object not found in the scene.");
        }

    }

    void OnEnable()
    {
        if (playerTransform != null && playerTransform.GetComponent<FirstPersonController>() != null)
        {
            StartCoroutine(WaitAndActivate());//PRIMERA VEZ NO SE ACTIVA
        }
        else
        {
            Debug.LogWarning("Player or FirstPersonController is not set. Skipping activation.");
        }
    }

    private Vector3 GetRandomPositionInSphere(Vector3 center, float radius)
    {
        Vector3 randomPoint = Random.insideUnitSphere * radius;
        return center + randomPoint;
    }

    private IEnumerator WaitAndActivate()
    {
        float radius = playerTransform.GetComponent<FirstPersonController>().screechRadius;
        randomPosition = GetRandomPositionInSphere(playerTransform.position, radius);
        relativePosition = randomPosition - playerTransform.position;

        yield return new WaitForSeconds(timeToAppear);

        screechObj.SetActive(true);
        StartCoroutine(CheckPlayerLooking());
    }

    private IEnumerator CheckPlayerLooking()
    {
        while (true)
        {
            if (playerCamera != null)
            {
                transform.position = playerTransform.position + relativePosition;

                if (killScreech)
                {
                    Debug.Log("Screech visto");
                    DeactivateObject();
                    yield break;
                }
                else
                {
                    timeNotLookedAt += Time.deltaTime;
                    Debug.Log($"Time not looked at: {timeNotLookedAt}");

                    if (timeNotLookedAt >= maxTimeNotLookedAt)
                    {
                        ApplyDamageToPlayer();
                        DeactivateObject();
                        yield break;
                    }
                }
            }

            yield return null;
        }
    }


    public void InteractObj()
    {
        killScreech = true;
    }

    private bool CheckPlayerView()
    {
        if (playerTransform == null || detectionSphere == null || visionSphere == null || playerCamera == null)
            return false;

        // Verificar si está mirando la visión frontal
        Vector3 directionToVisionSphere = (visionSphere.position - playerCamera.transform.position).normalized;
        float dotProduct = Vector3.Dot(playerCamera.transform.forward, directionToVisionSphere);

        // Validar el producto punto y el radio de visión frontal
        if (dotProduct > 0.9)
        {
            float distanceToVisionSphere = Vector3.Distance(playerCamera.transform.position, visionSphere.position);
            if (distanceToVisionSphere <= visionRadius)
            {
                Debug.Log("Player is within vision range and looking at the object.");
                return true;
            }
        }

        return false;
    }

    private void ApplyDamageToPlayer()
    {
        FirstPersonController player = playerTransform.GetComponent<FirstPersonController>();
        if (player != null && player.currentHealth > 0)
        {
            player.TakeDamage(damageAmount);
            Debug.Log($"Player damaged by {damageAmount}. Current health: {player.currentHealth}");
        }
    }

    private void DeactivateObject()
    {
        timeNotLookedAt = 0.0f;
        screechObj.SetActive(false);
        killScreech = false;
        gameObject.SetActive(false);
        Debug.Log("Screech object deactivated.");
    }

    private void OnDrawGizmos()
    {
        if (detectionSphere == null || visionSphere == null) return;

        // Dibuja la esfera de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionSphere.position, detectionRadius);

        // Dibuja la esfera de visión
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(visionSphere.position, visionRadius);

        // Dibuja la dirección de la cámara
        if (playerCamera != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(playerCamera.transform.position, playerCamera.transform.position + playerCamera.transform.forward * 10f);
        }
    }
}