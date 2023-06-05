using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public string currentBoundary;
    public static BoxCollider2D currentCollider;
    public static GameObject currentRespawn;
    [SerializeField] private float _respawnTimer;
    [SerializeField] private float _animationTimer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
        {
            currentBoundary = collision.gameObject.name;
            currentCollider = collision.gameObject.GetComponent<BoxCollider2D>();
            if (collision.gameObject.transform.childCount > 0)
            {
                currentRespawn = collision.transform.GetChild(0).gameObject;
                NewDataPersistenceManager.instance.SaveGame();
            }
        }

        if (collision.gameObject.CompareTag("Spikes"))
        {
            StartCoroutine(StartRespawn());
        }
    }

    public IEnumerator StartRespawn()
    {
        SugboMovement.isDead = true;
        SugboMovement.canMove = false;
        transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        yield return new WaitForSeconds(_respawnTimer);

        NewDataPersistenceManager.instance.LoadGame();
        transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(_animationTimer);
        SugboMovement.isDead = false;
        SugboMovement.canMove = true;
    }
}
