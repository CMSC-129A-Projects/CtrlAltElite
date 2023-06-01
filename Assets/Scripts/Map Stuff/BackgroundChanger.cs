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
            SetAllInactive();
            bgs[index].SetActive(true);
        }
    }

    private void SetAllInactive()
    {
        for (int i = 0; i < bgs.Count; i++)
        {
            bgs[i].SetActive(false);
        }
    }
}
