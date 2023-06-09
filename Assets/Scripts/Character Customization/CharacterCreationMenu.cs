using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCreationMenu : MonoBehaviour, IDataPersistence
{
    public List<OutfitChanger> outfitChangers = new List<OutfitChanger>();
    [SerializeField] private TMP_InputField inputField;

    [Header("Random Names")]
    public List<string> randomNames = new List<string>();

    public void RandomizeCharacter(){
        int nameIndex = Random.Range(0, randomNames.Count);
        inputField.text = randomNames[nameIndex];

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

    public void PlayButtonClick()
    {
        AudioManager.instance.PlayButtonClick();
    }

    public void StartGame()
    {
        Debug.Log("Starting Game");
        RandomizeForeigner();
        NewDataPersistenceManager.instance.SaveGame();
        SceneManager.LoadSceneAsync("City 1");
        //SceneManager.LoadSceneAsync("SaveTest");
        // SceneManager.LoadSceneAsync("NewIntroScene");
    }

    public void Hello()
    {
        Debug.Log("Hello");
    }

    public void OnBackClicked()
    {
        NewDataPersistenceManager.instance.DeleteSelectedProfileId();
        SceneManager.LoadSceneAsync("TestMenuSave");
    }

    public void RandomizeForeigner()
    {
        // Randomize foreigner

        List<int> _foreigner_list = new List<int>();

        foreach (OutfitChanger changer in outfitChangers)
        {
            _foreigner_list.Add(Random.Range(0, changer.options.Count));
        }

        NewDataPersistenceManager.instance.gameData.FheadIndex = _foreigner_list[0];
        NewDataPersistenceManager.instance.gameData.FbodyIndex = _foreigner_list[1];
        NewDataPersistenceManager.instance.gameData.FarmIndex = _foreigner_list[2];
        NewDataPersistenceManager.instance.gameData.FlegIndex = _foreigner_list[3];

        int _nameIndex = Random.Range(0, randomNames.Count);
        NewDataPersistenceManager.instance.gameData.Fname = randomNames[_nameIndex];
    }

    #region SAVE STUFF
    public void LoadData(GameData data)
    {
        outfitChangers[0].CurrentOption = data.headIndex;
        outfitChangers[0].bodyPart.sprite = outfitChangers[0].options[data.headIndex];

        outfitChangers[1].CurrentOption = data.bodyIndex;
        outfitChangers[1].bodyPart.sprite = outfitChangers[1].options[data.bodyIndex];

        outfitChangers[2].CurrentOption = data.armIndex;
        outfitChangers[2].bodyPart.sprite = outfitChangers[2].options[data.armIndex];

        outfitChangers[3].CurrentOption = data.legIndex;
        outfitChangers[3].bodyPart.sprite = outfitChangers[3].options[data.legIndex];
    }

    public void SaveData(GameData data)
    {
        data.headIndex = outfitChangers[0].CurrentOption;
        data.bodyIndex = outfitChangers[1].CurrentOption;
        data.armIndex = outfitChangers[2].CurrentOption;
        data.legIndex = outfitChangers[3].CurrentOption;
        data.name = inputField.text;
    }
    #endregion
}
