using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCollider : MonoBehaviour
{
    public GameObject currentPlatform;
    [SerializeField] public PlayerMovement playerMovement;
    private CapsuleCollider2D playerCollider;

    private int respawnPlatformTimer = 4;
    private float breakPlatformTimer = 4;


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
