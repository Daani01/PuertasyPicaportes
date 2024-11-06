using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesController : MonoBehaviour
{
    public float damageAmount = 10f;
    public float checkInterval = 1f;
    public Transform playerTransform;
    public Transform detectionSphere;      // Esfera de rango general
    public Transform visionSphere;         // Esfera de visión más pequeña
    public float detectionRadius = 5f;     // Radio de la esfera de rango general
    public float visionRadius = 1f;        // Radio de la esfera de visión
    public float horizontalViewAngle = 20f; // Ángulo de visión horizontal en grados
    private float nextCheckTime = 0f;

    void Start()
    {
        // Buscar al jugador si no se ha asignado manualmente
        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.Find("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
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
    }

    private void CheckPlayerView()
    {
        if (playerTransform == null || detectionSphere == null || visionSphere == null) return;

        // Calcular la dirección desde el jugador hacia el centro de la esfera de rango general
        Vector3 directionToDetectionSphere = detectionSphere.position - playerTransform.position;

        // Verificar si el jugador está dentro del rango de la esfera grande
        if (directionToDetectionSphere.magnitude <= detectionRadius)
        {
            // Calcular la dirección hacia la esfera de visión
            Vector3 directionToVisionSphere = visionSphere.position - playerTransform.position;
            directionToVisionSphere.y = 0; // Ignorar la altura para que el ángulo sea horizontal

            // Calcular el ángulo horizontal entre la cámara del jugador y la esfera de visión
            float horizontalAngle = Vector3.Angle(playerTransform.forward, directionToVisionSphere);

            // Verificar si el jugador está dentro del ángulo de visión horizontal y dentro del radio de la esfera de visión
            if (horizontalAngle <= horizontalViewAngle && directionToVisionSphere.magnitude <= visionRadius)
            {
                // Aplicar daño al jugador
                FirstPersonController player = playerTransform.GetComponent<FirstPersonController>();
                if (player != null && player.currentHealth > 0)
                {
                    player.currentHealth -= damageAmount;
                    Debug.Log($"Player damaged by {damageAmount}. Current health: {player.currentHealth}");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (detectionSphere == null || visionSphere == null) return;

        // Dibujar la esfera de rango general
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionSphere.position, detectionRadius);

        // Dibujar la esfera de visión
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(visionSphere.position, visionRadius);

        // Dibujar el ángulo de visión horizontal
        Vector3 leftBoundary = Quaternion.Euler(0, -horizontalViewAngle, 0) * visionSphere.forward * visionRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, horizontalViewAngle, 0) * visionSphere.forward * visionRadius;
        Gizmos.DrawRay(visionSphere.position, leftBoundary);
        Gizmos.DrawRay(visionSphere.position, rightBoundary);
    }
}
