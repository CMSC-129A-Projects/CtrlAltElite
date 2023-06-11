using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField] private GameObject transition;
    private void Awake()
    {
        Debug.Log("Loader Awake");
        transition.SetActive(true);

    }
}
