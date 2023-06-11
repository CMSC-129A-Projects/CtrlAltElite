using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightBoundary : MonoBehaviour
{
    private bool followPlayer = false;
    [SerializeField] private Light2D globalLight;
    [SerializeField] private float globalLightIntensity = 1f;
    [SerializeField] private Light2D spotLight;
    [SerializeField] private float spotLightOuterRadius;
    private GameObject player = null;

    private void Update()
    {
        if (followPlayer)
        {
            
            spotLight.transform.position = player.transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            spotLight.gameObject.SetActive(true);
            followPlayer = true;
            player = collision.gameObject;
            globalLight.intensity = 0;
            spotLight.pointLightOuterRadius = spotLightOuterRadius;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            spotLight.gameObject.SetActive(false);
            followPlayer = false;
            globalLight.intensity = globalLightIntensity;
        }
    }

}
