using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.SceneManagement;

public class OptionsDataManager : MonoBehaviour
{
    public static OptionsDataManager Instance { get; private set; }
    private OptionsMenu optionsMenu;
    private string previousSceneName;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Found more than one OptionsDataManager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        optionsMenu = FindObjectOfType<OptionsMenu>(true);
        previousSceneName = SceneManager.GetActiveScene().name;
    }

    private void OnEnable()
    {
        try
        {
            // just to be sure set the audio here
            float volume = SaveSystem.LoadPlayerOptions().volumePreference;
            float originalVolume = Mathf.Pow(10f, volume / 20f);
            AudioManager.instance.GetComponent<AudioSource>().outputAudioMixerGroup.audioMixer.
                SetFloat("Volume", Mathf.Log10(originalVolume) * 20);
            optionsMenu.LoadOptions();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
        
    }

    private void Update()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        // if (currentSceneName == "CharacterCustomization") return;
        if (currentSceneName != previousSceneName)
        {
            previousSceneName = currentSceneName;
            OptionsMenu newOptionsMenu = FindObjectOfType<OptionsMenu>(true);
            if (newOptionsMenu != null) newOptionsMenu.LoadOptions();

        }
    }
}
