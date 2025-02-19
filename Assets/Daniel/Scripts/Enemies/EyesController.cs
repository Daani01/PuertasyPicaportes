using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static FirstPersonController;

public class EyesController : Enemie
{
    public float damageAmount;
    public float checkInterval;
    public float detectionRadius;
    public float visionRadius;
    public Transform detectionSphere;
    public Transform visionSphere;
    private Transform playerTransform;

    private Camera playerCamera;
    private float nextCheckTime = 0f;
    private float timeElapsed = 0f;
    private AudioSource audioSource;
    private string soundName;

    private void OnEnable()
    {
        string enemyInfo = CSVManager.Instance.GetSpecificData(enemyName, ExcelValues.Prob.ToString());
        //Debug.Log($"Info de {enemyName}: {enemyInfo}");

        timeElapsed = 0.0f;
    }

    void Awake()
    {
        enemyName = "Eyes";
        dieInfo = "Has muerto por Eyes\n\nPrueba a no mirarlo por mucho tiempo";
        //damageAmount = 40.0f;

        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
                playerCamera = playerObject.GetComponentInChildren<Camera>();

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
    }

    void Update()
    {
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            CheckPlayerView();
        }

        timeElapsed += Time.deltaTime;

        if (timeElapsed >= 60f)
        {
            EnemyPool.Instance.ReturnEnemy(gameObject);

        }

        if(playerCamera != null)
        {
            transform.LookAt(playerCamera.transform);
        }
    }

    public void SetPosition(Transform spawnPosition)
    {
        if(spawnPosition != null)
        {
            soundName = "EyesSound";
            audioSource = SoundPoolManager.Instance.PlaySound(soundName, gameObject);

            gameObject.transform.position = spawnPosition.position;
        }
    }

    private void CheckPlayerView()
    {
        if (playerTransform == null || detectionSphere == null || visionSphere == null || playerCamera == null) return;

        if (playerTransform.GetComponent<FirstPersonController>().currentState != PlayerState.Hiding) 
        {
            Vector3 directionToDetectionSphere = detectionSphere.position - playerTransform.position;
            if (directionToDetectionSphere.magnitude <= detectionRadius)
            {
                Vector3 directionToVisionSphere = (visionSphere.position - playerCamera.transform.position).normalized;
                float dotProduct = Vector3.Dot(playerCamera.transform.forward, directionToVisionSphere);

                if (dotProduct > 0.9f && directionToVisionSphere.magnitude <= visionRadius) // Ajusta 0.9f para aumentar o disminuir el rango
                {
                    FirstPersonController player = playerTransform.GetComponent<FirstPersonController>();
                    if (player != null && player.currentHealth > 0)
                    {
                        player.TakeDamage(damageAmount, gameObject.GetComponent<Enemie>());
                        //Debug.Log($"Player damaged by {damageAmount}. Current health: {player.currentHealth}");

                        //EnemyPool.Instance.ReturnEnemy(gameObject);
                    }
                }
            }
        }        
    }

    // Dibujar Gizmos en el Editor
    private void OnDrawGizmos()
    {
        if (detectionSphere == null || visionSphere == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionSphere.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(visionSphere.position, visionRadius);
    }
}
