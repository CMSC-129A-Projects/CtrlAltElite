using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class OptionsDataManager : MonoBehaviour
{
    public static OptionsDataManager Instance { get; private set; }
    private OptionsMenu optionsMenu;

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
    }

    private void OnEnable()
    {
        try
        {
            Debug.Log("trying");
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

    private void OnDisable()
    {
        Debug.Log("OPTIONS DISABLE");
        // optionsMenu.SaveOptions();
    }




}
