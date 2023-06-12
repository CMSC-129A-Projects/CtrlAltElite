using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using TMPro;
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

    [Header("Dropdowns and Sliders")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Slider volumeSlider;

    [SerializeField] private float _volume;
    [SerializeField] private int _qualityIndex;
    [SerializeField] private bool _isFullScreen;

    Resolution[] resolutions;
    private void Start()
    {
        // InitResolution();
        // LoadOptions();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) 
        {
            _SetVolume(_volume);
        }
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

    #region SAVE STUFF
    public void SaveOptions()
    {
        SaveSystem.SavePlayerOptions(this);
    }

    public void LoadOptions()
    {
        InitActivateMenu();

        OptionsData loadedData = SaveSystem.LoadPlayerOptions();
        _volume = loadedData.volumePreference;
        _qualityIndex = loadedData.qualiltyIndex;
        // _isFullScreen = loadedData.isFullScreen;
        
        StartCoroutine(SetEverything());
    }

    public void InitLoadOptions() 
    {
        InitActivateMenu();
        OptionsData loadedData = SaveSystem.LoadPlayerOptions();
        _volume = loadedData.volumePreference;
        _qualityIndex = loadedData.qualiltyIndex;
        // _isFullScreen = loadedData.isFullScreen;
        StartCoroutine(InitSetEverything());
    }

    IEnumerator SetEverything()
    {
        yield return null;
        _SetVolume(_volume);
        // SetFullScreen(_isFullScreen);
        SetQuality(_qualityIndex);
        qualityDropdown.value = _qualityIndex;

        DeactivateMenu();
    }

    IEnumerator InitSetEverything()
    {
        yield return null;
        _SetVolume(_volume);
        // SetFullScreen(_isFullScreen);
        SetQuality(_qualityIndex);
        qualityDropdown.value = _qualityIndex;

        InitDeactivateMenu();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        _qualityIndex = GetQualityIndex();
        SaveOptions();
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        _isFullScreen = GetIsFullScreen();
        SaveOptions();
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
        _volume = GetAudioMixerVolume();
        SaveOptions();
    }

    private void _SetVolume(float volume)
    {
        float originalVolume = Mathf.Pow(10f, volume / 20f);
        volumeSlider.value = originalVolume;
        audioMixer.SetFloat("Volume", Mathf.Log10(originalVolume) * 20);
        _volume = GetAudioMixerVolume();
    }

    #endregion

    #region HELPERS
    public float GetAudioMixerVolume()
    {
        float volume;
        audioMixer.GetFloat("Volume", out volume);
        return volume;
    }

    public int GetQualityIndex()
    {
        return QualitySettings.GetQualityLevel();
    }

    public bool GetIsFullScreen()
    {
        return Screen.fullScreen;
    }

    public void PlayButtonClicked()
    {
        AudioManager.instance.PlayButtonClick();
    }
    #endregion


    public void ActivateMenu()
    {
        // set this menu to be active
        this.gameObject.SetActive(true);
        this.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
    }

    public void InitActivateMenu()
    {
        // set this menu to be active
        this.gameObject.SetActive(true);
        this.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        this.DeactivateMenu();
    }

    public void InitDeactivateMenu()
    {
        this.gameObject.SetActive(false);
        this.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }


}
