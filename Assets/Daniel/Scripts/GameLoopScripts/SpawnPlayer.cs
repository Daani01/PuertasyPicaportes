using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour, IProcess
{
    public bool IsCompleted { get; private set; } = false;
    public GameObject playerPrefab;
    public static FirstPersonController currentPlayer;

    public void ExecuteProcess(System.Action onComplete)
    {
        Spawn();
        IsCompleted = true;
        onComplete?.Invoke();
    }

    public void Spawn()
    {
        Vector3 spawnPosition = Vector3.zero;

        if (currentPlayer != null)
        {
            spawnPosition = currentPlayer.transform.position;
            currentPlayer.RemoveAllItemsRevive();
            DestroyImmediate(currentPlayer.gameObject); // Destruir YA
        }

        // Buscar y destruir TODAS las cámaras
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            DestroyImmediate(cam.gameObject); // Destruir YA
        }

        // Buscar y destruir TODAS las cámaras
        Enemie[] enemies = FindObjectsOfType<Enemie>();
        foreach (Enemie enemie in enemies)
        {
            if (enemie.gameObject.activeSelf)
            {
                DestroyImmediate(enemie.gameObject); // Destruir YA
            }
        }

        // Instanciar el nuevo jugador
        GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        currentPlayer = playerInstance.GetComponent<FirstPersonController>();
    }






}
