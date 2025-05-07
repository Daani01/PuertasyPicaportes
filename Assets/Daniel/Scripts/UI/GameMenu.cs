using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [Header("Canvas UI")]
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public GameObject controlsMenuUI;
    public GameObject statsMenuUI;
    public GameObject creditsMenuUI;

    [Header("Estadisticas")]
    public TMP_Text tiempoRecord;
    public TMP_Text puertaRecord;
    public TMP_Text tiempoTotal;
    public TMP_Text intentos;
    public TMP_Text victorias;
    public TMP_Text screechName;
    public TMP_Text screechValue;

    public TMP_Text eyesName;
    public TMP_Text eyesValue;
    public TMP_Text rushName;
    public TMP_Text rushValue;
    public TMP_Text ambushName;
    public TMP_Text ambushValue;
    public TMP_Text jackName;
    public TMP_Text jackValue;
    public TMP_Text a60Name;
    public TMP_Text a60Value;
    public TMP_Text a90Name;
    public TMP_Text a90Value;
    public TMP_Text a120Name;
    public TMP_Text a120Value;


    [Header("Graficos")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown screenModeDropdown;
    public TMP_Dropdown qualityDropdown;
    public Toggle vSyncToggle;
    public TMP_Dropdown fpsLimitDropdown;    
    private Resolution[] resolutions;

    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("FPS")]
    public TMP_Text infoFPSText; // Asigna este TextMeshPro en el Inspector

    private float deltaTime = 0.0f;

    private void Update()
    {
        ShowInfo();
    }

    void ShowInfo()
    {
        // Calcula FPS usando deltaTime
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        // Actualiza el TextMeshPro con FPS, CPU y GPU
        infoFPSText.text = $"FPS: {Mathf.Ceil(fps)}";
    }

    private void Start()
    {
        LoadResolutions();
        LoadSettings();
    }

    public void ResumeGame()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.GetComponent<FirstPersonController>().ResumeGame();
        }
    }

    public void OpenOptionsMenu()
    {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        optionsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        //ApplySettings();
    }

    public void OpenControlsMenu()
    {
        pauseMenuUI.SetActive(false);
        controlsMenuUI.SetActive(true);
    }
    public void CloseControlsMenu()
    {
        controlsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        //ApplySettings();
    }
    public void OpenStatsMenu()
    {
        var data = SaveSystem.LoadPlayerData();

        if (data != null)
        {
            if(data.recordTime != "23:59:59")
            {
                tiempoRecord.text = data.recordTime;
            }

            puertaRecord.text = data.doorRecord.ToString();
            tiempoTotal.text = data.globalTime;
            intentos.text = data.attempts.ToString();
            victorias.text = data.wins.ToString();

            if (data.deathsByScreech > 0)
            {
                screechValue.text = data.deathsByScreech.ToString();
                screechName.text = "SCREECH";
            }
            else
            {
                screechName.text = "???";
            }

            if (data.deathsByEyes > 0)
            {
                eyesValue.text = data.deathsByEyes.ToString();
                eyesName.text = "EYES";
            }
            else
            {
                eyesName.text = "???";
            }

            if (data.deathsByRush > 0)
            {
                rushValue.text = data.deathsByRush.ToString();
                rushName.text = "RUSH";
            }
            else
            {
                rushName.text = "???";
            }

            if (data.deathsByAmbush > 0)
            {
                ambushValue.text = data.deathsByAmbush.ToString();
                ambushName.text = "AMBUSH";
            }
            else
            {
                ambushName.text = "???";
            }

            if (data.deathsByJack > 0)
            {
                jackValue.text = data.deathsByJack.ToString();
                jackName.text = "JACK";
            }
            else
            {
                jackName.text = "???";
            }

            if (data.deathsByA60 > 0)
            {
                a60Value.text = data.deathsByA60.ToString();
                a60Name.text = "A60";
            }
            else
            {
                a60Name.text = "???";
            }

            if (data.deathsByA90 > 0)
            {
                a90Value.text = data.deathsByA90.ToString();
                a90Name.text = "A90";
            }
            else
            {
                a90Name.text = "???";
            }

            if (data.deathsByA120 > 0)
            {
                a120Value.text = data.deathsByA120.ToString();
                a120Name.text = "A120";
            }
            else
            {
                a120Name.text = "???";
            }            

        }

        pauseMenuUI.SetActive(false);
        statsMenuUI.SetActive(true);
    }
    public void CloseStatsMenu()
    {
        statsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        //ApplySettings();
    }
    public void OpenCreditsMenu()
    {
        pauseMenuUI.SetActive(false);
        creditsMenuUI.SetActive(true);
    }
    public void CloseCreditsMenu()
    {
        creditsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        //ApplySettings();
    }

    public void ResetGame()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.GetComponent<FirstPersonController>().InstaDie();
        }
    }

    public void EndGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ------------------------- GRÁFICOS -------------------------
    private void LoadResolutions()
    {
        // Resoluciones más comunes y básicas (puedes ajustar esta lista según tus necesidades)
        List<Vector2Int> commonResolutions = new List<Vector2Int>
    {
        new Vector2Int(1280, 720),    // HD
        new Vector2Int(1920, 1080),   // Full HD
        new Vector2Int(2560, 1440),   // 2K
        new Vector2Int(3840, 2160)    // 4K
    };

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();
        List<Resolution> filteredResolutions = new List<Resolution>();

        foreach (Resolution res in resolutions)
        {
            Vector2Int current = new Vector2Int(res.width, res.height);
            if (commonResolutions.Contains(current) && !filteredResolutions.Exists(r => r.width == res.width && r.height == res.height))
            {
                filteredResolutions.Add(res);
                resolutionOptions.Add(res.width + " x " + res.height);
            }
        }

        // Si no hay ninguna resolución encontrada, se usa al menos la actual
        if (resolutionOptions.Count == 0)
        {
            Resolution current = Screen.currentResolution;
            resolutionOptions.Add(current.width + " x " + current.height);
            filteredResolutions.Add(current);
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        int savedIndex = PlayerPrefs.GetInt("Resolution", filteredResolutions.Count - 1);
        resolutionDropdown.value = Mathf.Clamp(savedIndex, 0, filteredResolutions.Count - 1);
        resolutionDropdown.RefreshShownValue();

        // Actualizar el array con solo las resoluciones filtradas
        resolutions = filteredResolutions.ToArray();
    }


    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
        PlayerPrefs.SetInt("Resolution", index);
    }

    /*
    public void SetScreenMode(int index)
    {
        Screen.fullScreenMode = (FullScreenMode)index;
        PlayerPrefs.SetInt("ScreenMode", index);
    }
    */
    public void SetScreenMode(int index)
    {
        FullScreenMode newMode = (FullScreenMode)index;

        // Usamos la resolución actualmente seleccionada
        Resolution currentResolution = Screen.currentResolution;

        // Reaplica la resolución con el nuevo modo de pantalla
        Screen.SetResolution(currentResolution.width, currentResolution.height, newMode);

        PlayerPrefs.SetInt("ScreenMode", index);
    }


    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("Quality", index);
        //qualityDropdown.RefreshShownValue();  // Asegúrate de que se actualice el valor visible


        fpsLimitDropdown.RefreshShownValue();
    }


    public void SetVSync(bool isOn)
    {
        QualitySettings.vSyncCount = isOn ? 1 : 0;
        PlayerPrefs.SetInt("VSync", isOn ? 1 : 0);
    }

    public void SetFPSLimit(int index)
    {
        int[] fpsLimits = { 30, 60, 120, 144, 240, -1 };
        Application.targetFrameRate = fpsLimits[index];
        PlayerPrefs.SetInt("FPSLimit", index);
    }

    // ------------------------- AUDIO -------------------------
    public void SetMasterVolume(float volume)
{
        audioMixer.SetFloat("MasterVolume", volume); // Esto simula volumen 0 de forma segura

        PlayerPrefs.SetFloat("MasterVolume", volume);
}

    public void SetMusicVolume(float volume)
    {
            audioMixer.SetFloat("MusicVolume", volume);

            PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
            audioMixer.SetFloat("SFXVolume", volume);

            PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void ApplySettings()
    {
        PlayerPrefs.Save();
        CloseOptionsMenu();
    }

    private void LoadSettings()
    {
        // Configurar opciones para cada dropdown
        List<string> screenModes = new List<string> { "Ventana", "Pantalla Completa" };
        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(screenModes);
        screenModeDropdown.value = PlayerPrefs.GetInt("ScreenMode");
        screenModeDropdown.RefreshShownValue();

        List<string> qualityOptions = new List<string> { "Muy baja", "Baja", "Media", "Alta", "Muy alta", "Ultra" };
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(qualityOptions);
        qualityDropdown.value = PlayerPrefs.GetInt("Quality");
        qualityDropdown.RefreshShownValue();

        List<string> fpsOptions = new List<string> { "30", "60", "120", "144", "240" };
        fpsLimitDropdown.ClearOptions();
        fpsLimitDropdown.AddOptions(fpsOptions);
        fpsLimitDropdown.value = PlayerPrefs.GetInt("FPSLimit");
        fpsLimitDropdown.RefreshShownValue();
        SetFPSLimit(PlayerPrefs.GetInt("FPSLimit"));

        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");

    }

}
