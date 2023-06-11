using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class NewSaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    /*[SerializeField] private TextMeshProUGUI percentageCompleteText;
    [SerializeField] private TextMeshProUGUI deathCountText;*/
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI lastUpdated;
    [SerializeField] private TextMeshProUGUI medalsCollected;
    [SerializeField] private Image star;

    [Header("Clear Data Button")]
    [SerializeField] private Button clearButton;

    public bool hasData { get; private set; } = false;

    private Button saveSlotButton;

    private void Awake()
    {
        saveSlotButton = this.GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        // there's no data for this profileId
        if (data == null)
        {
            hasData = false;
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            clearButton.gameObject.SetActive(false);
        }
        // there is data for this profileId
        else
        {
            if (data.medalsCollected == data.totalMedals)
            {
                star.gameObject.SetActive(true);
            }
            else
            {
                star.gameObject.SetActive(false);
            }
            hasData = true;
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            clearButton.gameObject.SetActive(true);
            playerName.text = data.name;
            lastUpdated.text = DateTime.FromBinary(data.lastUpdated).ToShortDateString() + " " + DateTime.FromBinary(data.lastUpdated).ToShortTimeString();
            medalsCollected.text = "Medals: " + data.medalsCollected;
            /*percentageCompleteText.text = data.GetPercentageComplete() + "% COMPLETE";
            deathCountText.text = "DEATH COUNT: " + data.deathCount;*/
        }
    }

    public string GetProfileId()
    {
        return this.profileId;
    }

    public void SetInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
        clearButton.interactable = interactable;
    }

    public void DisableSaveSlotInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
        clearButton.interactable = !interactable;
    }

    public void DisableSaveSlotLoading(GameData data)
    {
        if (data.medalsCollected == data.totalMedals)
        {
            saveSlotButton.interactable = false;
            clearButton.interactable = true;
        }
    }
}