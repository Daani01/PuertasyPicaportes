using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesController : Enemie
{
    public TextAsset csvFile; // Asigna el archivo CSV en el Inspector
    private List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();


    public float damageAmount;                   
    public float checkInterval;
    public float detectionRadius;
    public float visionRadius;
    public Transform detectionSphere;
    public Transform visionSphere;
    private Transform playerTransform;

    private Camera playerCamera;                       
    private float nextCheckTime = 0f;                  

    void Start()
    {
        if (csvFile == null)
        {
            Debug.LogError("No se ha asignado un archivo CSV en el Inspector.");
            return;
        }

        LoadCSV();
        enemyName = "Eyes"; // Cambia esto por el nombre del enemigo que buscas
        Dictionary<string, string> enemyData = GetRowByEnemyName(enemyName);

        if (enemyData != null)
        {
            Debug.Log($"Datos de {enemyName}: {string.Join(", ", enemyData)}");

            // Ejemplo de cómo obtener solo un dato específico
            string info = GetSpecificData(enemyName, "Info");
            Debug.Log($"Info de {enemyName}: {info}");
        }
        else
        {
            Debug.LogWarning($"No se encontró información para {enemyName}");
        }


        //enemyName = "Eyes";

        if (playerTransform == null)
        {

            GameObject playerObject = GameObject.Find("Player");
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


    void LoadCSV()
    {
        string[] lines = csvFile.text.Split('\n');
        string[] headers = lines[0].Split(','); // Obtener nombres de columnas

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            Dictionary<string, string> row = new Dictionary<string, string>();

            for (int j = 0; j < headers.Length; j++)
            {
                row[headers[j].Trim()] = values[j].Trim();
            }
            data.Add(row);
        }
    }

    Dictionary<string, string> GetRowByEnemyName(string enemyName)
    {
        foreach (var row in data)
        {
            if (row.ContainsKey("Name") && row["Name"] == enemyName)
            {
                return row; // Devuelve toda la fila
            }
        }
        return null;
    }

    string GetSpecificData(string enemyName, string columnName)
    {
        Dictionary<string, string> row = GetRowByEnemyName(enemyName);
        if (row != null && row.ContainsKey(columnName))
        {
            return row[columnName]; // Devuelve solo el dato solicitado
        }
        return "Dato no encontrado";
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
        if (playerTransform == null || detectionSphere == null || visionSphere == null || playerCamera == null) return;

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
                    Debug.Log($"Player damaged by {damageAmount}. Current health: {player.currentHealth}");
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
