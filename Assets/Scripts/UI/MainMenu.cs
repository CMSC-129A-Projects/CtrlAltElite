using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnNewGameClicked()
    {
        Debug.Log("New Game Clicked");
        // initialize game data
        // DataPersistenceManager.instance.NewGame();
        // load scene
        SceneManager.LoadSceneAsync("SaveTest");
    }

    public void OnContinueClicked()
    {
        Debug.Log("Continue Clicked");
        SceneManager.LoadSceneAsync("SaveTest");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Player Has Quit The Game");
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("Settings menu");
    }

    public void ChangeScenetoMenu()
    {
        SceneManager.LoadSceneAsync("TestMenuSave");
    }
}

