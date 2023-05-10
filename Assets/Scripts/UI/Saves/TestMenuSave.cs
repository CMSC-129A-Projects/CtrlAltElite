using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestMenuSave : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private TestSaveSlotsMenu saveSlotsMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button loadGameButton;

    private void Start()
    {
        Debug.Log("Menu: " + DataPersistenceManager.instance.HasGameData());
        if (!DataPersistenceManager.instance.HasGameData())
        {
            continueGameButton.interactable = false; 
            loadGameButton.interactable = false;
        }
    }
    public void OnNewGameClicked()
    {
        /*DisableAllButtons();
        Debug.Log("New Game Clicked");
        DataPersistenceManager.instance.NewGame();
        SceneManager.LoadSceneAsync("SaveTest");*/
        saveSlotsMenu.ActivateMenu(false);
        this.DeactivateMenu();
    }

    public void OnContinueGameClicked()
    {
        DisableAllButtons();
        Debug.Log("Continue Clicked");
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadSceneAsync("SaveTest");
    }

    public void OnLoadGameClicked()
    {
        saveSlotsMenu.ActivateMenu(true);
        this.DeactivateMenu();
    }

    private void DisableAllButtons()
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
    }
    public void ActivateMenu()
    {
        this.gameObject.SetActive(true);
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

}
