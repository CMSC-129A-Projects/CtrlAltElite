using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Flags : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject lore;
    public void OnPointerEnter(PointerEventData eventData)
    {
        lore.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        lore.SetActive(false);
    }
}
