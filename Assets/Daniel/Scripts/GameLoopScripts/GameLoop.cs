using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabProcesses; // Prefabs a instanciar y ejecutar
    [SerializeField] private GDTFadeEffect fadeEffect; // Referencia al efecto de fade

    private List<GameObject> instantiatedObjects = new List<GameObject>();
    private int currentIndex = 0;
    private System.Diagnostics.Stopwatch totalStopwatch = new System.Diagnostics.Stopwatch(); // Cronómetro para tiempo total

    private void Start()
    {
        totalStopwatch.Start(); // Inicia el tiempo total
        StartCoroutine(RunGameLoop());
    }

    private IEnumerator RunGameLoop()
    {
        while (currentIndex < prefabProcesses.Count)
        {
            GameObject prefabToInstantiate = prefabProcesses[currentIndex];

            if (prefabToInstantiate != null)
            {
                GameObject currentGameObject = Instantiate(prefabToInstantiate, transform);
                instantiatedObjects.Add(currentGameObject);

                IProcess process = currentGameObject.GetComponent<IProcess>();

                if (process != null)
                {
                    System.Diagnostics.Stopwatch processStopwatch = System.Diagnostics.Stopwatch.StartNew();

                    //Debug.Log($"[GameLoop] Iniciando proceso en {currentGameObject.name}");
                    process.ExecuteProcess(() =>
                    {
                        processStopwatch.Stop();
                        Debug.Log($"[GameLoop] Proceso completado en {currentGameObject.name} (Tiempo: {processStopwatch.ElapsedMilliseconds} ms)");
                        OnProcessCompleted();
                    });

                    yield return new WaitUntil(() => process.IsCompleted);
                }
                else
                {
                    Debug.LogWarning($"[GameLoop] {currentGameObject.name} no tiene un script que implemente IProcess.");
                }
            }

            currentIndex++;
        }

        totalStopwatch.Stop();
        Debug.Log($"[GameLoop] Todos los procesos han finalizado. Tiempo total: {totalStopwatch.ElapsedMilliseconds} ms");

        //Llamamos al efecto de fade cuando todos los procesos terminan
        if (fadeEffect != null)
        {
            Debug.Log("[GameLoop] Activando efecto de fade...");
            fadeEffect.StartEffect();
        }
        else
        {
            Debug.LogWarning("[GameLoop] No se ha asignado el efecto de fade.");
        }
    }

    private void OnProcessCompleted()
    {
        //Debug.Log("[GameLoop] Proceso finalizado, avanzando al siguiente.");
    }
}
