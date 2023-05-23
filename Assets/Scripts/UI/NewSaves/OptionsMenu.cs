using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private NewMainMenu mainMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }
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


}
