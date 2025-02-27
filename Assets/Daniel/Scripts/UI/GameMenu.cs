using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

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
    public TMP_Dropdown shadowsDropdown;
    public TMP_Dropdown antialiasingDropdown;
    public TMP_Dropdown texturesDropdown;
    public Toggle ambientOcclusionToggle;
    public Toggle postProcessingToggle;
    private Resolution[] resolutions;

    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Controles")]
    public Slider mouseSensitivitySlider;
    public Toggle invertYAxisToggle;

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

        // Obtiene uso aproximado de la CPU
        float cpuUsage = SystemInfo.processorCount * SystemInfo.processorFrequency / 1000f; // Estimación

        // Obtiene uso aproximado de la GPU
        float gpuUsage = SystemInfo.graphicsMemorySize; // Muestra VRAM total, no carga en tiempo real

        // Actualiza el TextMeshPro con FPS, CPU y GPU
        infoText.text = $"FPS: {Mathf.Ceil(fps)} CPU: ~{cpuUsage:F1} GHz GPU VRAM: {gpuUsage} MB";
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
        // Asignar las resoluciones al dropdown de resoluciones
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionOptions.Add(resolutions[i].width + " x " + resolutions[i].height);
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", resolutions.Length - 1);
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
        PlayerPrefs.SetInt("Resolution", index);
    }

    public void SetScreenMode(int index)
    {
        Screen.fullScreenMode = (FullScreenMode)index;
        PlayerPrefs.SetInt("ScreenMode", index);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("Quality", index);
        qualityDropdown.RefreshShownValue();  // Asegúrate de que se actualice el valor visible
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


    /*
    public void SetShadows(int index)
    {
        QualitySettings.shadows = (ShadowQuality)index;
        PlayerPrefs.SetInt("Shadows", index);
    }

    public void SetAntialiasing(int index)
    {
        int[] aaValues = { 0, 2, 4, 8 };
        QualitySettings.antiAliasing = aaValues[index];
        PlayerPrefs.SetInt("AA", index);
    }

    public void SetTextures(int index)
    {
        int[] textureQualities = { 2, 1, 0 };
        QualitySettings.globalTextureMipmapLimit = textureQualities[index];
        PlayerPrefs.SetInt("Textures", index);
    }
    */

    // ------------------------- CONTROLES -------------------------

    /*
    public void SetMouseSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }

    public void SetInvertYAxis(bool isInverted)
    {
        PlayerPrefs.SetInt("InvertYAxis", isInverted ? 1 : 0);
    }
    */

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
        screenModeDropdown.value = PlayerPrefs.GetInt("ScreenMode", 1);
        screenModeDropdown.RefreshShownValue();

        List<string> qualityOptions = new List<string> { "Muy baja", "Baja", "Media", "Alta", "Muy alta", "Ultra" };
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(qualityOptions);
        qualityDropdown.value = PlayerPrefs.GetInt("Quality", 2);
        qualityDropdown.RefreshShownValue();

        List<string> fpsOptions = new List<string> { "30", "60", "120", "144", "240" };
        fpsLimitDropdown.ClearOptions();
        fpsLimitDropdown.AddOptions(fpsOptions);
        fpsLimitDropdown.value = PlayerPrefs.GetInt("FPSLimit", 1);
        fpsLimitDropdown.RefreshShownValue();

        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        /*
        List<string> shadowsOptions = new List<string> { "Bajo", "Medio", "Alto" };//BAJO NO //cambiar shadow resolution 4 opciones //pixel light count 3 / 4
        shadowsDropdown.ClearOptions();
        shadowsDropdown.AddOptions(shadowsOptions);
        shadowsDropdown.value = PlayerPrefs.GetInt("Shadows", 2);
        shadowsDropdown.RefreshShownValue();

        List<string> antialiasingOptions = new List<string> { "Ninguno", "2x", "4x", "8x" };
        antialiasingDropdown.ClearOptions();
        antialiasingDropdown.AddOptions(antialiasingOptions);
        antialiasingDropdown.value = PlayerPrefs.GetInt("AA", 1);
        antialiasingDropdown.RefreshShownValue();

        List<string> texturesOptions = new List<string> { "Baja", "Media", "Alta" }; //4 tipos
        texturesDropdown.ClearOptions();
        texturesDropdown.AddOptions(texturesOptions);
        texturesDropdown.value = PlayerPrefs.GetInt("Textures", 1);
        texturesDropdown.RefreshShownValue();
        */
    }

}
