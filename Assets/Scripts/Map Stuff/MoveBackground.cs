using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    private float length;
    private Vector3 startPosition;
    private GameObject cam;
    [SerializeField] private float parallaxEffect;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera");
        startPosition = transform.position;
        length = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float distance = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startPosition.x + distance, cam.transform.position.y, transform.position.z);

        if (temp > startPosition.x + length)
        {
            startPosition.x += length;
        }
        else if (temp < startPosition.x - length)
        {
            startPosition.x -= length;
        }
    }
}
