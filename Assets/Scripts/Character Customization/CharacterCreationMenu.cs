using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCreationMenu : MonoBehaviour, IDataPersistence
{
    public List<OutfitChanger> outfitChangers = new List<OutfitChanger>();
    [SerializeField] private TMP_InputField inputField;

    public void RandomizeCharacter(){
        foreach (OutfitChanger changer in outfitChangers)
        {
            changer.Randomize();
        }
    }

    private void Update()
    {
        if (inputField.isFocused)
        {
            SugboMovement.canMove = false;
        }
        else
        {
            SugboMovement.canMove = true;
        }
    }

    public void StartGame()
    {
        /*NewDataPersistenceManager.instance.SaveGame();
        // load the next scene - which will in turn load the game because of 
        // OnSceneLoaded() in the DataPersistenceManager
        SceneManager.LoadSceneAsync("SaveTest");*/
        Debug.Log("StartGame()");
        NewDataPersistenceManager.instance.SaveGame();
        // SceneManager.LoadSceneAsync("SaveTest");

        /*NewDataPersistenceManager.instance.gameData.position.x = -73.00418090820313f;
        NewDataPersistenceManager.instance.gameData.position.y = -18.1600341796875f;*/
        SceneManager.LoadSceneAsync("City 3");
    }

    #region SAVE STUFF
    public void LoadData(GameData data)
    {
        // Debug.Log($"{data.headIndex} {data.bodyIndex} {data.armIndex} {data.legIndex}");
        outfitChangers[0].CurrentOption = data.headIndex;
        outfitChangers[0].bodyPart.sprite = outfitChangers[0].options[data.headIndex];

        outfitChangers[1].CurrentOption = data.bodyIndex;
        outfitChangers[1].bodyPart.sprite = outfitChangers[1].options[data.bodyIndex];

        outfitChangers[3].CurrentOption = data.armIndex;
        outfitChangers[3].bodyPart.sprite = outfitChangers[3].options[data.armIndex];

        outfitChangers[4].CurrentOption = data.legIndex;
        outfitChangers[4].bodyPart.sprite = outfitChangers[4].options[data.legIndex];

    }

    public void SaveData(GameData data)
    {
        data.headIndex = outfitChangers[0].CurrentOption;
        data.bodyIndex = outfitChangers[1].CurrentOption;
        data.armIndex = outfitChangers[3].CurrentOption;
        data.legIndex = outfitChangers[5].CurrentOption;
        data.name = inputField.text;
    }
    #endregion
}
