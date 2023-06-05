using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    private SugboMovement player;
    private void Awake()
    {
        player = FindObjectOfType<SugboMovement>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && player.isOnPlatform && !player.inAir)
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || !player.isOnPlatform)
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
