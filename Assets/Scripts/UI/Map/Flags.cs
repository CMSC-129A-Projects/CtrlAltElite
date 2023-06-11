using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Flags : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject lore;
    private CanvasGroup canvasGroup;
    public bool fadeOutAnimation = false;
    public bool fadeInAnimation = false;
    private float _timer = 0f;
    [SerializeField] private float fadeInTime;
    [SerializeField] private float fadeOutTime;

    private void Start()
    {
        canvasGroup = lore.GetComponent<CanvasGroup>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        fadeInAnimation = true;
        fadeOutAnimation = false;
        _timer = 0f;
        lore.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //lore.SetActive(false);
        fadeInAnimation = false;
        fadeOutAnimation = true;
    }

    private void Update()
    {
        if (fadeInAnimation)
        {
            _timer += Time.deltaTime;
            // Calculate the normalized progress of the animation
            float progress = _timer / fadeInTime;
            // Increase the image's color alpha based on the progress
            canvasGroup.alpha = progress;

            if (_timer >= fadeInTime)
            {
                _timer = 0f;
                fadeInAnimation = false;
            }
        }

        if (fadeOutAnimation)
        {
            _timer += Time.deltaTime;

            // Calculate the normalized progress of the animation
            float progress = _timer / fadeOutTime;

            // Reduce the image's color alpha based on the progress
            canvasGroup.alpha = 1f - progress;

            if (_timer >= fadeOutTime)
            {
                _timer = 0f;
                fadeOutAnimation = false;
                lore.SetActive(false);
            }
        }
    }
}
