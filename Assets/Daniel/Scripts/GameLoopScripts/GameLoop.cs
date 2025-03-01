using System;
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
    [SerializeField] private TMP_Text final_Text;
    [SerializeField] private ProceduralRoomGenerator CurrentDoor;

    private string endText = "";


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
                yield return new WaitForSeconds(startFadeEffect.timeEffect);
                if(player != null)
                {
                    player.GetComponent<FirstPersonController>().blockPlayer = false;
                }
            }


            //StartCoroutine(FadeOutText());
        }

    }


    public void PlayerEndGameDead(Enemie enemie)
    {
        StartCoroutine(EndGameDead(enemie));
    }

    private IEnumerator EndGameDead(Enemie enemieData)
    {
        var data = SaveSystem.LoadPlayerData();

        if (data == null)
        {
            data = new SaveSystem.PlayerData();
            data.recordTime = "23:59:59";
            data.globalTime = "00:00:00";
        }

        if (endFadeEffect != null)
        {
            endFadeEffect.StartEffect();

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                FirstPersonController playerController = player.GetComponent<FirstPersonController>();
                if (playerController != null)
                {
                    playerController.DisableInputs();

                    data.coins = playerController.coinsCount;
                    data.attempts += 1;

                    TimeSpan sessionTime = TimeSpan.Parse(playerController.StopTimer());

                    TimeSpan global = TimeSpan.Parse(data.globalTime);
                    data.globalTime = (global + sessionTime).ToString(@"hh\:mm\:ss");

                    int currentDoorIndex = CurrentDoor.GetStaticCurrentRoomIndex();
                    if (data.doorRecord < currentDoorIndex)
                    {
                        data.doorRecord = currentDoorIndex;
                    }

                    switch (enemieData.enemyName)
                    {
                        case "Rush":
                            data.deathsByRush += 1;
                            break;
                        case "Eyes":
                            data.deathsByEyes += 1;
                            break;
                        case "Screech":
                            data.deathsByScreech += 1;
                            break;
                        default:
                            Debug.LogWarning($"[WARNING] Enemigo no reconocido: {enemieData.enemyName}");
                            break;
                    }

                }

                SaveSystem.SavePlayerData(data);

                endText = $"{enemieData.dieInfo}\n" +
                          $"Has llegado hasta la puerta: {CurrentDoor.GetStaticCurrentRoomIndex()}\n" +
                          $"Tiempo: {TimeSpan.Parse(playerController.StopTimer())}\n" +
                          $"Monedas: {data.coins}\n" +
                          $"Puerta record: {data.doorRecord}\n";

                yield return StartCoroutine(TypeTextEffect(endText, 5.0f));

                yield return new WaitForSeconds(3.5f);

                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            }
        }
    }


    public void PlayerEndGameRestart()
    {
        StartCoroutine(EndGameRestart());
    }

    private IEnumerator EndGameRestart()
    {
        var data = SaveSystem.LoadPlayerData();

        if (data == null)
        {
            data = new SaveSystem.PlayerData();
            data.recordTime = "23:59:59";
            data.globalTime = "00:00:00";
        }

        if (endFadeEffect != null)
        {
            endFadeEffect.StartEffect();

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                FirstPersonController playerController = player.GetComponent<FirstPersonController>();
                if (playerController != null)
                {
                    playerController.DisableInputs();

                    data.coins = playerController.coinsCount;
                    data.attempts += 1;

                    TimeSpan sessionTime = TimeSpan.Parse(playerController.StopTimer());

                    TimeSpan global = TimeSpan.Parse(data.globalTime);
                    data.globalTime = (global + sessionTime).ToString(@"hh\:mm\:ss");

                    int currentDoorIndex = CurrentDoor.GetStaticCurrentRoomIndex();
                    if (data.doorRecord < currentDoorIndex)
                    {
                        data.doorRecord = currentDoorIndex;
                    }

                }

                SaveSystem.SavePlayerData(data);

                endText = $"Tiempo: {TimeSpan.Parse(playerController.StopTimer())}\n" +
                          $"Monedas: {data.coins}\n" +
                          $"Puerta record: {data.doorRecord}\n";

                yield return StartCoroutine(TypeTextEffect(endText, 5.0f));

                yield return new WaitForSeconds(3.5f);

                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            }
        }
    }



    public void PlayerEndGameWin()
    {
        StartCoroutine(EndGameWin());
    }

    private IEnumerator EndGameWin()
    {

        var data = SaveSystem.LoadPlayerData();

        if (data == null)
        {
            data = new SaveSystem.PlayerData();
            data.recordTime = "23:59:59";
            data.globalTime = "00:00:00";
        }

        if (endFadeEffect != null)
        {
            endFadeEffect.StartEffect();

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                FirstPersonController playerController = player.GetComponent<FirstPersonController>();
                if (playerController != null)
                {
                    playerController.DisableInputs();

                    data.coins = playerController.coinsCount;
                    data.attempts += 1;

                    TimeSpan sessionTime = TimeSpan.Parse(playerController.StopTimer());

                    TimeSpan bestTime = TimeSpan.Parse(data.recordTime);
                    if (sessionTime < bestTime)
                    {
                        data.recordTime = sessionTime.ToString(@"hh\:mm\:ss");
                    }

                    TimeSpan global = TimeSpan.Parse(data.globalTime);
                    data.globalTime = (global + sessionTime).ToString(@"hh\:mm\:ss");

                    int currentDoorIndex = CurrentDoor.GetStaticCurrentRoomIndex();
                    if (data.doorRecord < currentDoorIndex)
                    {
                        data.doorRecord = currentDoorIndex;
                    }

                }

                SaveSystem.SavePlayerData(data);

                endText = $"HAS GANADO\n" +
                          $"Tiempo: {TimeSpan.Parse(playerController.StopTimer())}\n" +
                          $"Record de tiempo: {data.recordTime}\n\n" +
                          $"Monedas: {data.coins}\n" +
                          $"Puerta record: {data.doorRecord}\n";

                yield return StartCoroutine(TypeTextEffect(endText, 5.0f));

                yield return new WaitForSeconds(3.5f);

                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            }
        }
    }

    private IEnumerator TypeTextEffect(string text, float duration)
    {
        final_Text.text = "";
        float delay = duration / text.Length;

        foreach (char letter in text)
        {
            final_Text.text += letter;
            yield return new WaitForSeconds(delay);
        }
    }

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
