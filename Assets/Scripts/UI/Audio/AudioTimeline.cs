using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTimeline : MonoBehaviour
{
    [SerializeField] private List<AudioSource> audios;

    private void Start()
    {
        // just to be sure set the audio here
        float volume = SaveSystem.LoadPlayerOptions().volumePreference;
        float originalVolume = Mathf.Pow(10f, volume / 20f);
        Debug.Log($"audiotimeline {originalVolume}");
        foreach (AudioSource audioSource in audios)
        {
            audioSource.outputAudioMixerGroup.audioMixer.SetFloat("Volume", Mathf.Log10(originalVolume) * 20);
        }      
    }
}
