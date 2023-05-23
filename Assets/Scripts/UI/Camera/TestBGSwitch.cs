using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestBGSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Backgrounds")]
    [SerializeField] private List<GameObject> bgs = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Sirao")
        {
            SetAllInactive();
            bgs[0].SetActive(true);
        }

        if (collision.gameObject.name == "Sto Nino")
        {
            SetAllInactive();
            bgs[1].SetActive(true);
        }

        if (collision.gameObject.name == "Magellan")
        {
            SetAllInactive();
            bgs[2].SetActive(true);
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
