using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour, IProcess
{
    public bool IsCompleted { get; private set; } = false;
    public GameObject playerPrefab; // El prefab de tu jugador
    private FirstPersonController currentPlayer; // Referencia al jugador actual

    public void ExecuteProcess(System.Action onComplete)
    {
        StartCoroutine(ProcessRoutine(onComplete));
    }

    private IEnumerator ProcessRoutine(System.Action onComplete)
    {

        yield return StartCoroutine(Spawn());

        IsCompleted = true;

        onComplete?.Invoke();
    }

    private IEnumerator Spawn()
    {
        // Crear al jugador
        if (currentPlayer != null)
        {
            Destroy(currentPlayer.gameObject); // Destruye el jugador anterior si existe
        }

        // Buscar todos los objetos con el tag "Player" en la escena
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Destruir todos los objetos encontrados con el tag "Player"
        foreach (GameObject player in players)
        {
            Debug.Log(player); // Destruye cada jugador encontrado
        }

        GameObject playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity); // Instancia el jugador en la posición deseada
                                                                                                  //currentPlayer = playerInstance.GetComponent<FirstPersonController>();

        // Buscar todos los objetos con el tag "Player" en la escena
        GameObject[] players2 = GameObject.FindGameObjectsWithTag("Player");

        // Destruir todos los objetos encontrados con el tag "Player"
        foreach (GameObject player in players2)
        {
            Debug.Log(player); // Destruye cada jugador encontrado
        }

        // Aquí asignamos las referencias necesarias
        //currentPlayer.canvasManager = FindObjectOfType<ObjectCanvasManager>(); // Asignamos el Canvas Manager
        //currentPlayer.gameloopManager = FindObjectOfType<GameLoop>(); // Asignamos el GameLoop Manager

        yield return null;
    }

}
