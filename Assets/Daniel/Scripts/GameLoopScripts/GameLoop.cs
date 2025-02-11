using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoop : MonoBehaviour
{

    [SerializeField] private List<GameObject> prefabProcesses;
    [SerializeField] private GDTFadeEffect startFadeEffect;
    [SerializeField] private GDTFadeEffect endFadeEffect;

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

        if (startFadeEffect != null)
        {
            startFadeEffect.StartEffect();

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                yield return new WaitForSeconds(2.0f);
                player.GetComponent<FirstPersonController>().blockPlayer = false;
            }


            //StartCoroutine(FadeOutText());
        }

    }


    public void FadeEffectFinish()
    {
        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        if (endFadeEffect != null)
        {
            endFadeEffect.StartEffect();

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                // Bloqueamos al jugador antes de esperar (evita errores)
                FirstPersonController playerController = player.GetComponent<FirstPersonController>();
                if (playerController != null)
                {
                    playerController.blockPlayer = true;
                    playerController.StopAllCoroutines();
                }

                yield return new WaitForSeconds(5.0f);

                if (player != null)
                {
                    // Desactivamos el objeto o desactivamos sus componentes
                    
                    player.GetComponent<FirstPersonController>().enabled = false; // Desactiva los scripts del jugador
                    player.GetComponent<Collider>().enabled = false; // Desactiva el collider, si lo tiene

                    // Destruye el jugador anterior despu�s de desactivar sus componentes
                    Destroy(player.gameObject); // Espera un segundo antes de destruirlo para asegurarte de que todo se haya desactivado
                }


                yield return new WaitForSeconds(0.5f);

                // Recargar la escena desde cero
                SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            }
        }
    }

    // Asegura que OnSceneLoaded no se registre m�ltiples veces





    private IEnumerator FadeOutText()
    {
        float startAlpha = info_Text.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < startFadeEffect.timeEffect)
        {
            elapsedTime += Time.deltaTime;
            info_Text.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / startFadeEffect.timeEffect);
            yield return null;
        }

        info_Text.alpha = 0f;
    }

}
