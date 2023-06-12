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
    [SerializeField] private float dimSpeed;
    private GameObject player = null;

    private void Update()
    {
        if (followPlayer && globalLight.intensity >= 0)
        {
            globalLight.intensity -= Time.deltaTime * dimSpeed;
            
        }

        if (spotLight.intensity >= 1)
        {
            spotLight.intensity = 1;
        }


        if (globalLight.intensity <= 0.5f)
        {
            spotLight.transform.position = player.transform.position;
            spotLight.intensity += Time.deltaTime * dimSpeed/2;
        }

        if (globalLight.intensity <= 0 && followPlayer)
        {
            globalLight.intensity = 0;
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            spotLight.gameObject.SetActive(true);
            followPlayer = true;
            player = collision.gameObject;
            // globalLight.intensity = 0;
            spotLight.intensity = 0f;
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
