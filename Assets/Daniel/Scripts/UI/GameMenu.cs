using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;

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
    public TMP_Text infoText; // Asigna este TextMeshPro en el Inspector

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
        infoText.text = $"FPS: {Mathf.Ceil(fps)}";
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
