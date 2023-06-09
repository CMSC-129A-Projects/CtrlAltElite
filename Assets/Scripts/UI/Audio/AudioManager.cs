using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [SerializeField] private List<AudioClip> bgmClips = new List<AudioClip>();
    [SerializeField] private AudioSource bgm;
    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private List<AudioSource> jumps;
    [SerializeField] private AudioSource pickup;
    [SerializeField] private AudioSource death;
    [SerializeField] private AudioSource dash;
    [SerializeField] private AudioSource medal;
    [SerializeField] private AudioSource spring;

    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();

    private string previousSceneName;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one AudioManager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);   
    }

    private void Start()
    {
        previousSceneName = SceneManager.GetActiveScene().name;
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "TestMenuSave")
        {
            StopPlayingClips();
            bgm.clip = bgmClips[0];
            bgm.Play();
        }

        // testing purposes
        else if (sceneName == "City 3")
        {
            StopPlayingClips();
            bgm.clip = bgmClips[3];
            bgm.Play();
        }
    }

    private void Update()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "CharacterCustomization") return;
        if (currentSceneName != previousSceneName)
        {
            // Debug.Log("Scene has changed from " + previousSceneName + " to " + currentSceneName);
            previousSceneName = currentSceneName;
            StopPlayingClips();
            if (currentSceneName == "TestMenuSave")
            {
                bgm.clip = bgmClips[0];
                bgm.Play();
            }

            else if (currentSceneName == "NewIntroScene")
            {
                Debug.Log(true);
            }

            else if (currentSceneName == "City 1")
            {
                bgm.clip = bgmClips[1];
                bgm.Play();
            }

            else if (currentSceneName == "City 2")
            {
                bgm.clip = bgmClips[2];
                bgm.Play();
            }

            else if (currentSceneName == "City 3")
            {
                bgm.clip = bgmClips[3];
                bgm.Play();
            }
            else if (currentSceneName == "F City 4")
            {
                bgm.clip = bgmClips[4];
                bgm.Play();
            }
            else if (currentSceneName == "City 5")
            {
                bgm.clip = bgmClips[5];
                bgm.Play();
            }

        }
    }

    private void StopPlayingClips()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();
        }
    }
    public void PlayButtonClick()
    {
        buttonClick.Play();
    }

    public void PlayJump(bool baseJump)
    {
        if (baseJump) jumps[0].Play();
        else jumps[1].Play();
    }

    public void PlayPickup()
    {
        pickup.Play();
    }

    public void PlayDeath()
    {
        death.Play();
    }

    public void PlayDash()
    {
        dash.Play();
    }

    public void PlayMedal()
    {
        medal.Play();
    }

    public void PlaySpring()
    {
        spring.Play();
    }
}
