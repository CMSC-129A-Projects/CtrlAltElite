using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestMenuSave : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;

    private void Start()
    {
        Debug.Log("Menu: " + DataPersistenceManager.instance.HasGameData());
        if (!DataPersistenceManager.instance.HasGameData())
        {
            continueGameButton.interactable = false; 
        }
    }
    public void OnNewGameClicked()
    {
        DisableAllButtons();
        Debug.Log("New Game Clicked");
        DataPersistenceManager.instance.NewGame();
        SceneManager.LoadSceneAsync("SaveTest");
    }

    public void OnContinueGameClicked()
    {
        DisableAllButtons();
        Debug.Log("Continue Clicked");
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadSceneAsync("SaveTest");
    }

    private void DisableAllButtons()
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
    }
}
