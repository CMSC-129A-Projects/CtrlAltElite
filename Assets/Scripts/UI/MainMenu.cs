using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
        SceneManager.LoadScene("MainMenu");
    }
}

