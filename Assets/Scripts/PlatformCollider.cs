using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformCollider : MonoBehaviour
{
    public GameObject currentPlatform;
    [SerializeField] public PlayerMovement playerMovement;
    private CapsuleCollider2D playerCollider;

    private int respawnPlatformTimer = 4;
    private float breakPlatformTimer = 4;
    public bool inHidden = false;


    private void Start()
    {
        playerCollider = playerMovement.GetComponent<CapsuleCollider2D>();
    }
    // Update is called once per frame
    void Update()
    {
        if (playerMovement.moveInput.y == -1)
        {
            //if (LayerMask.LayerToName(currentPlatform.layer) == "TwoWay")
            if (currentPlatform != null)
            {
                if (LayerMask.LayerToName(currentPlatform.layer) == "TwoWay")
                {
                    StartCoroutine(DisableCollision());
                }
                    
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (LayerMask.LayerToName(collision.collider.gameObject.layer) == "OneWay" && playerMovement.isOnPlatform)
        {
            currentPlatform = collision.gameObject;
        }

        if (LayerMask.LayerToName(collision.collider.gameObject.layer) == "TwoWay" && playerMovement.isOnPlatform)
        {   
            currentPlatform = collision.gameObject;   
        }

        if (LayerMask.LayerToName(collision.collider.gameObject.layer) == "Breakable" && playerMovement.isOnPlatform)
        {
            currentPlatform = collision.gameObject;
            StartCoroutine(BreakPlatform(currentPlatform));
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (LayerMask.LayerToName(collision.collider.gameObject.layer) == "OneWay")
        {
            currentPlatform = null;
        }

        if (LayerMask.LayerToName(collision.collider.gameObject.layer) == "TwoWay")
        {
            currentPlatform = null;
            Debug.Log("TwoWay Exit");
        }

        if (LayerMask.LayerToName(collision.collider.gameObject.layer) == "Breakable")
        {
            currentPlatform = null;
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hidden")) // if player touches hidden tiles, hidden tiles get hidden
        {
            TilemapRenderer tilemapRenderer = collision.gameObject.GetComponent<TilemapRenderer>(); // renderer of the tiles
            tilemapRenderer.enabled = false; // set renderer to false to "hide" tiles
            inHidden = true;
        }

        if (collision.gameObject.CompareTag("HiddenOutCheck")) // if player touches hidden tiles, hidden tiles get hidden
        {
            inHidden = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hidden") && !inHidden) // if player leaves hidden tiles, hidden tiles get shown
        {
            StartCoroutine(EnableTile(collision));
        }
    }

    private IEnumerator EnableTile(Collider2D collision)
    {
        yield return new WaitForSeconds(0.5f);
        TilemapRenderer tilemapRenderer = collision.gameObject.GetComponent<TilemapRenderer>(); // renderer of the tiles
        tilemapRenderer.enabled = true; // set renderer to true to "show" tiles
    }



    private IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = currentPlatform.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(playerCollider, platformCollider);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);


    }

    private IEnumerator BreakPlatform(GameObject currentPlatform)
    {
        yield return new WaitForSeconds(breakPlatformTimer);
        currentPlatform.SetActive(false); //deactivates platform
        StartCoroutine(RespawnPlatform(currentPlatform));
    }


    private IEnumerator RespawnPlatform(GameObject currentPlatform)
    {
        yield return new WaitForSeconds(respawnPlatformTimer);
        currentPlatform.SetActive(true);
    }
}
