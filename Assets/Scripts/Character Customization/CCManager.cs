using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCManager : MonoBehaviour
{
    [SerializeField] private GameObject CCMenu;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Area"))
        {
            CCMenu.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Area"))
        {
            CCMenu.SetActive(false);
        }
    }
}
