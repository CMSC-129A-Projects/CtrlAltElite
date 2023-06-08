using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private NewMainMenu mainMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;


    public void ActivateMenu()
    {
        // set this menu to be active
        this.gameObject.SetActive(true);
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        this.DeactivateMenu();
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

    public void PlayButtonClicked()
    {
        AudioManager.instance.PlayButtonClick();
    }
}
