using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitOptions : MonoBehaviour
{
    private OptionsMenu optionsMenu;
    void Start()
    {
        InitOptionsNoSave(); 
    }

    private void InitOptionsNoSave()
    {
        OptionsData _optionsData = SaveSystem.LoadPlayerOptions();

        if (_optionsData == null)
        {
            optionsMenu = FindObjectOfType<OptionsMenu>(true);
            SaveSystem.SavePlayerOptions(optionsMenu);
            Debug.Log("INIT SAVED");
        }
    }

}
