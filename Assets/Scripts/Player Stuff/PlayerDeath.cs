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
    public float[] respawnPosition = new float[3];
    private SugboMovement player;

    private void Awake()
    {
        currentRespawn = GameObject.FindGameObjectWithTag("Respawn");
        player = GetComponent<SugboMovement>();
        if (currentRespawn != null)
        {
            respawnPosition[0] = currentRespawn.transform.position.x;
            respawnPosition[1] = currentRespawn.transform.position.y;
            respawnPosition[2] = currentRespawn.transform.position.z;
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
        {
            currentBoundary = collision.gameObject.name;
            currentCollider = collision.gameObject.GetComponent<BoxCollider2D>();
            if (collision.gameObject.transform.childCount > 0)
            {
                /*currentRespawn = collision.transform.GetChild(0).gameObject;
                respawnPosition[0] = currentRespawn.transform.position.x;
                respawnPosition[1] = currentRespawn.transform.position.y;
                respawnPosition[2] = currentRespawn.transform.position.z;

                Debug.Log("AutoSave");
                player.SavePlayer();*/
                // Debug.Log("New Spawnpoint Found: Autosaving");
                currentRespawn = collision.transform.GetChild(0).gameObject;
                // Debug.Log(currentRespawn.transform.position);
                // DataPersistenceManager.instance.SaveGame();
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
        Debug.Log("Death");
        // var _deathScript = transform.GetComponent<TestMovement2>();
        // _deathScript.SetGravityScale(0);

        SugboMovement.isDead = true;
        SugboMovement.canMove = false;
        transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        // transform.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(_respawnTimer);
        // transform.GetComponent<SpriteRenderer>().enabled = true;
        // transform.position = currentRespawn.transform.position;
        // player.LoadPlayer();
        NewDataPersistenceManager.instance.LoadGame();
        transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(_animationTimer);
        SugboMovement.isDead = false;
        SugboMovement.canMove = true;
        // call SugboMovement.SavePlayer() here
        // _deathScript.SetGravityScale(_deathScript.data.gravityScale);
    }
}
