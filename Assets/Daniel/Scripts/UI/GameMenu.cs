using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    //public GameObject mainMenuUI;
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
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
        if (optionsMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            optionsMenuUI.SetActive(true);
        }
    }

    public void CloseOptionsMenu()
    {
        if (optionsMenuUI != null)
        {
            optionsMenuUI.SetActive(false);
            pauseMenuUI.SetActive(true);
        }
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

}
