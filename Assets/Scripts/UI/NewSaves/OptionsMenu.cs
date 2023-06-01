using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour, IDataPersistence
{
    [Header("Menu Navigation")]
    [SerializeField] private NewMainMenu mainMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Resolution Dropdown")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;
    private void Start()
    {
        InitResolution();
    }

    private void InitResolution()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if ((resolutions[i].width == Screen.currentResolution.width) && 
                (resolutions[i].height == Screen.currentResolution.height))
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }


    #region SAVE STUFF
    public void LoadData(GameData data)
    {
        if (data == null) return;
        SetVolume(data.volumePreference);
        SetQuality(data.qualityIndex);
        SetFullScreen(data.isFullScreen);
    }

    public void SaveData(GameData data)
    {
        if (data == null) return;
        audioMixer.GetFloat("Volume", out float volume);
        data.volumePreference = volume;
        data.qualityIndex = QualitySettings.GetQualityLevel();
        data.isFullScreen = Screen.fullScreen;
    }
    #endregion




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
