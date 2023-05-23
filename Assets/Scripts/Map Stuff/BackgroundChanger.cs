using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundChanger : MonoBehaviour
{
    [Header("Backgrounds")]
    [SerializeField] private List<GameObject> bgs = new List<GameObject>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Area"))
        {
            int index = bgs.FindIndex(obj => obj.name == collision.gameObject.name);
            Debug.Log(collision.gameObject.name + ", " + index);
            SetAllInactive();
            bgs[index].SetActive(true);
        }

        /*int index = bgs.FindIndex(obj => obj.name == collision.gameObject.name && collision.gameObject.CompareTag("Area"));
        Debug.Log(collision.gameObject.name + ", " + index);
        SetAllInactive();
        bgs[index].SetActive(true);*/
        /*// ALL HARD CODED
        if (collision.gameObject.name == "Sirao")
        {
            SetAllInactive();
            bgs[0].SetActive(true);
        }

        if (collision.gameObject.name == "StoNino")
        {
            SetAllInactive();
            bgs[1].SetActive(true);
        }

        if (collision.gameObject.name == "Magellan")
        {
            SetAllInactive();
            bgs[2].SetActive(true);
        }

        if (collision.gameObject.name == "Fort")
        {
            SetAllInactive();
            bgs[3].SetActive(true);
        }

        if (collision.gameObject.name == "CCLEX")
        {
            SetAllInactive();
            bgs[4].SetActive(true);
        }*/


    }

    private void SetAllInactive()
    {
        for (int i = 0; i < bgs.Count; i++)
        {
            bgs[i].SetActive(false);
        }
    }
}
