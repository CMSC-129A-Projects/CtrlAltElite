using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool Paused = false;
    public bool inOptions = false;
    public GameObject PauseMenuCanvas;
    public OptionsMenu optionsMenu;

    void Start()
    {
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inOptions)
            {
                onOptionsBackClicked();
            }
            else if (Paused)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }
    }

    void Stop()
    {
        PauseMenuCanvas.SetActive(true);
            Time.timeScale = 0f;
        Paused = true;
    }

    public void Play()
    {
        PauseMenuCanvas.SetActive(false);
            Time.timeScale = 1f;
        Paused = false;
    }

    public void MainMenuButton()
    {
        NewDataPersistenceManager.instance.SaveGame();
        SceneManager.LoadScene("TestMenuSave");
    }

    public void OptionsButton()
    {
        PauseMenuCanvas.SetActive(false);
        inOptions = true;
        optionsMenu.ActivateMenu();
    }

    public void onOptionsBackClicked()
    {
        optionsMenu.DeactivateMenu();
        inOptions = false;
        PauseMenuCanvas.SetActive(true);
    }

    public void PlayButtonClicked()
    {
        AudioManager.instance.PlayButtonClick();
    }
}
