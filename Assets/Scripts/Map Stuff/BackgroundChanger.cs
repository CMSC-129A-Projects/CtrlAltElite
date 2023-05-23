using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundChanger : MonoBehaviour
{

    public GameObject mainBackground;
    public GameObject mainBackground2;

    void OnCollisionEnter(Collision collision) // Use OnTriggerEnter if using a trigger collider
    {
        if (collision.gameObject.CompareTag("BasePlayer")) // Replace "Character" with the tag of your character object
        {
            mainBackground.SetActive(false);
            mainBackground2.SetActive(true);
        }
    }

}
