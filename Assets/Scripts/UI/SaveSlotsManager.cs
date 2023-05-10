using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotsManager : MonoBehaviour
{
    private SaveSlot[] saveSlots;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        // update the selected profile id
        // DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
        // create a new game
        DataPersistenceManager.instance.NewGame();
        // load gameplay scene asynchronously
        SceneManager.LoadSceneAsync("SaveTest");
    }

    /*public void ActivateMenu()
    {
        // load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        // loop through each of the save slots

        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);
        }
    }*/
}
