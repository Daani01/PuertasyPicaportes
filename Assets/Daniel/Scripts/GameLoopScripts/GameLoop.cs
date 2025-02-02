using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabProcesses;
    [SerializeField] private GDTFadeEffect fadeEffect;

    [SerializeField] private TMP_Text info_Text;


    private List<GameObject> instantiatedObjects = new List<GameObject>();
    private int currentIndex = 0;
    private System.Diagnostics.Stopwatch totalStopwatch = new System.Diagnostics.Stopwatch();

    private void Start()
    {
        totalStopwatch.Start();
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

                    info_Text.text = $"Cargando: {currentIndex}/{prefabProcesses.Count}";
                    //Debug.Log($"[GameLoop] Iniciando proceso en {currentGameObject.name}");

                    process.ExecuteProcess(() =>
                    {
                        processStopwatch.Stop();
                        //Debug.Log($"[GameLoop] Proceso completado en {currentGameObject.name} (Tiempo: {processStopwatch.ElapsedMilliseconds} ms)");
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
        info_Text.text = $"Carga completa en \n{totalStopwatch.ElapsedMilliseconds} ms";
        //Debug.Log($"[GameLoop] Todos los procesos han finalizado. Tiempo total: {totalStopwatch.ElapsedMilliseconds} ms");

        if (fadeEffect != null)
        {
            fadeEffect.StartEffect();
            //StartCoroutine(FadeOutText());
        }

    }

    private IEnumerator FadeOutText()
    {
        float startAlpha = info_Text.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeEffect.timeEffect)
        {
            elapsedTime += Time.deltaTime;
            info_Text.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeEffect.timeEffect);
            yield return null;
        }

        info_Text.alpha = 0f;
    }

}
