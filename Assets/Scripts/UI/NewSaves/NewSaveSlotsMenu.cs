using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewSaveSlotsMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private NewMainMenu mainMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;

    [Header("Confirmation Popup")]
    [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;

    private NewSaveSlot[] saveSlots;

    private bool isLoadingGame = false;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<NewSaveSlot>();
    }

    public void PlayButtonClicked()
    {
        AudioManager.instance.PlayButtonClick();
    }

    public void OnSaveSlotClicked(NewSaveSlot saveSlot)
    {
        // disable all buttons
        DisableMenuButtons();

        // case - loading game
        if (isLoadingGame)
        {
            Debug.Log("Loaded Save Slot Clicked");
            NewDataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            SaveGameAndLoadScene_Load();
        }
        // case - new game, but the save slot has data
        else if (saveSlot.hasData)
        {
            confirmationPopupMenu.ActivateMenu(
                "Starting a New Game with this slot will override the currently saved data. Are you sure?",
                // function to execute if we select 'yes'
                () =>
                {
                    NewDataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    NewDataPersistenceManager.instance.NewGame();
                    SaveGameAndLoadScene_New();
                },
                // function to execute if we select 'cancel'
                () =>
                {
                    this.ActivateMenu(isLoadingGame);
                }
            );
        }
        // case - new game, and the save slot has no data
        else
        {
            NewDataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            NewDataPersistenceManager.instance.NewGame();
            SaveGameAndLoadScene_New();
        }
    }

    private void SaveGameAndLoadScene_New()
    {
        // save the game anytime before loading a new scene
        NewDataPersistenceManager.instance.SaveGame();
        // load the scene
        SceneManager.LoadSceneAsync("CharacterCustomization");
        /*int _sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        TransitionManager.instance.NextScene(_sceneIndex);*/
    }

    private void SaveGameAndLoadScene_Load()
    {
        Debug.Log("LoadingGame");
        // NewDataPersistenceManager.instance.gameData.newGame = false;
        NewDataPersistenceManager.instance.SaveGame();
        // change SaveTest to the scene number soon
        // SceneManager.LoadSceneAsync("SaveTest");
        int _sceneIndex = NewDataPersistenceManager.instance.gameData.sceneIndex;

        // Debug.Log(NewDataPersistenceManager.instance.gameData.name + " " + NewDataPersistenceManager.instance.gameData.newGame);
        SceneManager.LoadSceneAsync(_sceneIndex);
        // TransitionManager.instance.NextScene(_sceneIndex);
    }

    public void OnClearClicked(NewSaveSlot saveSlot)
    {
        DisableMenuButtons();
        this.DeactivateMenu();
        confirmationPopupMenu.ActivateMenu(
            "Are you sure you want to delete this saved data?",
            // function to execute if we select 'yes'
            () =>
            {
                NewDataPersistenceManager.instance.DeleteProfileData(saveSlot.GetProfileId());
                ActivateMenu(isLoadingGame);
            },
            // function to execute if we select 'cancel'
            () =>
            {
                ActivateMenu(isLoadingGame);
            }
        );
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        this.DeactivateMenu();
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        // set this menu to be active
        this.gameObject.SetActive(true);

        // set mode
        this.isLoadingGame = isLoadingGame;

        // load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = NewDataPersistenceManager.instance.GetAllProfilesGameData();

        // ensure the back button is enabled when we activate the menu
        backButton.interactable = true;

        // loop through each save slot in the UI and set the content appropriately
        //GameObject firstSelected = backButton.gameObject;
        foreach (NewSaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);
            if (profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else if (profileData != null && !isLoadingGame)
            {
                saveSlot.DisableSaveSlotInteractable(false);
            }
            else if (profileData != null && isLoadingGame)
            {
                if (profileData.medalsCollected == profileData.totalMedals)
                {
                    saveSlot.DisableSaveSlotInteractable(false);
                }
            }
            else
            {
                saveSlot.SetInteractable(true);
            }
        }
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

    private void DisableMenuButtons()
    {
        foreach (NewSaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }
        backButton.interactable = false;
    }
}