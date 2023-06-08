using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewMainMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private NewSaveSlotsMenu saveSlotsMenu;
    [SerializeField] private OptionsMenu optionsMenu;
    [SerializeField] private MapMenu mapMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mapButton;

    private void Start()
    {
        DisableButtonsDependingOnData();
    }

    private void DisableButtonsDependingOnData()
    {
        if (!NewDataPersistenceManager.instance.HasGameData())
        {
            loadGameButton.interactable = false;
        }
    }

    public void OnNewGameClicked()
    {
        saveSlotsMenu.ActivateMenu(false);
        this.DeactivateMenu();
    }

    public void OnLoadGameClicked()
    {
        saveSlotsMenu.ActivateMenu(true);
        this.DeactivateMenu();
    }

    public void OnOptionsClicked()
    {
        optionsMenu.ActivateMenu();
        this.DeactivateMenu();
    }

    public void OnMapClicked()
    {
        mapMenu.ActivateMenu();
        this.DeactivateMenu();

    }
    public void PlayButtonClicked()
    {
        AudioManager.instance.PlayButtonClick();
    }

    private void DisableMenuButtons()
    {
        newGameButton.interactable = false;
    }

    public void ActivateMenu()
    {
        this.gameObject.SetActive(true);
        DisableButtonsDependingOnData();
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
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