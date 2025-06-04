using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoop : MonoBehaviour
{
    [Header("Revive Player")]
    public SpawnPlayer spawnPlayer;

    [SerializeField] private List<GameObject> prefabProcesses;
    [SerializeField] private GDTFadeEffect startFadeEffect;
    [SerializeField] private GDTFadeEffect endFadeEffect;

    [SerializeField] private TMP_Text info_Text;
    [SerializeField] private TMP_Text final_Text;
    [SerializeField] private GameObject endGameButton;
    [SerializeField] private GameObject restartGameButton;
    [SerializeField] private ProceduralRoomGenerator CurrentDoor;

    private string endText = "";

    [Header("Timer Manager")]
    private float elapsedTime = 0f;
    private float maxTime = 23 * 3600f; // 23 horas en segundos

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

                    TimeSpan sessionTime = TimeSpan.Parse(StopTimer());

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
                        case "A60":
                            data.deathsByA60 += 1;
                            break;
                        case "A90":
                            data.deathsByA90 += 1;
                            break;
                        case "A120":
                            data.deathsByA120 += 1;
                            break;
                        case "Ambush":
                            data.deathsByAmbush += 1;
                            break;
                        case "Jack":
                            data.deathsByJack += 1;
                            break;
                        default:
                            Debug.LogWarning($"[WARNING] Enemigo no reconocido: {enemieData.enemyName}");
                            break;
                    }

                }

                SaveSystem.SavePlayerData(data);

                endText = $"{enemieData.dieInfo}\n\n" +
                          $"Has llegado hasta la puerta: {CurrentDoor.GetStaticCurrentRoomIndex()}\n" +
                          $"Tiempo: {TimeSpan.Parse(StopTimer())}\n" +
                          $"Monedas: {data.coins}\n" +
                          $"Puerta record: {data.doorRecord}\n";

                yield return StartCoroutine(TypeTextEffect(endText, 3.0f));

                if(playerController.coinsCount > 300)
                {
                    restartGameButton.GetComponent<Button>().enabled = true;
                }
                else
                {
                    restartGameButton.GetComponent<Button>().enabled = false;
                }

                restartGameButton.SetActive(true);
                endGameButton.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
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

                    TimeSpan sessionTime = TimeSpan.Parse(StopTimer());

                    TimeSpan global = TimeSpan.Parse(data.globalTime);
                    data.globalTime = (global + sessionTime).ToString(@"hh\:mm\:ss");

                    int currentDoorIndex = CurrentDoor.GetStaticCurrentRoomIndex();
                    if (data.doorRecord < currentDoorIndex)
                    {
                        data.doorRecord = currentDoorIndex;
                    }

                }

                SaveSystem.SavePlayerData(data);

                endText = $"Tiempo: {TimeSpan.Parse(StopTimer())}\n" +
                          $"Monedas: {data.coins}\n" +
                          $"Puerta record: {data.doorRecord}\n";

                yield return StartCoroutine(TypeTextEffect(endText, 3.0f));



                //esperar a que el jugador active el revivir

                if (playerController.coinsCount > 300)
                {
                    restartGameButton.GetComponent<Button>().enabled = true;
                }
                else
                {
                    restartGameButton.GetComponent<Button>().enabled = false;
                }

                restartGameButton.SetActive(true);
                endGameButton.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
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
                    data.wins += 1;

                    TimeSpan sessionTime = TimeSpan.Parse(StopTimer());

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

                endText = $"HAS GANADO\n\n" +
                          $"Tiempo: {TimeSpan.Parse(StopTimer())}\n" +
                          $"Record de tiempo: {data.recordTime}\n\n" +
                          $"Monedas: {data.coins}\n" +
                          $"Puerta record: {data.doorRecord}\n";

                yield return StartCoroutine(TypeTextEffect(endText, 3.0f));


                endGameButton.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

            }
        }
    }


    public void RevivePlayer()
    {
        StartCoroutine(RevivePlayerLogic());
    }

    private IEnumerator RevivePlayerLogic()
    {
        endFadeEffect.EndEffect();

        restartGameButton.SetActive(false);
        endGameButton.SetActive(false);

        var data = SaveSystem.LoadPlayerData();
        data.coins -= 300;
        SaveSystem.SavePlayerData(data);

        yield return StartCoroutine(DeleteTextEffect(2.0f));

        spawnPlayer.Spawn();

        StartCoroutine("StartTimer");

        // Como es static accedo directamente sin buscar en la escena
        FirstPersonController playerController = SpawnPlayer.currentPlayer;

        if (playerController != null)
        {
            playerController.blockPlayer = false;
            playerController.Heal(100.0f);
        }
    }


    public IEnumerator StartTimer()
    {
        while (elapsedTime < maxTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public string StopTimer()
    {
        StopCoroutine("StartTimer");
        int hours = Mathf.FloorToInt(elapsedTime / 3600);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);

        return formattedTime;
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

    private IEnumerator DeleteTextEffect(float duration)
    {
        string text = final_Text.text;
        float delay = duration / text.Length;

        for (int i = text.Length - 1; i >= 0; i--)
        {
            final_Text.text = text.Substring(0, i);
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
